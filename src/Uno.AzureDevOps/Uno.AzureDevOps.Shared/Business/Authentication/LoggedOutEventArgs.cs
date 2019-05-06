using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.AzureDevOps.Business.Authentication
{
	public class LoggedOutEventArgs : EventArgs
	{
		public Exception Exception { get; set; }
	}
}
