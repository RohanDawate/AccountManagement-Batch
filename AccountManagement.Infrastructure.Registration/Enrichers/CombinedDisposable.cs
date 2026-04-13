namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    internal class CombinedDisposable : IDisposable
    {
        private readonly IDisposable _logContextDisposable;
        private readonly Action _undoAction;

        public CombinedDisposable(IDisposable logContextDisposable, Action undoAction)
        {
            _logContextDisposable = logContextDisposable;
            _undoAction = undoAction;
        }

        public void Dispose()
        {
            _logContextDisposable.Dispose(); // Pops the property from Serilog
            _undoAction.Invoke();            // Resets our AsyncLocal variables
        }
    }
}
