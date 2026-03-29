using AccountManagement.Infrastructure.Logging;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using System.Text.Json;

namespace AccountManagement.Infrastructure
{
    public sealed class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new ()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation == null)
            {
                Log.Warning("Intercept called with null invocation");
                return;
            }

            var className = invocation?.TargetType?.Name ?? "<UnknownClass>";
            var methodName = invocation?.Method.Name ?? "<UnknownMethod>";

            // ENTER
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var entryLog = new BatchLogEntry
                {
                    Direction = "ENTRY",
                    Class = className,
                    Method = methodName,
                    Args = BuildArgs(invocation)
                };
                Log.Information("{@Entry}", entryLog);

                //Log.Information(JsonSerializer.Serialize(entryLog, _jsonOptions));
            }

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
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        var exitLog = new BatchLogEntry
                        {
                            Direction = "EXIT",
                            Class = className,
                            Method = methodName,
                            DurationMs = sw.ElapsedMilliseconds,
                            Args = BuildArgs(invocation),
                            Result = invocation.ReturnValue
                        };
                        Log.Information("{@BatchLogEntry}", exitLog);
                    }
                }

            }
            catch (Exception ex)
            {
                sw.Stop();
                var errorLog = new BatchLogEntry
                {
                    Direction = "EXIT",
                    Class = className,
                    Method = methodName,
                    DurationMs = sw.ElapsedMilliseconds,
                    Args = BuildArgs(invocation),
                    Exception = CleanException(ex)
                };
                Log.Error("{@BatchLogEntry}", errorLog);
                throw;

            }
        }

        private async Task InterceptAsync(Task task, string className, string methodName, Stopwatch sw, IInvocation invocation)
        {
            await task;
            sw.Stop();
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var exitLog = new BatchLogEntry
                {
                    Direction = "EXIT",
                    Class = className,
                    Method = methodName,
                    DurationMs = sw.ElapsedMilliseconds,
                    Args = BuildArgs(invocation)
                };
                Log.Information("{@Entry}", exitLog);
            }
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, string className, string methodName, Stopwatch sw, IInvocation invocation)
        {
            var result = await task;
            sw.Stop();
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var exitLog = new BatchLogEntry
                {
                    Direction = "EXIT",
                    Class = className,
                    Method = methodName,
                    DurationMs = sw.ElapsedMilliseconds,
                    Args = BuildArgs(invocation),
                    Result = result
                };
                Log.Information("{@Entry}", exitLog);
            }
            return result;
        }

        private object BuildArgs(IInvocation invocation)
        {
            var parameters = invocation.Method.GetParameters();
            var dict = new Dictionary<string, object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var name = parameters[i].Name;
                var value = invocation.Arguments[i];

                if (value is CancellationToken ct)
                {
                    dict[name] = new { ct.IsCancellationRequested, ct.CanBeCanceled };
                }
                else
                {
                    dict[name] = value;
                }
            }

            return dict;
        }

        private string CleanException(Exception ex)
        {
            // Only keep the message and the first few relevant frames
            var lines = ex.ToString()
                .Split(Environment.NewLine)
                .Where(l => !l.Contains("System.Runtime") &&
                            !l.Contains("Microsoft.Extensions") &&
                            !l.Contains("Castle.DynamicProxy"))
                .ToArray();

            return string.Join(Environment.NewLine, lines);
        }


    }
}
