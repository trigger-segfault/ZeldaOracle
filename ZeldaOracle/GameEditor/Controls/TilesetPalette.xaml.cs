using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Tiles;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Util;

namespace ZeldaEditor.Controls {
	/// <summary>
	/// Interaction logic for TilesetPalette.xaml
	/// </summary>
	public partial class TilesetPalette : UserControl {

		private EditorWindow mainWindow;
		private TilesetDisplay tilesetDisplay;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TilesetPalette(EditorWindow mainWindow) {
			this.mainWindow = mainWindow;

			InitializeComponent();

			// Create the tileset display
			tilesetDisplay					= new TilesetDisplay();
			tilesetDisplay.EditorControl	= mainWindow.EditorControl;
			tilesetDisplay.Name				= "tilesetDisplay";
			tilesetDisplay.Dock				= System.Windows.Forms.DockStyle.Fill;
			tilesetDisplay.HoverChanged		+= OnTilesetDisplayHoverChanged;
			hostTilesetDisplay.Child		= tilesetDisplay;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler SelectionChanged {
			add { tilesetDisplay.SelectionChanged += value; }
			remove { tilesetDisplay.SelectionChanged -= value; }
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
		

		//-----------------------------------------------------------------------------
		// Innteral Methods
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
		// Properties
		//-----------------------------------------------------------------------------

		public IEnumerable<Tileset> Tilesets {
			set {
				// Create a sorted list of tileset names
				List<string> names = new List<string>();
				foreach (Tileset tileset in value)
					names.Add(tileset.ID);
				names.Sort((a, b) => AlphanumComparator.Compare(a, b, true));

				// Add the tile list as the first option
				names.Insert(0, "<Tile List>");
			
				comboBoxTilesets.ItemsSource = names;
				comboBoxTilesets.SelectedIndex = 0;
			}
		}

		public IEnumerable<Zone> Zones {
			set {
				// Create a sorted list of zone names
				List<string> names = new List<string>();
				foreach (Zone zone in value)
					names.Add(zone.ID);
				names.Sort((a, b) => AlphanumComparator.Compare(a, b, true));
			
				comboBoxZones.ItemsSource = names;
				comboBoxZones.SelectedIndex = 0;
			}
		}

		public BaseTileData SelectedTileData {
			get { return tilesetDisplay.SelectedTileData; }
			set { tilesetDisplay.SelectedTileData = value; }
		}
	}
}
