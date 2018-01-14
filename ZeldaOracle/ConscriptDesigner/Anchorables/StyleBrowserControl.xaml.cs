using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConscriptDesigner.Control;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Geometry;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteBrowserControl.xaml
	/// </summary>
	public partial class StyleBrowserControl : UserControl {

		private StylePreview stylePreview;

		private bool suppressEvents;

		public StyleBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.stylePreview = new StylePreview();
			this.stylePreview.Refreshed += OnStylePreviewRefreshed;
			this.stylePreview.HoverSpriteChanged += OnHoverSpriteChanged;
			this.host.Child = this.stylePreview;
			this.suppressEvents = false;

			DesignerControl.PreviewZoneChanged += OnPreviewZoneChanged;

			OnHoverSpriteChanged();
		}

		private void OnHoverSpriteChanged(object sender = null, EventArgs e = null) {
			SpriteInfo hoverSprite = stylePreview.HoverSprite;
			if (hoverSprite == null) {
				statusSpriteStyle.Content = "";
				statusSpriteInfo.Content = "";
			}
			else {
				string info = "Type: " + hoverSprite.Sprite.GetType().Name.Replace("Sprite", "");
				if (hoverSprite.HasSubstrips)
					info += ", Substrip: " + hoverSprite.SubstripIndex;
				statusSpriteInfo.Content = info;
				statusSpriteStyle.Content = hoverSprite.Name;
			}
		}

		public void Dispose() {
			DesignerControl.PreviewZoneChanged -= OnPreviewZoneChanged;
			stylePreview.Dispose();
		}

		public void RefreshList() {
			stylePreview.RefreshList();
		}

		public void ClearList() {
			stylePreview.ClearList();
		}

		private void OnStylePreviewRefreshed(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxStyleGroups.Items.Clear();
			foreach (string styleGroup in stylePreview.GetStyleGroups()) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = styleGroup;
				item.Tag = styleGroup;
				comboBoxStyleGroups.Items.Add(item);
				if (styleGroup == stylePreview.StyleGroup) {
					comboBoxStyleGroups.SelectedItem = item;
				}
			}
			comboBoxZones.ItemsSource = DesignerControl.PreviewZones;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			stylePreview.UpdateFilter(textBoxSearch.Text);
		}

		private void OnStyleGroupChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxStyleGroups.SelectedIndex != -1) {
				string styleGroup = (string) ((ComboBoxItem) comboBoxStyleGroups.SelectedItem).Tag;
				stylePreview.UpdateStyleGroup(styleGroup);
			}
		}

		private void OnToggleAnimations(object sender, RoutedEventArgs e) {
			stylePreview.Animating = !stylePreview.Animating;
			buttonRestartAnimations.IsEnabled = stylePreview.Animating;
		}

		private void OnRestartAnimations(object sender, RoutedEventArgs e) {
			stylePreview.RestartAnimations();
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			DesignerControl.PreviewZoneID = (string) comboBoxZones.SelectedItem;
		}

		private void OnPreviewZoneChanged(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
			stylePreview.Invalidate();
		}
	}
}
