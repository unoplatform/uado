using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public bool IsVisibleIfTrue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((IsVisibleIfTrue && (value as bool?) == true) || (!IsVisibleIfTrue && (value as bool?) == false))
			{
				return Visibility.Visible;
			}

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (IsVisibleIfTrue && (value as Visibility?) == Visibility.Visible)
			{
				return true;
			}

			return false;
		}
	}
}
