namespace ImageProcessing
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <remarks>
    /// The idea of this code was taken from the blog post of John Thiriet.
    /// Some minor modifications where applied.
    /// Source: https://johnthiriet.com/mvvm-going-async-with-async-command/ (03.06.2021)
    /// </remarks>
    public class AsyncRelayCommand : ICommand
    {
        private bool isExecuting;
        private readonly Func<object?, Task> execute;
        private readonly Func<object?, bool>? canExecute;

        public AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
        {
            this.isExecuting = false;
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (this.isExecuting)
            {
                return false;
            }

            return this.canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (!this.CanExecute(parameter))
            {
                return;
            }

            try
            {
                this.isExecuting = true;
                Task.Run(() => this.execute(parameter))
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult();
            }
            finally
            {
                this.isExecuting = false;
            }
        }
    }
}