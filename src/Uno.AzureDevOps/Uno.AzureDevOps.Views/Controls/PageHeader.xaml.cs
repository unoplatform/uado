using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Framework.Navigation;
using Windows.UI.Xaml;

namespace Uno.AzureDevOps.Views.Controls
{
	public enum HeaderMode
	{
		Minimal,
		Large
	}

	public sealed partial class PageHeader : UadoUserControl
	{
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register(
				nameof(Mode),
				typeof(HeaderMode),
				typeof(PageHeader),
				new PropertyMetadata(HeaderMode.Large));

		public static readonly DependencyProperty ProjectInitialsVisibilityProperty =
			DependencyProperty.Register(
				nameof(ProjectInitialsVisibility),
				typeof(Visibility),
				typeof(PageHeader),
				new PropertyMetadata(Visibility.Collapsed));

		public static readonly DependencyProperty TeamsVisibilityProperty =
			DependencyProperty.Register(
				nameof(TeamsVisibility),
				typeof(Visibility),
				typeof(PageHeader),
				new PropertyMetadata(Visibility.Collapsed));

		public static readonly DependencyProperty HamburgerMenuVisibilityProperty =
			DependencyProperty.Register(
				nameof(HamburgerMenuVisibility),
				typeof(Visibility),
				typeof(PageHeader),
				new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register(
				nameof(Title),
				typeof(string),
				typeof(PageHeader),
				new PropertyMetadata(string.Empty));

		public static readonly DependencyProperty BackCaptionProperty =
			DependencyProperty.Register(
				nameof(BackCaption),
				typeof(string),
				typeof(PageHeader),
				new PropertyMetadata(string.Empty));

		public static readonly DependencyProperty BackCommandProperty =
			DependencyProperty.Register(
				nameof(BackCommand),
				typeof(RelayCommand),
				typeof(PageHeader),
				new PropertyMetadata(null));

		public PageHeader()
		{
			InitializeComponent();
		}

		public event RoutedEventHandler HamburgerClick;

		public HeaderMode Mode
		{
			get => (HeaderMode)GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public Visibility ProjectInitialsVisibility
		{
			get => (Visibility)GetValue(ProjectInitialsVisibilityProperty);
			set => SetValue(ProjectInitialsVisibilityProperty, value);
		}

		public Visibility TeamsVisibility
		{
			get => (Visibility)GetValue(TeamsVisibilityProperty);
			set => SetValue(TeamsVisibilityProperty, value);
		}

		public Visibility HamburgerMenuVisibility
		{
			get => (Visibility)GetValue(HamburgerMenuVisibilityProperty);
			set => SetValue(HamburgerMenuVisibilityProperty, value);
		}

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public string BackCaption
		{
			get => (string)GetValue(BackCaptionProperty);
			set => SetValue(BackCaptionProperty, value);
		}

		public RelayCommand BackCommand
		{
			get => (RelayCommand)GetValue(BackCommandProperty);
			set => SetValue(BackCommandProperty, value);
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			HamburgerClick?.Invoke(sender, e);
		}
	}
}
