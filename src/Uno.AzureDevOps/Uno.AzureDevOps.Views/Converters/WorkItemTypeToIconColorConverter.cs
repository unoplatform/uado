using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class WorkItemTypeToIconColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			switch (value)
			{
				case "Task":
					return "#F2CB1D";
				case "Bug":
					return "#CC293D";
				case "Feature":
					return "#773B93";
				case "Test Case":
					return "#1C4B50";
				case "Impediment":
					return "#B40F9E";
				case "Product Backlog Item":
				default:
					return "#489DCC";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
