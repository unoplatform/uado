﻿using Uno.AzureDevOps.Presentation;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Uno.AzureDevOps.Views.Content
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			InitializeComponent();
			DataContext = new LoginPageViewModel();
		}
	}
}