using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZeldaEditor.Control;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game;
using ZeldaColor = ZeldaOracle.Common.Graphics.Color;

namespace ZeldaEditor.Controls {

	public class FormatCodeEventArgs : RoutedEventArgs {
		public string FormatCode { get; private set; }
		public FormatCodeEventArgs(RoutedEvent routedEvent, string formatCode) : base(routedEvent) {
			FormatCode = formatCode;
		}
	}

	public delegate void FormatCodeEventHandler(object sender, FormatCodeEventArgs e);
	
	public class FormatCodesDropdown : DropDownButton {

		public static readonly RoutedEvent FormatCodeSelectedEvent = EventManager.RegisterRoutedEvent(
			"FormatCodeSelected", RoutingStrategy.Bubble, typeof(FormatCodeEventHandler), typeof(FormatCodesDropdown));

		public event FormatCodeEventHandler FormatCodeSelected {
			add { AddHandler(FormatCodeSelectedEvent, value); }
			remove { RemoveHandler(FormatCodeSelectedEvent, value); }
		}

		public static readonly DependencyProperty IsColorCodesProperty = DependencyProperty.Register(
			"IsColorCodes", typeof(bool), typeof(FormatCodesDropdown),
			new FrameworkPropertyMetadata(
				false, OnIsColorCodesChanged));

		private static void OnIsColorCodesChanged(object sender, DependencyPropertyChangedEventArgs e) {
			FormatCodesDropdown element = sender as FormatCodesDropdown;
			if (element != null && element.templateApplied) {
				element.InitDropDownButton();
			}
		}

		public bool IsColorCodes {
			get { return (bool)GetValue(IsColorCodesProperty); }
			set { SetValue(IsColorCodesProperty, value); }
		}

		/*private static readonly string[] FormatCodesOrder = new string[] {
			"triangle", "square", "circle", "heart", "diamond", "club", "spade", "rupee",
			"up", "down", "right", "left", "up-tri", "down-tri", "right-tri", "left-tri",
			"male", "female", "music", "music-beam", "!!", "pilcrow", "section", "house",
			"1", "2", "3", "cursor", "invalid"
		};*/
		private static Style ToolBarButtonStyle;
		private static readonly Point2I ThinCharSize = new Point2I(16, 24);
		private static readonly Point2I ThinButtonSize = ThinCharSize + new Point2I(6, 4);
		private static readonly Point2I WideCharSize = new Point2I(25, 24);
		private static readonly Point2I WideButtonSize = WideCharSize + new Point2I(8, 4);

		static FormatCodesDropdown() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FormatCodesDropdown),
					new FrameworkPropertyMetadata(typeof(FormatCodesDropdown)));
		}
		private bool templateApplied;
		private DropDownMenuItem menuItem;

		public FormatCodesDropdown() {
			if (ToolBarButtonStyle == null) {
				ToolBarButtonStyle = (Style)FindResource(ToolBar.ButtonStyleKey);
			}
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				DropDownMenuItem menuItem = Items[0] as DropDownMenuItem;
				WrapPanel part_wrapPanel = menuItem.GetPopupTemplateChild("PART_WrapPanel") as WrapPanel;
				part_wrapPanel.Children.Clear();
				if (IsColorCodes) {
					//part_wrapPanel.Width = ThinButtonSize.X * 8;
					part_wrapPanel.Width = 65;
					part_wrapPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
					foreach (var pair in FormatCodes.GetColorCodes()) {
						string code = "<" + pair.Key + ">";
						string name = pair.Key;
						name = "" + char.ToUpper(pair.Key[0]) + pair.Key.Substring(1);
						Button button = new Button();
						button.Width = part_wrapPanel.Width;
						button.Content = name;
						button.ToolTip = code;
						button.Tag = code;
						button.Style = ToolBarButtonStyle;
						button.HorizontalAlignment = HorizontalAlignment.Stretch;
						ZeldaColor color = pair.Value.GetUnmappedColor(GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
						button.Foreground = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
						button.Click += OnFormatCodeSelected;
						part_wrapPanel.Children.Add(button);
					}
				}
				else {
					part_wrapPanel.Width = ThinButtonSize.X * 8;
					int index = 0;
					Point2I buttonSize = WideButtonSize;
					foreach (var pair in EditorImages.StringCodeImages) {
						string code = "<" + pair.Key + ">";
						ImageButton button = new ImageButton();
						button.Width = buttonSize.X;
						button.Height = buttonSize.Y;
						button.Padding = new Thickness(0);
						button.Source = pair.Value;
						button.ToolTip = code;
						button.Tag = code;
						button.Click += OnFormatCodeSelected;
						part_wrapPanel.Children.Add(button);
						if (index + 1 == 4) {
							// Skip two characters and change the button size
							index += 2;
							buttonSize = ThinButtonSize;
						}
						index++;
					}
				}
			}
		}
		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			menuItem = new DropDownMenuItem();
			InitDropDownButton();
			Items.Add(menuItem);
			menuItem.Items.Add(new DropDownMenuItem());
			templateApplied = true;
		}

		private void InitDropDownButton() {
			Image icon = new Image();
			icon.Stretch = Stretch.None;
			if (IsColorCodes) {
				icon.Source = EditorImages.ColorCodes;
				menuItem.Icon = icon;
				menuItem.Header = "Colors";
				menuItem.ToolTip = "Insert color format codes";
			}
			else {
				icon.Source = EditorImages.StringCodes;
				menuItem.Icon = icon;
				menuItem.Header = "Characters";
				menuItem.ToolTip = "Insert string format codes";
			}
		}

		private void OnFormatCodeSelected(object sender, RoutedEventArgs e) {
			string formatCode = (sender as Button).Tag as string;
			menuItem.Close();
			RaiseEvent(new FormatCodeEventArgs(FormatCodeSelectedEvent, formatCode));
		}
	}
}
