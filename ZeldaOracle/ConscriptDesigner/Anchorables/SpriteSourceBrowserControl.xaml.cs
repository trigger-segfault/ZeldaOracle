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
using ConscriptDesigner.Util;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteSourceBrowserControl.xaml
	/// </summary>
	public partial class SpriteSourceBrowserControl : UserControl {

		private SpriteSourcePreview spriteSourcePreview;

		private bool suppressEvents;

		private string sourceName;

		private List<KeyValuePair<string, ISpriteSource>> sources;

		public SpriteSourceBrowserControl() {
			InitializeComponent(); this.suppressEvents = true;
			InitializeComponent();

			this.spriteSourcePreview = new SpriteSourcePreview();
			this.spriteSourcePreview.HoverSpriteChanged += OnHoverSpriteChanged;
			this.host.Child = this.spriteSourcePreview;
			this.suppressEvents = false;
			this.sources = new List<KeyValuePair<string, ISpriteSource>>();
			this.sourceName = "";

			OnHoverSpriteChanged();
		}

		private void OnHoverSpriteChanged(object sender = null, EventArgs e = null) {
			SpriteInfo hoverSprite = spriteSourcePreview.HoverSprite;
			if (hoverSprite == null) {
				statusSpriteInfo.Content = "";
			}
			else {
				string info = "Type: " + hoverSprite.Sprite.GetType().Name.Replace("Sprite", "");
				if (hoverSprite.HasSubstrips)
					info += ", Substrip: " + hoverSprite.SubstripIndex;
				statusSpriteInfo.Content = info;
			}
		}

		public void Dispose() {
			spriteSourcePreview.Dispose();
		}

		public void RefreshList() {
			suppressEvents = true;
			sources.Clear();
			comboBoxSpriteSources.Items.Clear();
			foreach (var pair in ZeldaResources.GetResourceDictionary<ISpriteSource>()) {
				sources.Add(pair);
			}
			sources.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));
			foreach (var pair in sources) {
				comboBoxSpriteSources.Items.Add(pair.Key);
			}
			var source = ZeldaResources.GetResource<ISpriteSource>(sourceName);
			if (sources.Any()) {
				if (source == null) {
					sourceName = sources[0].Key;
					source = sources[0].Value;
				}
				comboBoxSpriteSources.SelectedItem = sourceName;
				spriteSourcePreview.UpdateSpriteSource(sourceName, source);
			}
			suppressEvents = false;
		}

		public void ClearList() {
			sources.Clear();
			spriteSourcePreview.ClearSpriteSource();
		}

		private void OnToggleAnimations(object sender, RoutedEventArgs e) {
			spriteSourcePreview.Animating = !spriteSourcePreview.Animating;
			buttonRestartAnimations.IsEnabled = spriteSourcePreview.Animating;
		}

		private void OnRestartAnimations(object sender, RoutedEventArgs e) {
			spriteSourcePreview.RestartAnimations();
		}

		private void OnSpriteSourceChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxSpriteSources.SelectedIndex != -1) {
				sourceName = (string) comboBoxSpriteSources.SelectedItem;
				var source = ZeldaResources.GetResource<ISpriteSource>(sourceName);
				spriteSourcePreview.UpdateSpriteSource(sourceName, source);
			}
		}
	}
}
