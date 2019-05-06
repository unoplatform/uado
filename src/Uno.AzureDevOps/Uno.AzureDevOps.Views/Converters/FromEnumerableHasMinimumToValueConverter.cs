using System;
using System.Collections;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class FromEnumerableHasMinimumToValueConverter : IValueConverter
	{
		public int Minimum { get; set; } = 1;

		public object EnumerableHasMinimumValue { get; set; }

		public object EnumerableDefaultValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var enumerableValue = value as IEnumerable;

			if (value != null && enumerableValue == null)
			{
				throw new ArgumentException($"Converter value (of type {value.GetType().FullName}) needs to be an IEnumerable.");
			}

			var meetsMinimum = (enumerableValue?.Cast<object>().Count() ?? 0) >= Minimum;

			return meetsMinimum
				? EnumerableHasMinimumValue
				: EnumerableDefaultValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
