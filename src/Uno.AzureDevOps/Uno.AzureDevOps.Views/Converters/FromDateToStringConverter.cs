using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class FromDateToStringConverter : IValueConverter
	{
		public string FormatDate { get; set; }

		// This converts the DateTime object to the string to display.
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// Retrieve the format string and use it to format the value.
			var formatString = FormatDate;

			if (!string.IsNullOrEmpty(formatString))
			{
				return string.Format(CultureInfo.InvariantCulture, formatString, value);
			}

			// If the format string is null or empty, simply call ToString()
			// on the value.
			return value.ToString();
		}

		// No need to implement converting back on a one-way binding
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
