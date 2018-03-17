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
using ZeldaOracle.Game.Tiles;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for TileBrowserControl.xaml
	/// </summary>
	public partial class TileBrowserControl : UserControl {

		const string TileListName = "<Tile List>";

		private TilePreview preview;

		private List<BaseTileData> tileData;
		private List<BaseTileData> filteredTileData;

		private List<KeyValuePair<string, Tileset>> tilesets;
		private Tileset tileset;
		private string tilesetName;


		private bool suppressEvents;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tile browser control.</summary>
		public TileBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.preview = new TilePreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.tileData = new List<BaseTileData>();
			this.filteredTileData = new List<BaseTileData>();
			this.tilesets = new List<KeyValuePair<string, Tileset>>();
			this.tileset = null;
			this.tilesetName = TileListName;
			this.statusBarHoverIndex.Visibility = Visibility.Collapsed;

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
			DesignerControl.SelectedTileset = null;
			DesignerControl.SelectedTileData = null;
			DesignerControl.SelectedTileLocation = -Point2I.One;
			DesignerControl.ResourcesLoaded -= OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded -= OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated -= OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged -= OnPreviewScaleChanged;
			preview.Dispose();
		}

		public void Reload() {
			suppressEvents = true;

			tileData.Clear();
			filteredTileData.Clear();
			foreach (var pair in ZeldaResources.GetDictionary<BaseTileData>()) {
				tileData.Add(pair.Value);
			}

			tilesets.Clear();
			foreach (var pair in ZeldaResources.GetDictionary<Tileset>()) {
				tilesets.Add(new KeyValuePair<string, Tileset>(pair.Key, pair.Value));
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));

			tilesets.Insert(0, new KeyValuePair<string, Tileset>(TileListName, null));

			comboBoxTilesets.Items.Clear();
			foreach (var pair in tilesets) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = pair.Key;
				item.Tag = pair.Key;
				comboBoxTilesets.Items.Add(item);
			}
			tileset = ZeldaResources.Get<Tileset>(tilesetName);
			if (tilesets.Any() && tileset == null) {
				tilesetName = tilesets[0].Key;
				tileset = null;
			}
			comboBoxTilesets.SelectedIndex = tilesets.IndexOf(
				new KeyValuePair<string, Tileset>(tilesetName, tileset));

			UpdateTiles();
			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			tileData.Clear();
			tilesets.Clear();
			preview.Unload();
		}


		//-----------------------------------------------------------------------------
		// Sprites Setup
		//-----------------------------------------------------------------------------

		private void UpdateFilter() {
			string filter = textBoxSearch.Text;
			filteredTileData = new List<BaseTileData>();
			if (!string.IsNullOrEmpty(filter)) {
				foreach (var tile in tileData) {
					if (tile.Name.Contains(filter)) {
						filteredTileData.Add(tile);
					}
				}
			}
			else {
				filteredTileData = tileData;
			}
			preview.UpdateList(filteredTileData);
		}
		
		private void UpdateTiles() {
			if (tileset != null)
				preview.UpdateList(tileset);
			else if (tilesetName == TileListName)
				UpdateFilter();
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

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			UpdateFilter();
		}

		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				tilesetName = (string) ((ComboBoxItem) comboBoxTilesets.SelectedItem).Tag;
				if (comboBoxTilesets.SelectedIndex == 0) {
					toolbarSearch.Visibility = Visibility.Visible;
					statusBarHoverIndex.Visibility = Visibility.Collapsed;
					tileset = null;
				}
				else {
					toolbarSearch.Visibility = Visibility.Collapsed;
					statusBarHoverIndex.Visibility = Visibility.Visible;
					tileset = ZeldaResources.Get<Tileset>(tilesetName);
				}
				UpdateTiles();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private bool TileListMode {
			get { return tilesetName == TileListName; }
		}
	}
}
