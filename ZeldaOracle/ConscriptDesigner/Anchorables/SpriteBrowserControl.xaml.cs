using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ConscriptDesigner.Control;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteBrowserControl.xaml
	/// </summary>
	public partial class SpriteBrowserControl : UserControl {

		private SpritePreview preview;

		private bool suppressEvents;

		private Point2I spriteSize;
		private Dictionary<Point2I, List<SpriteInfo>> spriteSizes;
		private List<Point2I> orderedSpriteSizes;

		private List<SpriteInfo> filteredSprites;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sprite browser control.</summary>
		public SpriteBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.preview = new SpritePreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.spriteSizes = new Dictionary<Point2I, List<SpriteInfo>>();
			this.orderedSpriteSizes = new List<Point2I>();
			this.filteredSprites = new List<SpriteInfo>();

			DesignerControl.ResourcesLoaded += OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded += OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated += OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged += OnPreviewScaleChanged;
			
			this.suppressEvents = false;
		}
		

		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		public void Dispose() {
			DesignerControl.ResourcesLoaded -= OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded -= OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated -= OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged -= OnPreviewScaleChanged;
			preview.Dispose();
		}

		public void Reload() {
			suppressEvents = true;

			spriteSizes.Clear();
			orderedSpriteSizes = new List<Point2I>();
			var spriteDictionary = ZeldaResources.GetDictionary<ISprite>().ToArray();
			foreach (var pair in spriteDictionary) {
				ISprite sprite = pair.Value;
				if (sprite is Animation) {
					Animation anim = sprite as Animation;
					int substripIndex = 0;
					Animation substrip = anim;
					do {
						AddSprite(pair.Key, substrip, substripIndex);
						substrip = substrip.NextStrip;
						substripIndex++;
					} while (substrip != null);
				}
				else {
					AddSprite(pair.Key, sprite);
				}
			}

			// Sort the sprite sizes
			orderedSpriteSizes.Sort((p1, p2) => (p1.X + p1.Y) - (p2.X + p2.Y));
			if (!spriteSizes.ContainsKey(spriteSize) && orderedSpriteSizes.Any()) {
				spriteSize = orderedSpriteSizes[0];
			}

			comboBoxSpriteSizes.Items.Clear();
			foreach (Point2I point in orderedSpriteSizes) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = point.X + " x " + point.Y;
				item.Tag = point;
				comboBoxSpriteSizes.Items.Add(item);
			}
			comboBoxSpriteSizes.SelectedIndex = orderedSpriteSizes.IndexOf(spriteSize);

			UpdateFilter(textBoxSearch.Text);
			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			preview.Unload();
		}


		//-----------------------------------------------------------------------------
		// Sprites Setup
		//-----------------------------------------------------------------------------

		private Point2I RoundSize(Point2I size) {
			return ((size + 7) / 8) * 8;
		}

		private void AddSprite(string name, ISprite sprite, int substripIndex = 0) {
			SpriteInfo spr = new SpriteInfo(name, sprite, substripIndex);
			Point2I size = RoundSize(spr.Bounds.Size);
			if (!spriteSizes.ContainsKey(size)) {
				spriteSizes.Add(size, new List<SpriteInfo>());
				orderedSpriteSizes.Add(size);
			}
			spriteSizes[size].Add(spr);
		}

		private void UpdateFilter(string filter) {
			filteredSprites = new List<SpriteInfo>();
			if (!string.IsNullOrEmpty(filter)) {
				foreach (var spr in GetSizeList()) {
					if (spr.Name.Contains(filter)) {
						filteredSprites.Add(spr);
					}
				}
			}
			else {
				filteredSprites = GetSizeList();
			}
			preview.UpdateList(filteredSprites, spriteSize);
		}

		private List<SpriteInfo> GetSizeList() {
			if (spriteSizes.ContainsKey(spriteSize))
				return spriteSizes[spriteSize];
			return new List<SpriteInfo>();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnResourcesLoaded(object sender = null, EventArgs e = null) {
			Reload();
		}

		private void OnResourcesUnloaded(object sender = null, EventArgs e = null) {
			Unload();
		}

		private void OnPreviewInvalidated(object sender = null, EventArgs e = null) {
			preview.Invalidate();
		}

		private void OnPreviewScaleChanged(object sender = null, EventArgs e = null) {
			preview.UpdateScale();
		}

		private void OnHoverChanged(object sender = null, EventArgs e = null) {
			SpriteInfo hoverSprite = preview.HoverSprite;
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

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			UpdateFilter(textBoxSearch.Text);
		}

		private void OnSpriteSizeChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxSpriteSizes.SelectedIndex != -1) {
				spriteSize = (Point2I) ((ComboBoxItem) comboBoxSpriteSizes.SelectedItem).Tag;
				UpdateFilter(textBoxSearch.Text);
			}
		}
	}
}
