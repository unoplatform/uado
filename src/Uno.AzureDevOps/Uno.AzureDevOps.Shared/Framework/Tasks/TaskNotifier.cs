using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

#if !__WASM__
using Xamarin.Essentials;
#endif

namespace Uno.AzureDevOps.Framework.Tasks
{
	public class TaskNotifier<TResult> : ITaskNotifier<TResult>
	{
		private readonly Action<Exception> _onFaulted;
		private readonly TaskScheduler _dispatcherTaskScheduler;

		public TaskNotifier(Task<TResult> task, Action<Exception> onFaulted = null, TaskScheduler dispatcherTaskScheduler = null)
		{
			_dispatcherTaskScheduler = dispatcherTaskScheduler;
			_onFaulted = onFaulted;

			Task = task;
			if (!task.IsCompleted)
			{
				RunTask(task);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Task<TResult> Task { get; }

		public TResult Result => Task.Status == TaskStatus.RanToCompletion
					? Task.Result
					: default;

		public TaskStatus Status => Task.Status;

		public bool IsCanceled => Task.IsCanceled;

		public bool IsCompleted => Task.IsCompleted;

		public bool IsExecuting => !Task.IsCompleted;

		public bool IsFaulted => Task.IsFaulted;

		public bool IsSuccess => Task.Status == TaskStatus.RanToCompletion;

		public bool IsInternetFaulted { get; set; }

		public AggregateException Exception => Task.Exception;

		Task ITaskNotifier.Task => Task;

		object ITaskNotifier.Result => Result;

		private static TaskScheduler GetDefaultScheduler()
		{
			return SynchronizationContext.Current == null
				? TaskScheduler.Current
				: TaskScheduler.FromCurrentSynchronizationContext();
		}

		private void RunTask(Task taskToExecute)
		{
			taskToExecute.ContinueWith(
				task =>
				{
					if (task.IsFaulted)
					{
#if !__WASM__
						if (Connectivity.NetworkAccess != NetworkAccess.Internet)
						{
							IsInternetFaulted = true;
						}
#endif
						Console.Error.WriteLine(task.Exception);
						_onFaulted?.Invoke(task.Exception);
					}

#if NETFX_CORE
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
#else
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccess)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInternetFaulted)));
#endif
				},
				scheduler: _dispatcherTaskScheduler ?? GetDefaultScheduler());
		}
	}
}
