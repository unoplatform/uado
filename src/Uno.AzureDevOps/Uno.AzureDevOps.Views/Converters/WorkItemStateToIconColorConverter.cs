using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Views.Converters
{
	public class WorkItemStateToIconColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			switch (value)
			{
				case "Blocked":
				case "Cannot Reproduce":
				case "Invalid":
					return "#e60017";
				case "Open":
				case "Committed Testable":
				case "Committed Tested":
					return "#5688e0";
				case "Committed":
				case "In Progress":
					return "#007acc";
				case "Duplicate":
				case "As Designed":
				case "Will not fix":
				case "To be reviewed":
					return "#ff9d00";
				case "Fix failed":
					return "#e87025";
				case "Done":
					return "#339933";
				default:
					return "#d5d5d5";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
