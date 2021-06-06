namespace ImageProcessing
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <remarks>
    /// The ideas of this code were taken from the following sources:
    /// - https://johnthiriet.com/mvvm-going-async-with-async-command/ (03.06.2021)
    /// - https://www.youtube.com/watch?v=dbh1st68Tco (04.06.2021)
    /// </remarks>
    public class AsyncRelayCommand : ICommand
    {
        private bool isExecuting;
        private readonly Func<object?, Task> executeAsync;
        private readonly Func<object?, bool>? canExecute;

        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
        {
            this.isExecuting = false;
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
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

        public async void Execute(object? parameter)
        {
            if (!this.CanExecute(parameter))
            {
                return;
            }

            try
            {
                this.isExecuting = true;
                await this.executeAsync(parameter);
            }
            finally
            {
                this.isExecuting = false;
            }
        }
    }
}