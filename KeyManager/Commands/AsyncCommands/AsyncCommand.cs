using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyManager.Commands.AsyncCommands
{
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<CancellationToken, object, Task<TResult>> _command;
        private readonly CancelAsyncCommand _cancelCommand;
        private NotifyTaskCompletion<TResult> _execution;

        /// The object given as a parameter can be set on a gui object by a property "CommandParameter"
        private readonly Predicate<object> _canExecutePredicate;

        public AsyncCommand(Func<CancellationToken, object, Task<TResult>> command, Predicate<object> canExecutePredicate)
        {
            _command = command;
            _cancelCommand = new CancelAsyncCommand();
            _canExecutePredicate = canExecutePredicate;
        }

        // parameter: the CommandParameter if defined
        public override bool CanExecute(object parameter)
        {
            // check predicate if exists
            if (_canExecutePredicate != null)
                return _canExecutePredicate(parameter);
            // otherwise check if execution is completed
            return Execution == null || Execution.IsCompleted;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token, parameter));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            _cancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public NotifyTaskCompletion<TResult> Execution
        {
            get { return _execution; }
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        public bool HasExecution()
        {
            return Execution != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class CancelAsyncCommand : ICommand
        {
            private CancellationTokenSource _cts = new CancellationTokenSource();
            private bool _commandExecuting;

            public CancellationToken Token { get { return _cts.Token; } }

            public void NotifyCommandStarting()
            {
                _commandExecuting = true;
                if (!_cts.IsCancellationRequested)
                    return;
                _cts = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                _commandExecuting = false;
                RaiseCanExecuteChanged();
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _commandExecuting && !_cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                _cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    // static factory methods for creating AsyncCommands
    public static class AsyncCommand
    {
        /// <summary>
        /// Creates an AsyncCommand. 
        /// </summary>
        /// <param name="command">A lambda returning a Task with a parameter of type object</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<Task> command)
        {
            return new AsyncCommand<object>(async (cancTok, param) => { await command(); return null; }, null);
        }

        /// <summary>
        /// Creates an AsyncCommand. Command takes an object as argument.
        /// </summary>
        /// <param name="command">A lambda returning a Task with a parameter of type object</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<object, Task> command)
        {
            return new AsyncCommand<object>(async (cancTok, param) => { await command(param); return null; }, null);
        }

        /// <summary>
        /// Creates an AsyncCommand with additional canExecute predicate for disabling bound control. 
        /// </summary>
        /// <param name="command">A lambda returning a Task with a parameter of type object</param>
        /// <param name="canExecutePredicate">Predicate taking the CommandParameter as agument</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<Task> command, Predicate<object> canExecutePredicate)
        {
            return new AsyncCommand<object>(async (cancTok, param) => { await command(); return null; }, canExecutePredicate);
        }

        /// <summary>
        /// Creates an AsyncCommand with additional canExecute predicate for disabling bound control. Command takes an object as argument.
        /// </summary>
        /// <param name="command">A lambda returning a Task with a parameter of type object</param>
        /// <param name="canExecutePredicate">Predicate taking the CommandParameter as agument</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<object, Task> command, Predicate<object> canExecutePredicate)
        {
            return new AsyncCommand<object>(async (cancTok, param) => { await command(param); return null; }, canExecutePredicate);
        }
    


        /*
          public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
        {
            return new AsyncCommand<TResult>( (cancTok, param) => command(), null);
        }


        /// <summary>
        /// Creates a cancellable AsyncCommand 
        /// </summary>
        /// <param name="command">A lambda taking a CancellationToken and returning a Task</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<CancellationToken, Task> command)
        {
            return new AsyncCommand<object>(async (cancTok, param) => { await command(cancTok); return null; }, null);
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, object, Task<TResult>> command)
        {
            return new AsyncCommand<TResult>(command, null);
        }
        

        
        /// <summary>
        /// Creates an AsyncCommand with TResult as task return type with additional canExecute predicate for disabling bound control
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="command">A lambda returning a TResult</param>
        /// <param name="canExecutePredicate">Predicate taking the CommandParameter as agument</param>
        /// <returns></returns>
        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command, Predicate<object> canExecutePredicate)
        {
            return new AsyncCommand<TResult>( (cancTok, param) => command(), canExecutePredicate);
        }
        */
    }
}