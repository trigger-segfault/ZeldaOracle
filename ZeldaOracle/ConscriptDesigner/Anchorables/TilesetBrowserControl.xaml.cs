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
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for TilesetBrowserControl.xaml
	/// </summary>
	public partial class TilesetBrowserControl : UserControl {

		private TilesetPreview preview;

		private List<KeyValuePair<string, ITileset>> tilesets;
		private ITileset tileset;
		private string tilesetName;

		private bool suppressEvents;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tileset browser control.</summary>
		public TilesetBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.preview = new TilesetPreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.suppressEvents = false;
			this.tilesets = new List<KeyValuePair<string, ITileset>>();
			this.tileset = null;
			this.tilesetName = "";

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

			tilesets.Clear();

			foreach (var pair in ZeldaResources.GetResourceDictionary<ITileset>()) {
				tilesets.Add(new KeyValuePair<string, ITileset>(pair.Key, pair.Value));
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));

			comboBoxTilesets.Items.Clear();
			foreach (var pair in tilesets) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = pair.Key;
				item.Tag = pair.Key;
				comboBoxTilesets.Items.Add(item);
			}
			tileset = ZeldaResources.GetResource<ITileset>(tilesetName);
			if (tilesets.Any() && tileset == null) {
				tilesetName = tilesets[0].Key;
				tileset = tilesets[0].Value;
			}
			comboBoxTilesets.SelectedIndex = tilesets.IndexOf(
				new KeyValuePair<string, ITileset>(tilesetName, tileset));

			UpdateTileset();
			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			preview.Unload();
		}


		//-----------------------------------------------------------------------------
		// Sprites Setup
		//-----------------------------------------------------------------------------

		private void UpdateTileset() {
			if (tileset != null)
				preview.UpdateList(tileset);
			else
				preview.Unload();
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
			BaseTileData hoverTileData = preview.HoverTileData;
			if (hoverTileData == null) {
				textBlockTileName.Text = "";
				statusTileInfo.Content = "";
			}
			else {
				textBlockTileName.Text = hoverTileData.Name;
				if (hoverTileData.Type == null) {
					if (hoverTileData is TileData)
						statusTileInfo.Content = "Type: Tile";
					else
						statusTileInfo.Content = "Type: EventTile";
				}
				else {
					statusTileInfo.Content = "Type: " + hoverTileData.Type.Name;
				}
			}

			Point2I hoverPoint = preview.HoverPoint;
			if (hoverPoint == -Point2I.One)
				statusHoverIndex.Content = "(?, ?)";
			else
				statusHoverIndex.Content = hoverPoint.ToString();
		}

		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				tilesetName = (string) ((ComboBoxItem) comboBoxTilesets.SelectedItem).Tag;
				tileset = ZeldaResources.GetResource<ITileset>(tilesetName);
				UpdateTileset();
			}
		}
	}
}
