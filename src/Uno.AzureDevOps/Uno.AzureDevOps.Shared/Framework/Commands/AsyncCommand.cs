using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Uno.AzureDevOps.Framework.Commands
{
	public sealed class AsyncCommand : IAsyncCommand
	{
		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;

		private bool _isExecuting;

		public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute()
		{
			return !_isExecuting && (_canExecute?.Invoke() ?? true);
		}

		public async Task ExecuteAsync()
		{
			if (CanExecute())
			{
				try
				{
					_isExecuting = true;
					await _execute();
				}
				finally
				{
					_isExecuting = false;
				}
			}

			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute();
		}

		void ICommand.Execute(object parameter)
		{
			try
			{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				ExecuteAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
			catch
			{
				// swallow exception, if needed add an error handler to the command
			}
		}
	}
}
