using System.Threading.Tasks;

namespace Uno.AzureDevOps.Framework.Tasks
{
	public interface ITaskNotifier<T> : ITaskNotifier
	{
		new Task<T> Task { get; }

		new T Result { get; }
	}
}
