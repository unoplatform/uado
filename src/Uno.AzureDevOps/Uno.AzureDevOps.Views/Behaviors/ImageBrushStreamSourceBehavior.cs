using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Uno.AzureDevOps.Views.Behaviors
{
	[SuppressMessage("", "SA1201", Justification = "Behavior")]
	[SuppressMessage("", "CA1720", Justification = "Behavior")]
	public static class ImageBrushStreamSourceBehavior
	{
		private static HttpClient _client = new HttpClient();

		public static object GetSource(ImageBrush obj)
		{
			return obj.GetValue(SourceProperty);
		}

		public static void SetSource(ImageBrush obj, object value)
		{
			obj.SetValue(SourceProperty, value);
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source",
			typeof(object),
			typeof(ImageBrushStreamSourceBehavior),
			new PropertyMetadata(null, (d, e) => OnSourceChanged((ImageBrush)d, e.NewValue)));

		public static string GetAuthorizationToken(ImageBrush obj)
		{
			return (string)obj.GetValue(AuthorizationTokenProperty);
		}

		public static void SetAuthorizationToken(ImageBrush obj, string value)
		{
			obj.SetValue(AuthorizationTokenProperty, value);
		}

		public static readonly DependencyProperty AuthorizationTokenProperty = DependencyProperty.RegisterAttached(
			"AuthorizationToken",
			typeof(string),
			typeof(ImageBrushStreamSourceBehavior),
			new PropertyMetadata(null));

		private static async void OnSourceChanged(ImageBrush imageBrush, object newSource)
		{
			var bitmapImage = default(BitmapImage);

			if (newSource is Stream sourceStream)
			{
				bitmapImage = await GetBitmapImage(sourceStream);
			}
			else if (newSource is string imageUrl && !string.IsNullOrWhiteSpace(imageUrl) && Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
			{
				_client.DefaultRequestHeaders.Clear();
				var authorization = GetAuthorizationToken(imageBrush);

				if (!string.IsNullOrWhiteSpace(authorization))
				{
					_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
				}

				var stream = await (await _client.GetAsync(new Uri(imageUrl))).Content.ReadAsStreamAsync();

				bitmapImage = await GetBitmapImage(stream);
			}

			imageBrush.ImageSource = bitmapImage;
		}

		private static async Task<BitmapImage> GetBitmapImage(Stream sourceStream)
		{
			var bitmapImage = new BitmapImage();

			sourceStream.Position = 0;

			try
			{
#if NETFX_CORE
				await bitmapImage.SetSourceAsync(sourceStream.AsRandomAccessStream()).AsTask();
#else
				await bitmapImage.SetSourceAsync(sourceStream);
#endif
			}
			catch
			{
			}

			return bitmapImage;
		}
	}
}
