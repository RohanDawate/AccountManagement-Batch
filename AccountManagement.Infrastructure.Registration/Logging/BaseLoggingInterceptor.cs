using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public abstract class BaseLoggingInterceptor : IInterceptor
    {
        protected readonly ILogger _logger;
        protected readonly string _contextType;
        private readonly LoggingOptions _options;

        protected BaseLoggingInterceptor(ILogger logger, string contextType, IOptions<LoggingOptions> options)
        {
            _logger = logger;
            _contextType = contextType; 
            _options = options.Value;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_options.Enabled)
            {
                // Logging disabled → just run the method
                invocation.Proceed();
                return;
            }

            var className = invocation.TargetType?.Name ?? "<UnknownClass>";
            var methodName = invocation.Method?.Name ?? "<UnknownMethod>";

            LogEntry(invocation, className, methodName);

            var sw = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Task task)
                {
                    invocation.ReturnValue = InterceptAsync((dynamic)task, className, methodName, sw, invocation);
                }
                else
                {
                    sw.Stop();
                    LogExit(invocation, className, methodName, sw.ElapsedMilliseconds, invocation.ReturnValue);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                LogError(invocation, className, methodName, sw.ElapsedMilliseconds, ex);
                throw;
            }
        }

        private async Task InterceptAsync(Task task, string className, string methodName, Stopwatch sw, IInvocation invocation)
        {
            await task;
            sw.Stop();
            LogExit(invocation, className, methodName, sw.ElapsedMilliseconds, null);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, string className, string methodName, Stopwatch sw, IInvocation invocation)
        {
            var result = await task;
            sw.Stop();
            LogExit(invocation, className, methodName, sw.ElapsedMilliseconds, result);
            return result;
        }

        protected object BuildArgs(IInvocation invocation)
        {
            if (!_options.EnableArgumentLogging)
                return new { }; // empty object

            var parameters = invocation.Method.GetParameters();
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var name = parameters[i].Name ?? $"arg{i}";
                var value = invocation.Arguments[i];

                if (value is CancellationToken ct)
                {
                    if (_options.LogCancellationToken)
                    {
                        if (_options.LogCancellationTokenDetails)
                        {
                            dict[name] = new
                            {
                                ct.IsCancellationRequested,
                                ct.CanBeCanceled
                            };
                        }
                        else
                        {
                            dict[name] = "<CancellationToken omitted>";
                        }
                    }
                    else
                        continue;                        
                }
                else
                {
                    dict[name] = value ?? "<null>";
                }
            }
            return dict;
        }

        protected string CleanException(Exception ex)
        {
            var lines = ex.ToString()
                .Split(Environment.NewLine)
                .Where(l => !l.Contains("System.Runtime") &&
                            !l.Contains("Microsoft.Extensions") &&
                            !l.Contains("Castle.DynamicProxy"))
                .ToArray();
            return string.Join(Environment.NewLine, lines);
        }

        protected abstract void LogEntry(IInvocation invocation, string className, string methodName);
        protected abstract void LogExit(IInvocation invocation, string className, string methodName, long durationMs, object? result);
        protected abstract void LogError(IInvocation invocation, string className, string methodName, long durationMs, Exception ex);
    }
}
