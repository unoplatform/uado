using System;
using System.Collections.Generic;
using System.Text;
using Uno.AzureDevOps.Views.Controls;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class FromHeaderModeToValueConverter : IValueConverter
	{
		public object MinimalValue { get; set; }

		public object LargeValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return MinimalValue;
			}

			switch ((HeaderMode)value)
			{
				case HeaderMode.Large:
					return LargeValue;

				default:
					return MinimalValue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
