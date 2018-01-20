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
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteSourceBrowserControl.xaml
	/// </summary>
	public partial class SpriteSourceBrowserControl : UserControl {

		private SpriteSourcePreview preview;
		
		private List<KeyValuePair<string, ISpriteSource>> sources;
		private SpriteInfo[,] spriteGrid;
		private ISpriteSource source;
		private Point2I spriteSize;
		private string sourceName;

		private bool suppressEvents;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sprite source browser control.</summary>
		public SpriteSourceBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.preview = new SpriteSourcePreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.sources = new List<KeyValuePair<string, ISpriteSource>>();
			this.spriteGrid = new SpriteInfo[0, 0];
			this.spriteSize = Point2I.One;
			this.sourceName = "";
			
			DesignerControl.ResourcesLoaded += OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded += OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated += OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged += OnPreviewScaleChanged;

			OnHoverChanged();

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

			sources.Clear();
			spriteGrid = new SpriteInfo[0, 0];

			foreach (var pair in ZeldaResources.GetResourceDictionary<ISpriteSource>()) {
				sources.Add(pair);
			}
			sources.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));

			comboBoxSpriteSources.Items.Clear();
			foreach (var pair in sources) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = pair.Key;
				item.Tag = pair.Key;
				comboBoxSpriteSources.Items.Add(item);
			}
			source = ZeldaResources.GetResource<ISpriteSource>(sourceName);
			if (sources.Any() && source == null) {
				sourceName = sources[0].Key;
				source = sources[0].Value;
			}
			comboBoxSpriteSources.SelectedIndex = sources.IndexOf(
				new KeyValuePair<string, ISpriteSource>(sourceName, source));

			UpdateSource();

			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			preview.Unload();
		}


		//-----------------------------------------------------------------------------
		// Sprites Setup
		//-----------------------------------------------------------------------------

		private void UpdateSource() {
			spriteSize = Point2I.One;
			if (source != null) {
				spriteGrid = new SpriteInfo[source.Width, source.Height];
				for (int x = 0; x < source.Width; x++) {
					for (int y = 0; y < source.Height; y++) {
						ISprite sprite = source.GetSprite(x, y);
						if (sprite != null) {
							spriteGrid[x, y] = new SpriteInfo("", sprite);
							spriteSize = GMath.Max(spriteSize, spriteGrid[x, y].Bounds.Size);
						}
					}
				}
				preview.UpdateList(spriteGrid, source, spriteSize);
			}
			else {
				spriteGrid = new SpriteInfo[0, 0];
				preview.Unload();
			}
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
				statusSpriteInfo.Content = "";
			}
			else {
				string info = "Type: " + hoverSprite.Sprite.GetType().Name.Replace("Sprite", "");
				if (hoverSprite.HasSubstrips)
					info += ", Substrip: " + hoverSprite.SubstripIndex;
				statusSpriteInfo.Content = info;
			}

			Point2I hoverPoint = preview.HoverPoint;
			if (hoverPoint == -Point2I.One)
				statusHoverIndex.Content = "(?, ?)";
			else
				statusHoverIndex.Content = hoverPoint.ToString();
		}

		private void OnSpriteSourceChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxSpriteSources.SelectedIndex != -1) {
				sourceName = (string) ((ComboBoxItem) comboBoxSpriteSources.SelectedItem).Tag;
				source = ZeldaResources.GetResource<ISpriteSource>(sourceName);
				UpdateSource();
			}
		}
	}
}
