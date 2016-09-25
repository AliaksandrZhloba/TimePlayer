using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace TimePlayer.Commands
{
	public class ConfirmDeleteVideoPartCommand : ICommand
	{
		private Action _executed;


		public ConfirmDeleteVideoPartCommand(Action executed)
		{
			_executed = executed;
		}


		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;


		public void Execute(object parameter)
		{
			_executed();
		}
	}
}
