using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Uno.AzureDevOps.Framework.Commands
{
	public sealed class AsyncCommand<T> : IAsyncCommand<T>
	{
		private readonly Func<T, Task> _execute;
		private readonly Func<T, bool> _canExecute;

		private bool _isExecuting;

		public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(T parameter)
		{
			return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
		}

		public async Task ExecuteAsync(T parameter)
		{
			if (CanExecute(parameter))
			{
				try
				{
					_isExecuting = true;
					await _execute(parameter);
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
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			try
			{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				ExecuteAsync((T)parameter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
			catch
			{
				// swallow exception, if needed add an error handler to the command
			}
		}
	}
}
