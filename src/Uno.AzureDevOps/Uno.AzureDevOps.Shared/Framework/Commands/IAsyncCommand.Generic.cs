using System.Threading.Tasks;
using System.Windows.Input;

namespace Uno.AzureDevOps.Framework.Commands
{
	public interface IAsyncCommand<T> : ICommand
	{
		Task ExecuteAsync(T parameter);

		bool CanExecute(T parameter);
	}
}
