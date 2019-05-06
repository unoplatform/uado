using System.Threading.Tasks;
using System.Windows.Input;

namespace Uno.AzureDevOps.Framework.Commands
{
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync();

		bool CanExecute();
	}
}
