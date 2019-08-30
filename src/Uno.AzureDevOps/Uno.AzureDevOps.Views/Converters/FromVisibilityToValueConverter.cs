using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class FromVisibilityToValueConverter : IValueConverter
	{
		public object VisibleValue { get; set; }

		public object CollapsedValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
			=> value is Visibility visibility && visibility == Visibility.Visible
				? VisibleValue
				: CollapsedValue;

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotSupportedException();
	}
}
