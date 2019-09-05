using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.AzureDevOps.Client;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class InitialsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var split = (value as string)?.Split(' ');
			var first = split?.FirstOrDefault()?.Substring(0, 1);
			var last = (split?.Length ?? 0) > 1
				? split?.LastOrDefault()?.Substring(0, 1)
				: string.Empty;

			return first?.ToUpperInvariant() + last?.ToUpperInvariant() ?? string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
