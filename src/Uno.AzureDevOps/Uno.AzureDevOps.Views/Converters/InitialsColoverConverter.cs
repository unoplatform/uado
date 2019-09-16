using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Uno.AzureDevOps.Views.Converters
{
	public class InitialsColoverConverter : IValueConverter
	{
		private static readonly Dictionary<string, string> _colorMappings = new Dictionary<string, string>()
		{
			{ "A", "#FF16A085" },
			{ "B", "#FF341F97" },
			{ "C", "#FF2980B9" },
			{ "D", "#FFC0392B" },
			{ "E", "#FF2C3E50" },
			{ "F", "#FFD35400" },
			{ "G", "#FF27AE60" },
			{ "H", "#FFC56CF0" },
			{ "I", "#FF6C5CE7" },
			{ "J", "#FFE84393" },
			{ "K", "#FF636E72" },
			{ "L", "#FF27AE60" },
			{ "M", "#FF5F27CD" },
			{ "N", "#FF01A3A4" },
			{ "O", "#FFEE5253" },
			{ "P", "#FF6F1E51" },
			{ "Q", "#FFCC8E35" },
			{ "R", "#FF833471" },
			{ "S", "#FF1B1464" },
			{ "T", "#FF006266" },
			{ "U", "#FF1289A7" },
			{ "V", "#FFEA2027" },
			{ "W", "#FF40407A" },
			{ "X", "#FF227093" },
			{ "Y", "#FFB33939" },
			{ "Z", "#FF34495E" },
		};

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var baseColor = Colors.DarkGray;

			if (value != null && value is string str && !string.IsNullOrEmpty(str))
			{
				var overlayColor = Colors.Transparent;
				var first = str.Substring(0, 1).ToUpperInvariant();

				if (_colorMappings.TryGetValue(first, out var hex1))
				{
					baseColor = GetColorFromHex(hex1);
				}

				if (str.Length == 1)
				{
					return new SolidColorBrush(baseColor);
				}

				var second = str.Substring(1, 1);

				if (_colorMappings.TryGetValue(second, out var hex2))
				{
					overlayColor = GetColorFromHex(hex2);
				}

				return new SolidColorBrush(Blend(overlayColor, baseColor, 0.5));
			}
			else
			{
				return new SolidColorBrush(baseColor);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}

		private Color GetColorFromHex(string hex)
		{
			hex = hex.Replace("#", string.Empty);

			var a = (byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16);
			var r = (byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16);
			var g = (byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16);
			var b = (byte)System.Convert.ToUInt32(hex.Substring(6, 2), 16);

			return Color.FromArgb(a, r, g, b);
		}

		/// <summary>Blends the specified colors together.</summary>
		/// <param name="color">Color to blend onto the background color.</param>
		/// <param name="backColor">Color to blend the other color onto.</param>
		/// <param name="amount">How much of <paramref name="color"/> to keep,
		/// “on top of” <paramref name="backColor"/>.</param>
		/// <returns>The blended colors.</returns>
		private Color Blend(Color color, Color backColor, double amount)
		{
			var r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
			var g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
			var b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));

			return Color.FromArgb(255, r, g, b);
		}
	}
}
