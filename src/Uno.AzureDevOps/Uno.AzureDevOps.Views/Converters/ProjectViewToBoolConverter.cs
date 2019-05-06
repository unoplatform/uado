using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class ProjectViewToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null || parameter == null)
			{
				return false;
			}

			string checkValue = value.ToString();
			string targetValue = parameter.ToString();

			return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value == null || parameter == null)
			{
				return null;
			}

			if ((bool)value)
			{
				return parameter;
			}

			return null;
		}
	}
}
