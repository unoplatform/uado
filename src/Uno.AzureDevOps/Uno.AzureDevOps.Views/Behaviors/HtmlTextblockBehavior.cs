using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using HtmlAgilityPack;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Uno.AzureDevOps.Views.Behaviors
{
	[SuppressMessage("", "SA1201", Justification = "Behavior")]
	[SuppressMessage("", "CA1720", Justification = "Behavior")]
	public static class HtmlTextBlockBehavior
	{
		private const string _bulletCharacter = "\u2022";

		private static bool _isBold = false;
		private static bool _isItalic = false;
		private static bool _isUnderline = false;
		private static bool _isHyperLink = false;
		private static bool _isPhone = false;
		private static bool _isEmail = false;
		private static bool _isHeader = false;
		private static bool _isList = false;
		private static bool _previousHasLinebreak = false;
		private static string _hyperlink = string.Empty;
		private static string _phone = string.Empty;
		private static string _email = string.Empty;

		public static bool GetDisableLinks(DependencyObject obj)
		{
			return (bool)obj.GetValue(DisableLinksProperty);
		}

		public static void SetDisableLinks(DependencyObject obj, bool value)
		{
			obj.SetValue(DisableLinksProperty, value);
		}

		public static readonly DependencyProperty DisableLinksProperty =
			DependencyProperty.RegisterAttached(
				"DisableLinks",
				typeof(bool),
				typeof(HtmlTextBlockBehavior),
				new PropertyMetadata(false));

		public static Brush GetLinkColor(DependencyObject obj)
		{
			return (Brush)obj.GetValue(LinkColorProperty);
		}

		public static void SetLinkColor(DependencyObject obj, Brush value)
		{
			obj.SetValue(LinkColorProperty, value);
		}

		public static readonly DependencyProperty LinkColorProperty =
			DependencyProperty.RegisterAttached(
				"LinkColor",
				typeof(Brush),
				typeof(HtmlTextBlockBehavior),
				new PropertyMetadata(default(Brush)));

		public static ICommand GetOpenPhoneCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(OpenPhoneCommandProperty);
		}

		public static void SetOpenPhoneCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(OpenPhoneCommandProperty, value);
		}

		public static readonly DependencyProperty OpenPhoneCommandProperty =
			DependencyProperty.RegisterAttached(
				"OpenPhoneCommand",
				typeof(ICommand),
				typeof(HtmlTextBlockBehavior),
				new PropertyMetadata(null));

		public static ICommand GetOpenEmailCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(OpenEmailCommandProperty);
		}

		public static void SetOpenEmailCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(OpenEmailCommandProperty, value);
		}

		public static readonly DependencyProperty OpenEmailCommandProperty =
			DependencyProperty.RegisterAttached(
				"OpenEmailCommand",
				typeof(ICommand),
				typeof(HtmlTextBlockBehavior),
				new PropertyMetadata(null));

		public static FontFamily GetRegularFontFamily(DependencyObject obj)
		{
			return (FontFamily)obj.GetValue(RegularFontFamilyProperty);
		}

		public static void SetRegularFontFamily(DependencyObject obj, FontFamily value)
		{
			obj.SetValue(RegularFontFamilyProperty, value);
		}

		public static readonly DependencyProperty RegularFontFamilyProperty =
			DependencyProperty.RegisterAttached("RegularFontFamily", typeof(FontFamily), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(FontFamily)));

		public static int GetHeaderFontSize(DependencyObject obj)
		{
			return (int)obj.GetValue(HeaderFontSizeProperty);
		}

		public static void SetHeaderFontSize(DependencyObject obj, int value)
		{
			obj.SetValue(HeaderFontSizeProperty, value);
		}

		public static readonly DependencyProperty HeaderFontSizeProperty =
			DependencyProperty.RegisterAttached("HeaderFontSize", typeof(int), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(int)));

		public static FontFamily GetBoldFontFamily(DependencyObject obj)
		{
			return (FontFamily)obj.GetValue(BoldFontFamilyProperty);
		}

		public static void SetBoldFontFamily(DependencyObject obj, FontFamily value)
		{
			obj.SetValue(BoldFontFamilyProperty, value);
		}

		public static readonly DependencyProperty BoldFontFamilyProperty =
			DependencyProperty.RegisterAttached("BoldFontFamily", typeof(FontFamily), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(FontFamily)));

		public static FontFamily GetSemiBoldFontFamily(DependencyObject obj)
		{
			return (FontFamily)obj.GetValue(SemiBoldFontFamilyProperty);
		}

		public static void SetSemiBoldFontFamily(DependencyObject obj, FontFamily value)
		{
			obj.SetValue(SemiBoldFontFamilyProperty, value);
		}

		public static readonly DependencyProperty SemiBoldFontFamilyProperty =
			DependencyProperty.RegisterAttached("SemiBoldFontFamily", typeof(FontFamily), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(FontFamily)));

		public static FontFamily GetItalicFontFamily(DependencyObject obj)
		{
			return (FontFamily)obj.GetValue(ItalicFontFamilyProperty);
		}

		public static void SetItalicFontFamily(DependencyObject obj, FontFamily value)
		{
			obj.SetValue(ItalicFontFamilyProperty, value);
		}

		public static readonly DependencyProperty ItalicFontFamilyProperty =
			DependencyProperty.RegisterAttached("ItalicFontFamily", typeof(FontFamily), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(FontFamily)));

		public static FontFamily GetBoldItalicFontFamily(DependencyObject obj)
		{
			return (FontFamily)obj.GetValue(BoldItalicFontFamilyProperty);
		}

		public static void SetBoldItalicFontFamily(DependencyObject obj, FontFamily value)
		{
			obj.SetValue(BoldItalicFontFamilyProperty, value);
		}

		public static readonly DependencyProperty BoldItalicFontFamilyProperty =
			DependencyProperty.RegisterAttached("BoldItalicFontFamily", typeof(FontFamily), typeof(HtmlTextBlockBehavior), new PropertyMetadata(default(FontFamily)));

		public static string GetHtmlText(DependencyObject obj)
		{
			return (string)obj.GetValue(HtmlTextProperty);
		}

		public static void SetHtmlText(DependencyObject obj, string value)
		{
			obj.SetValue(HtmlTextProperty, value);
		}

		public static readonly DependencyProperty HtmlTextProperty =
			DependencyProperty.RegisterAttached(
				"HtmlText",
				typeof(string),
				typeof(HtmlTextBlockBehavior),
				new PropertyMetadata(null, OnHtmlTextChanged));

		private static void OnHtmlTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textblock = d as TextBlock;
			textblock.Inlines.Clear();

			if (e.NewValue is string text)
			{
				text = text.Replace("\n", string.Empty);
				text = text.Replace("<br>", "\n");
				text = text.Replace("&nbsp;", " ");

				while (text.Length > 0)
				{
					text = ApplyStyle(textblock, text, d);
				}
			}
		}

		private static string ApplyStyle(TextBlock textblock, string text, DependencyObject sender)
		{
			var startOfTag = text.IndexOf("<");

			if (startOfTag > 0)
			{
				AddInline(textblock, text.Substring(0, startOfTag), sender);
				text = text.Substring(startOfTag);
			}

			if (startOfTag == -1)
			{
				AddInline(textblock, text, sender);
				return string.Empty;
			}

			return ParseHtmlTag(textblock, text, sender);
		}

		private static string ParseHtmlTag(TextBlock textblock, string text, DependencyObject sender)
		{
			var linebreak = _previousHasLinebreak;
			_previousHasLinebreak = false;

			if (text.StartsWith("<b>"))
			{
				_isBold = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("<h3>"))
			{
				_isHeader = true;
				_isBold = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("<a"))
			{
				var end = text.IndexOf(">");

				if (!GetDisableLinks(sender))
				{
					var aTag = text.Substring(0, end + 1);
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(aTag);
					var node = htmlDoc.DocumentNode.SelectSingleNode("//a");
					var link = node?.Attributes["href"]?.Value;
					if (link.StartsWith("tel:"))
					{
						link = link.Substring(4);
						_phone = link;
						_isPhone = true;
					}
					else if (link.StartsWith("mailto:"))
					{
						link = link.Substring(7);
						_email = link;
						_isEmail = true;
					}
					else
					{
						_hyperlink = link;
						_isHyperLink = true;
					}
				}

				return text.Substring(end + 1);
			}

			if (text.StartsWith("</a>"))
			{
				_isHyperLink = false;
				_isPhone = false;
				_phone = string.Empty;
				_hyperlink = string.Empty;
				_isEmail = false;
				_email = string.Empty;
				return text.Substring(4);
			}

			if (text.StartsWith("<strong>"))
			{
				_isBold = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("</b>"))
			{
				_isBold = false;
				return text.Substring(4);
			}

			if (text.StartsWith("</h3>"))
			{
				_isHeader = false;
				_isBold = false;

				var remaining = text.Substring(5);
				if (remaining.Length > 0)
				{
					textblock.Inlines.Add(new LineBreak());
				}

				return remaining;
			}

			if (text.StartsWith("</strong>"))
			{
				_isBold = false;
				return text.Substring(9);
			}

			if (text.StartsWith("<i>"))
			{
				_isItalic = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("<em>"))
			{
				_isItalic = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("</i>"))
			{
				_isItalic = false;
				return text.Substring(4);
			}

			if (text.StartsWith("</em>"))
			{
				_isItalic = false;
				return text.Substring(5);
			}

			if (text.StartsWith("<u>"))
			{
				_isUnderline = true;
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("</u>"))
			{
				_isUnderline = false;
				return text.Substring(4);
			}

			if (text.StartsWith("<p>"))
			{
				var end = text.IndexOf(">");
				return text.Substring(end + 1);
			}

			if (text.StartsWith("</p>"))
			{
				var remaining = text.Substring(4);
				if (remaining.Length > 0)
				{
					if (!linebreak)
					{
						textblock.Inlines.Add(new LineBreak());
						_previousHasLinebreak = true;
					}

					textblock.Inlines.Add(new LineBreak());
				}

				return remaining;
			}

			if (text.StartsWith("<ul>"))
			{
				_isList = true;
				var end = text.IndexOf(">");

				return text.Substring(end + 1);
			}

			if (text.StartsWith("<li>"))
			{
				_isList = true;
				var end = text.IndexOf(">");

				return text.Substring(end + 1);
			}

			if (text.StartsWith("</li>"))
			{
				_isList = false;
				var remaining = text.Substring(5);

				if (remaining.Length > 0)
				{
					textblock.Inlines.Add(new LineBreak());
				}

				return remaining;
			}

			if (text.StartsWith("</ul>"))
			{
				_isList = false;

				var remaining = text.Substring(5);
				if (remaining.Length > 0)
				{
					textblock.Inlines.Add(new LineBreak());
					_previousHasLinebreak = true;
				}

				return remaining;
			}

			// Remove all unknown html tags
			if (text.StartsWith("<"))
			{
				var end = text.IndexOf(">");
				if (end == -1)
				{
					return string.Empty;
				}

				return text.Substring(end + 1);
			}

			AddInline(textblock, text.Substring(0, 1), sender);
			return text.Substring(1);
		}

		private static void AddInline(TextBlock textblock, string text, DependencyObject sender)
		{
			var run = default(Inline);

			if (_isList)
			{
				textblock.Inlines.Add(new Run()
				{
					Text = $"{_bulletCharacter} "
				});

				// Reset element to make sure there is no other bullet put in the middle of the list item.
				_isList = false;
			}

			var fontStyle = _isItalic ? FontStyle.Italic : FontStyle.Normal;
			var fontWeight = _isBold ? FontWeights.Bold : (FontWeight)textblock.GetValue(TextBlock.FontWeightProperty);
			var linkFontWeight = _isBold ? FontWeights.Bold : FontWeights.SemiBold; // App specific: link are semibold by default
			var fontFamily = GetFontFamily(textblock, fontWeight, fontStyle);
			var linkFontFamily = GetFontFamily(textblock, linkFontWeight, fontStyle);

			if (_isUnderline)
			{
				run = new Underline();
				((Underline)run).Inlines.Add(new Run()
				{
					Text = text,
					FontStyle = fontStyle,
					FontWeight = linkFontWeight,
					FontFamily = fontFamily
				});
			}
			else if (_isHyperLink)
			{
				try
				{
					run = new Hyperlink()
					{
						NavigateUri = new Uri(_hyperlink)
					};

					var innerRun = new Run()
					{
						Text = text,
						FontStyle = fontStyle,
						FontWeight = linkFontWeight,
						FontFamily = linkFontFamily
					};

					if (GetLinkColor(textblock) != null)
					{
						innerRun.Foreground = GetLinkColor(textblock);
					}

					((Hyperlink)run).Inlines.Add(innerRun);
				}
				catch
				{
					run = new Run()
					{
						Text = text,
						FontStyle = fontStyle,
						FontWeight = fontWeight,
						FontFamily = fontFamily
					};
				}
			}
			else if (_isPhone)
			{
				run = new Hyperlink() { UnderlineStyle = UnderlineStyle.None };

				var innerRun = new Run()
				{
					Text = text,
					FontStyle = fontStyle,
					FontWeight = linkFontWeight,
					FontFamily = linkFontFamily
				};

				if (GetLinkColor(textblock) != null)
				{
					innerRun.Foreground = GetLinkColor(textblock);
				}

				((Hyperlink)run).Inlines.Add(innerRun);

				var phone = _phone;
				((Hyperlink)run).Click += (s, e) => OpenPhone(sender, phone);
			}
			else if (_isEmail)
			{
				run = new Hyperlink() { UnderlineStyle = UnderlineStyle.None };

				var innerRun = new Run()
				{
					Text = text,
					FontStyle = fontStyle,
					FontWeight = linkFontWeight,
					FontFamily = linkFontFamily
				};

				if (GetLinkColor(textblock) != null)
				{
					innerRun.Foreground = GetLinkColor(textblock);
				}

				((Hyperlink)run).Inlines.Add(innerRun);

				var email = _email;
				((Hyperlink)run).Click += (s, e) => OpenEmail(sender, email);
			}
			else
			{
				run = new Run()
				{
					Text = text,
					FontStyle = fontStyle,
					FontWeight = fontWeight,
					FontFamily = fontFamily,
					FontSize = _isHeader ? GetHeaderFontSize(sender) : (double)textblock.GetValue(TextBlock.FontSizeProperty)
				};
			}

			textblock.Inlines.Add(run);
		}

		private static FontFamily GetFontFamily(TextBlock textblock, FontWeight fontWeight, FontStyle fontStyle)
		{
			FontFamily fontFamily = null;

			if (FontWeights.Bold.Equals(fontWeight) && FontStyle.Italic.Equals(fontStyle))
			{
				fontFamily = GetBoldItalicFontFamily(textblock);
			}
			else if (FontWeights.Bold.Equals(fontWeight))
			{
				fontFamily = GetBoldFontFamily(textblock);
			}
			else if (FontWeights.SemiBold.Equals(fontWeight))
			{
				fontFamily = GetSemiBoldFontFamily(textblock);
			}
			else if (FontStyle.Italic.Equals(fontStyle))
			{
				fontFamily = GetItalicFontFamily(textblock);
			}

			return fontFamily ?? textblock.GetValue(TextBlock.FontFamilyProperty) as FontFamily ?? GetRegularFontFamily(textblock);
		}

		private static void OpenPhone(DependencyObject sender, string phoneNumber)
		{
			GetOpenPhoneCommand(sender)?.Execute(phoneNumber);
		}

		private static void OpenEmail(DependencyObject sender, string email)
		{
			GetOpenEmailCommand(sender)?.Execute(email);
		}
	}
}
