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

		private TilesetPreview tilesetPreview;

		private bool suppressEvents;

		private string tilesetName;

		private List<KeyValuePair<string, ITileset>> tilesets;

		public TilesetBrowserControl() {
			InitializeComponent(); this.suppressEvents = true;
			InitializeComponent();

			this.tilesetPreview = new TilesetPreview();
			this.tilesetPreview.HoverTileDataChanged += OnHoverTileDataChanged;
			this.host.Child = this.tilesetPreview;
			this.suppressEvents = false;
			this.tilesets = new List<KeyValuePair<string, ITileset>>();
			this.tilesetName = "";

			DesignerControl.PreviewZoneChanged += OnPreviewZoneChanged;

			OnHoverTileDataChanged();
		}

		private void OnHoverTileDataChanged(object sender = null, EventArgs e = null) {
			BaseTileData hoverTileData = tilesetPreview.HoverTileData;
			Point2I hoverIndex = tilesetPreview.HoverIndex;
			if (hoverIndex == -Point2I.One)
				statusHoverIndex.Content = "(?, ?)";
			else
				statusHoverIndex.Content = hoverIndex.ToString();
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

		public void Dispose() {
			DesignerControl.PreviewZoneChanged -= OnPreviewZoneChanged;
			tilesetPreview.Dispose();
		}

		public void RefreshList() {
			suppressEvents = true;
			tilesets.Clear();
			comboBoxTilesets.Items.Clear();
			foreach (var pair in ZeldaResources.GetResourceDictionary<Tileset>()) {
				tilesets.Add(new KeyValuePair<string, ITileset>(pair.Key, pair.Value));
			}
			foreach (var pair in ZeldaResources.GetResourceDictionary<EventTileset>()) {
				tilesets.Add(new KeyValuePair<string, ITileset>(pair.Key, pair.Value));
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));
			foreach (var pair in tilesets) {
				comboBoxTilesets.Items.Add(pair.Key);
			}
			ITileset tileset = ZeldaResources.GetResource<Tileset>(tilesetName);
			if (tileset == null)
				tileset = ZeldaResources.GetResource<EventTileset>(tilesetName);
			if (tilesets.Any()) {
				if (tileset == null) {
					tilesetName = tilesets[0].Key;
					tileset = tilesets[0].Value;
				}
				comboBoxTilesets.SelectedItem = tilesetName;
				tilesetPreview.UpdateTileset(tilesetName, tileset);
			}
			comboBoxZones.ItemsSource = DesignerControl.PreviewZones;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
		}

		public void ClearList() {
			tilesets.Clear();
			tilesetPreview.ClearTileset();
		}

		private void OnToggleAnimations(object sender, RoutedEventArgs e) {
			tilesetPreview.Animating = !tilesetPreview.Animating;
			buttonRestartAnimations.IsEnabled = tilesetPreview.Animating;
		}

		private void OnRestartAnimations(object sender, RoutedEventArgs e) {
			tilesetPreview.RestartAnimations();
		}

		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				tilesetName = (string) comboBoxTilesets.SelectedItem;
				ITileset tileset = ZeldaResources.GetResource<Tileset>(tilesetName);
				if (tileset == null)
					tileset = ZeldaResources.GetResource<EventTileset>(tilesetName);
				tilesetPreview.UpdateTileset(tilesetName, tileset);
			}
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			DesignerControl.PreviewZoneID = (string) comboBoxZones.SelectedItem;
		}

		private void OnPreviewZoneChanged(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			suppressEvents = false;
			tilesetPreview.Invalidate();
		}
	}
}
