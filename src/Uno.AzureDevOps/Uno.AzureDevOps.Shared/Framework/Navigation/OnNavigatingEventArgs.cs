using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.AzureDevOps.Framework.Navigation
{
	public class OnNavigatingEventArgs : EventArgs
	{
		public OnNavigatingEventArgs(string pageKey)
		{
			PageKey = pageKey;
		}

		public string PageKey { get; }
	}
}
