using System;
using System.Collections;
using System.Linq;
using Windows.UI.Xaml.Data;

#if NETFX_CORE
using Windows.UI.Xaml;
#elif __ANDROID__ || __IOS__ || __WASM__
using Visibility = Windows.UI.Xaml.Visibility;
#else
using System.Windows;
#endif

namespace Uno.AzureDevOps.Views.Converters
{
	/// <summary>
	/// This converter outputs a visibility value based on the presence of any items in an enumerable
	///
	/// VisibilityOnEnumerableHasAny (VisibilityOnEnumerableHasAny) : The visibility that should be returned
	/// when the enumerable has items.
	///
	/// By default, VisibilityOnEnumerableHasAny is set to Visible.
	///
	/// This may be used to show or hide a list based on the presence or absence of data.
	/// </summary>
	public class FromEnumerableHasAnyToVisibilityConverter : IValueConverter
	{
		public FromEnumerableHasAnyToVisibilityConverter()
		{
			VisibilityOnEnumerableHasAny = Converters.VisibilityOnEnumerableHasAny.Visible;
			VisibilityOnEnumerableNull = Converters.VisibilityOnEnumerableNull.Visible;
		}

		public VisibilityOnEnumerableHasAny VisibilityOnEnumerableHasAny { get; set; }

		public VisibilityOnEnumerableNull VisibilityOnEnumerableNull { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			var inverse = VisibilityOnEnumerableHasAny == VisibilityOnEnumerableHasAny.Collapsed;

			var visibilityOnTrue = (!inverse) ? Visibility.Visible : Visibility.Collapsed;
			var visibilityOnFalse = (!inverse) ? Visibility.Collapsed : Visibility.Visible;

			var enumerableValue = value as IEnumerable;

			if (value != null && enumerableValue == null)
			{
				throw new ArgumentException($"Converter value (of type {value.GetType().FullName}) needs to be an IEnumerable.");
			}

			if (value == null)
			{
				return VisibilityOnEnumerableNull == VisibilityOnEnumerableNull.Collapsed
						? Visibility.Collapsed
						: Visibility.Visible;
			}

			var valueToConvert = enumerableValue?.Cast<object>().Any() ?? false;

			return valueToConvert ? visibilityOnTrue : visibilityOnFalse;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
