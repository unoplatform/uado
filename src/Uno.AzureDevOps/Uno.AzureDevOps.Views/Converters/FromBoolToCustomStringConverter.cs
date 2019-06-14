using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	/// <summary>
	/// Until v2, this converter is handy to avoid code duplication in error handling
	/// </summary>
	public class FromBoolToCustomStringConverter : IValueConverter
	{
		public string ErrorNoInternetConnection { get; set; }

		public string ErrorInternal { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value != null)
			{
				var isInternetFaulted = (bool)value;

				return isInternetFaulted ? ErrorNoInternetConnection : ErrorInternal;
			}

			return ErrorInternal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
	}
}
