using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public partial class FromEmptyStringToVisibilityConverter : IValueConverter
	{
		public FromEmptyStringToVisibilityConverterMode Mode { get; set; } = FromEmptyStringToVisibilityConverterMode.EmptyMeansCollapsed;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			bool inverse = Mode == FromEmptyStringToVisibilityConverterMode.EmptyMeansVisible;

			var str = value as string;

			if (str == null && value != null)
			{
				throw new ArgumentException($"Value needs to be a string or null. Got {value} ({value.GetType().FullName})");
			}

			if (string.IsNullOrWhiteSpace(str))
			{
				return inverse ? Visibility.Visible : Visibility.Collapsed;
			}
			else
			{
				return inverse ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
