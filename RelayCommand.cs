using System;
using System.Threading;
using System.Windows.Input;

namespace SimpleSmtpMailSender
{
    public class RelayCommand : ICommand
    {
        private readonly bool _haveParameters;
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Action _executeWithoutParameters;
        private readonly Func<bool> _canExecuteWithoutParameters;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _haveParameters = true;
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action executeWithoutParameters, Func<bool> canExecuteWithoutParameters = null)
        {
            _haveParameters = false;
            _executeWithoutParameters = executeWithoutParameters ?? throw new ArgumentNullException(nameof(executeWithoutParameters));
            _canExecuteWithoutParameters = canExecuteWithoutParameters;
        }

        public void Execute(object parameter)
        {
            if (_haveParameters)
            {
                _execute(parameter);
            }
            else
            {
                _executeWithoutParameters();
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_haveParameters)
            {
                return _canExecute == null || _canExecute(parameter);
            }
            else
            {
                return _canExecuteWithoutParameters == null || _canExecuteWithoutParameters();
            }
        }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged(EventArgs e = null)
        {
            var canExecuteChanged = Interlocked.CompareExchange(ref CanExecuteChanged, null, null);
            canExecuteChanged?.Invoke(this, e);
        }
    }
}