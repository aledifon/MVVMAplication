using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMApplication.MVVM
{
    internal class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Func<object?, bool> _canExecute;
        private bool _isExecuting;

        // Custom Personaliz. of adding/removing event Subscriptors
        public event EventHandler? CanExecuteChanged;
        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool> canExecute = null)
        {
            this._executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this._canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            // Disable while is executing and combines external logic from canExecute
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();            // Disable the button during the execution

                await _executeAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();           // Re-enable the button again
            }
            
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
