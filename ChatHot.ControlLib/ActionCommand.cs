using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChatHot.ControlLib
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action ExecuteAction=null;
        private Action<Object> ExecuteActionArg = null;
        public ActionCommand(Action Act)
        {
            ExecuteAction = Act;
        }
        public ActionCommand(Action<Object> Act)
        {
            ExecuteActionArg = Act;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                Execute();return;
            }
            ExecuteActionArg?.Invoke(parameter);
        }
        public void Execute()
        {
            ExecuteAction?.Invoke();
        }
    }
}
