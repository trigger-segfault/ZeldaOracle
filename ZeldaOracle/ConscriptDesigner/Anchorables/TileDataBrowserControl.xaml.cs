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
using ZeldaOracle.Game.Tiles;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteBrowserControl.xaml
	/// </summary>
	public partial class TileDataBrowserControl : UserControl {

		private TileDataPreview preview;
		
		private List<BaseTileData> tileData;
		private List<BaseTileData> filteredTileData;
		
		private bool suppressEvents;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tile data browser control.</summary>
		public TileDataBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();
			
			this.preview = new TileDataPreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.tileData = new List<BaseTileData>();
			this.filteredTileData = new List<BaseTileData>();


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

			tileData.Clear();
			filteredTileData.Clear();
			foreach (var pair in ZeldaResources.GetResourceDictionary<BaseTileData>()) {
				tileData.Add(pair.Value);
			}

			UpdateFilter();
			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			tileData.Clear();
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
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			UpdateFilter();
		}
	}
}
