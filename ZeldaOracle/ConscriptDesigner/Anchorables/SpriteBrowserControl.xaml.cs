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
using ConscriptDesigner.Util;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteBrowserControl.xaml
	/// </summary>
	public partial class SpriteBrowserControl : UserControl {

		private SpritePreview spritePreview;

		private bool suppressEvents;

		public SpriteBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.spritePreview = new SpritePreview();
			this.spritePreview.Refreshed += OnSpritePreviewRefreshed;
			this.spritePreview.HoverSpriteChanged += OnHoverSpriteChanged;
			this.host.Child = this.spritePreview;
			this.suppressEvents = false;

			DesignerControl.PreviewZoneChanged += OnPreviewZoneChanged;

			OnHoverSpriteChanged();
		}

		private void OnHoverSpriteChanged(object sender = null, EventArgs e = null) {
			SpriteInfo hoverSprite = spritePreview.HoverSprite;
			if (hoverSprite == null) {
				textBlockSpriteName.Text = "";
				statusSpriteInfo.Content = "";
			}
			else {
				string info = "Type: " + hoverSprite.Sprite.GetType().Name.Replace("Sprite", "");
				if (hoverSprite.HasSubstrips)
					info += ", Substrip: " + hoverSprite.SubstripIndex;
				statusSpriteInfo.Content = info;
				textBlockSpriteName.Text = hoverSprite.Name;
			}
		}

		public void Dispose() {
			DesignerControl.PreviewZoneChanged -= OnPreviewZoneChanged;
			spritePreview.Dispose();
		}

		public void RefreshList() {
			spritePreview.RefreshList();
		}

		public void ClearList() {
			spritePreview.ClearList();
		}

		private void OnSpritePreviewRefreshed(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxSpriteSizes.Items.Clear();
			foreach (Point2I point in spritePreview.GetSpriteSizes()) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = point.X + " x " + point.Y;
				item.Tag = point;
				comboBoxSpriteSizes.Items.Add(item);
				if (point == spritePreview.SpriteSize) {
					comboBoxSpriteSizes.SelectedItem = item;
				}
			}
			comboBoxZones.ItemsSource = DesignerControl.PreviewZones;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			spritePreview.UpdateFilter(textBoxSearch.Text);
		}

		private void OnSpriteSizeChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxSpriteSizes.SelectedIndex != -1) {
				Point2I size = (Point2I) ((ComboBoxItem) comboBoxSpriteSizes.SelectedItem).Tag;
				spritePreview.UpdateSpriteSize(size);
			}
		}

		private void OnToggleAnimations(object sender, RoutedEventArgs e) {
			spritePreview.Animating = !spritePreview.Animating;
			buttonRestartAnimations.IsEnabled = spritePreview.Animating;
		}

		private void OnRestartAnimations(object sender, RoutedEventArgs e) {
			spritePreview.RestartAnimations();
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			DesignerControl.PreviewZoneID = (string) comboBoxZones.SelectedItem;
		}

		private void OnPreviewZoneChanged(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
			spritePreview.Invalidate();
		}
	}
}
