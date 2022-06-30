using System;
using System.Windows.Input;

namespace KeyManager.Commands
{
    /// <summary>
    /// The ViewModelCommand class - an ICommand that can fire a function.
    /// Based on the class shown here : http://www.codeproject.com/Articles/274982/Commands-in-MVVM
    /// </summary>
    public class RelayCommand : ICommand
    {   /// <summary>
        // if true, the CommandManager will requery the CanExecuteChanged event
        /// </summary>
        private readonly bool _requery;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        /// <param name="requery">if set to <c>true</c> requery is enabled.</param>
        public RelayCommand(Action action, bool canExecute = true, bool requery = false)
        {
            //  Set the action.
            Action = action;
            _canExecute = canExecute;
            _requery = requery;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecutePredicate">predicate for can execute</param>
        /// /// <param name="requery">if set to <c>true</c> requery is enabled.</param>
        public RelayCommand(Action action, Predicate<object> canExecutePredicate, bool requery = false)
        {
            Action = action;
            _canExecutePredicate = canExecutePredicate;
            _requery = requery;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        /// <param name="requery">if set to <c>true</c> requery is enabled.</param>
        public RelayCommand(Action<object> parameterizedAction, bool canExecute = true, bool requery = false)
        {
            //  Set the action.
            ParameterizedAction = parameterizedAction;
            _canExecute = canExecute;
            _requery = requery;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        /// <param name="canExecutePredicate">predicate for canExecute </param>
        /// <param name="requery">if set to <c>true</c> requery is enabled.</param>
        public RelayCommand(Action<object> parameterizedAction, Predicate<object> canExecutePredicate, bool requery = false)
        {
            //  Set the action.
            ParameterizedAction = parameterizedAction;
            _canExecutePredicate = canExecutePredicate;
            _requery = requery;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(object param)
        {
            //  Invoke the executing command, allowing the command to be cancelled.
            CancelCommandEventArgs args = new CancelCommandEventArgs() { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  If the event has been cancelled, bail now.
            if (args.Cancel)
                return;

            //  Call the action or the parameterized action, whichever has been set.
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs() { Parameter = param });
        }

        protected void InvokeAction(object param)
        {
            Action theAction = Action;
            Action<object> theParameterizedAction = ParameterizedAction;
            if (theAction != null)
                theAction();
            else
                theParameterizedAction?.Invoke(param);
        }

        protected void InvokeExecuted(CommandEventArgs args)
        {
            CommandEventHandler executed = Executed;

            //  Call the executed event.
            executed?.Invoke(this, args);
        }

        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            CancelCommandEventHandler executing = Executing;

            //  Call the executed event.
            executing?.Invoke(this, args);
        }


        /// <summary>
        /// The action (or parameterized action) that will be called when the command is invoked.
        /// </summary>
        protected Action Action;

        protected Action<object> ParameterizedAction;

        /// <summary>
        /// Bool indicating whether the command can execute.
        /// </summary>
        private bool _canExecute;


        /// The object given as a parameter can be set on a gui object by a property "CommandParameter"
        private readonly Predicate<object> _canExecutePredicate;



        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute.
        /// Used to manually trigger CanExecuteChanged
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        /// 

        public bool RaiseCanExecuteChanged
        {
            get { return _canExecute; }
            set
            {
                if (_canExecute != value)
                {
                    _canExecute = value;
                    EventHandler canExecuteChanged = _canExecuteChangedHandler;
                    canExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecutePredicate != null)
            {
                return _canExecutePredicate(parameter);
            }

            return _canExecute;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            DoExecute(parameter);
        }

        #endregion


        private EventHandler _canExecuteChangedHandler;
        /// <summary>
        /// Occurs when can execute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_requery)
                    CommandManager.RequerySuggested += value;
                _canExecuteChangedHandler += value;
            }
            remove
            {
                if (_requery)
                    CommandManager.RequerySuggested -= value;
                _canExecuteChangedHandler -= value;
            }
        }

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event CancelCommandEventHandler Executing;

        /// <summary>
        /// Occurs when the command executed.
        /// </summary>
        public event CommandEventHandler Executed;
    }

    /// <summary>
    /// The CommandEventHandler delegate.
    /// </summary>
    public delegate void CommandEventHandler(object sender, CommandEventArgs args);

    /// <summary>
    /// The CancelCommandEvent delegate.
    /// </summary>
    public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);

    /// <summary>
    /// CommandEventArgs - simply holds the command parameter.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public object Parameter { get; set; }
    }

    /// <summary>
    /// CancelCommandEventArgs - just like above but allows the event to 
    /// be cancelled.
    /// </summary>
    public class CancelCommandEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
}
