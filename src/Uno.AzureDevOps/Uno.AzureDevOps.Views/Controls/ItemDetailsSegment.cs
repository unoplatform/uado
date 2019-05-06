using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Uno.AzureDevOps.Views.Controls
{
	public partial class ItemDetailsSegment : ContentControl
	{
		public static readonly DependencyProperty MainLabelProperty =
			DependencyProperty.Register("MainLabel", typeof(object), typeof(ItemDetailsSegment), new PropertyMetadata(null));

		public static readonly DependencyProperty TextContentProperty =
			DependencyProperty.Register("TextContent", typeof(string), typeof(ItemDetailsSegment), new PropertyMetadata(null));

		public static readonly DependencyProperty EmptyContentStringProperty =
			DependencyProperty.Register("EmptyContentString", typeof(object), typeof(ItemDetailsSegment), new PropertyMetadata(null));

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(object), typeof(ItemDetailsSegment), new PropertyMetadata(null));

		public object MainLabel
		{
			get { return (object)GetValue(MainLabelProperty); }
			set { SetValue(MainLabelProperty, value); }
		}

		public string TextContent
		{
			get { return (string)GetValue(TextContentProperty); }
			set { SetValue(TextContentProperty, value); }
		}

		public object EmptyContentString
		{
			get { return (object)GetValue(EmptyContentStringProperty); }
			set { SetValue(EmptyContentStringProperty, value); }
		}

		public object Command
		{
			get { return (object)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
	}
}
