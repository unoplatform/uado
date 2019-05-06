using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Framework.Tasks
{
	public interface ITaskNotifier : INotifyPropertyChanged
	{
		Task Task { get; }

		object Result { get; }

		TaskStatus Status { get; }

		bool IsCanceled { get; }

		bool IsCompleted { get; }

		bool IsFaulted { get; }

		bool IsExecuting { get; }

		bool IsSuccess { get; }

		AggregateException Exception { get; }
	}
}
