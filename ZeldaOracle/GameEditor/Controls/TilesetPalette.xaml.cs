using System;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Controls;

using ZeldaEditor.Util;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Content;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Controls {
	/// <summary>
	/// Interaction logic for TilesetPalette.xaml
	/// </summary>
	public partial class TilesetPalette : UserControl {

		private EditorWindow mainWindow;
		private TilesetDisplay tilesetDisplay;

		//private Tileset tileset;
		//private Zone zone;
		//private string tileSearchFilter;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TilesetPalette(EditorWindow mainWindow) {
			this.mainWindow = mainWindow;

			InitializeComponent();

			// Create the tileset display
			tilesetDisplay					= new TilesetDisplay();
			tilesetDisplay.EditorWindow		= mainWindow;
			tilesetDisplay.EditorControl	= mainWindow.EditorControl;
			tilesetDisplay.Name				= "tilesetDisplay";
			tilesetDisplay.Dock				= System.Windows.Forms.DockStyle.Fill;
			tilesetDisplay.HoverChanged		+= OnTilesetDisplayHoverChanged;
			hostTilesetDisplay.Child		= tilesetDisplay;

			SetupTilesetList();
			SetupZoneList();
		}

		private void SetupTilesetList() {
			int index = 0;

			// Create a sorted list of sorted tileset names
			List<string> tilesets = new List<string>();
			foreach (var pair in ZeldaResources.GetDictionary<Tileset>()) {
				tilesets.Add(pair.Key);
				if (tilesetDisplay.TilesSource.IsTileset &&
					pair.Key == tilesetDisplay.TilesSource.Tileset.ID)
					index = tilesets.Count; // No -1 because "<Tile List>" is added after sorting to the front
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a, b, true));

			// Add the tile list as the first option
			tilesets.Insert(0, "<Tile List>");
			
			comboBoxTilesets.ItemsSource = tilesets;
			comboBoxTilesets.SelectedIndex = index;
		}

		private void SetupZoneList() {
			int index = -1;

			// Create the list of sorted zone names
			List<string> zones = new List<string>();
			foreach (var pair in ZeldaResources.GetDictionary<Zone>()) {
				zones.Add(pair.Key);
				if (tilesetDisplay.Zone != null && pair.Key == tilesetDisplay.Zone.ID)
					index = zones.Count - 1;
			}
			zones.Sort((a, b) => AlphanumComparator.Compare(a, b, true));

			if (index < 0 && zones.Count > 0)
				index = 0;

			comboBoxZones.ItemsSource = zones;
			comboBoxZones.SelectedIndex = index;
		}
		

		//-----------------------------------------------------------------------------
		// Intneral Methods
		//-----------------------------------------------------------------------------

		private List<BaseTileData> CreateFilteredTileList(string filter) {
			List<BaseTileData> filteredTileData = new List<BaseTileData>();
			foreach (var pair in ZeldaResources.GetDictionary<BaseTileData>()) {
				if (pair.Key.Contains(filter)) {
					filteredTileData.Add(pair.Value);
				}
			}
			return filteredTileData;
		}


		//-----------------------------------------------------------------------------
		// UI Event Callbacks
		//-----------------------------------------------------------------------------

		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (comboBoxTilesets.SelectedIndex != -1) {
				if (comboBoxTilesets.SelectedIndex == 0) {
					toolbarTileSearch.Visibility = System.Windows.Visibility.Visible;
					tilesetDisplay.TilesSource = CreateFilteredTileList(textBoxTileSearch.Text);
				}
				else {
					toolbarTileSearch.Visibility = System.Windows.Visibility.Collapsed;
					string name = (string) comboBoxTilesets.SelectedItem;
					if (name == "<Tile List>")
						tilesetDisplay.TilesSource = CreateFilteredTileList(textBoxTileSearch.Text);
					else if (ZeldaResources.Contains<Tileset>(name))
						tilesetDisplay.TilesSource = ZeldaResources.Get<Tileset>(name);
				}
			}
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			if (comboBoxZones.SelectedIndex != -1) {
				string name = comboBoxZones.SelectedItem as string;
				if (name != null && name != "(none)")
					tilesetDisplay.Zone = ZeldaResources.Get<Zone>(name);
			}
		}

		private void OnTileSearchTextChanged(object sender, TextChangedEventArgs e) {
			tilesetDisplay.TilesSource = CreateFilteredTileList(textBoxTileSearch.Text);
		}

		private void OnTilesetDisplayHoverChanged(object sender, EventArgs e) {
			BaseTileData tileData = tilesetDisplay.HoverTileData;
			if (tileData == null)
				textBlockTileName.Text = "";
			else
				textBlockTileName.Text = tileData.ResourceName;
		}
	}
}
