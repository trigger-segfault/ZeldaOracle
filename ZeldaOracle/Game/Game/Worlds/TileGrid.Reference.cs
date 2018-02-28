using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Tiles.Clipboard;
using Clipboard = System.Windows.Forms.Clipboard;

namespace ZeldaOracle.Game.Worlds {
	public partial class TileGrid {

		/// <summary>The structure used to serialize to the clipboard.</summary>
		[Serializable]
		private class TileGridReference {
			
			// UNUSED
			private int     resourceChecksum;

			private Point2I size;
			private int     startLayer;
			private int     layerCount;
			private bool    includeTiles;
			private bool    includeActions;
			private TileDataReference[,,]           tiles;
			private List<ActionTileDataReference>   actionTiles;


			//-----------------------------------------------------------------------------
			// Constructors
			//-----------------------------------------------------------------------------

			/// <summary>Constructs a tile grid reference from the tile grid.</summary>
			public TileGridReference(TileGrid tileGrid) {
				this.resourceChecksum   = 0;// Resources.Checksum;

				this.size           = tileGrid.Size;
				this.startLayer     = tileGrid.StartLayer;
				this.layerCount     = tileGrid.LayerCount;
				this.includeTiles   = tileGrid.IncludesTiles;
				this.includeActions = tileGrid.IncludesActions;
				this.tiles          = new TileDataReference[size.X, size.Y, layerCount];
				this.actionTiles    = new List<ActionTileDataReference>();


				if (tileGrid.IncludesTiles) {
					foreach (TileDataInstance tile in tileGrid.GetTilesAtLocation()) {
						tiles[tile.Location.X, tile.Location.Y,
							tile.Layer - startLayer] = new TileDataReference(tile);
					}
				}

				if (tileGrid.IncludesActions) {
					foreach (ActionTileDataInstance actionTile in
						tileGrid.GetActionTilesAtPosition()) {
						actionTiles.Add(new ActionTileDataReference(actionTile));
					}
				}
			}


			//-----------------------------------------------------------------------------
			// Clipboard
			//-----------------------------------------------------------------------------

			/// <summary>Dereferences the tile grid and returns a normal tile grid.</summary>
			public TileGrid Dereference() {
				TileGrid tileGrid = new TileGrid(size,
				startLayer, layerCount,
				includeTiles, includeActions);

				if (includeTiles) {
					for (int x = 0; x < size.X; x++) {
						for (int y = 0; y < size.Y; y++) {
							for (int i = 0; i < layerCount; i++) {
								TileDataReference tile = tiles[x, y, i];
								if (tile == null)
									continue;
								if (!tile.ConfirmResourceExists())
									throw new ResourceReferenceException(typeof(TileData), tile.Name);
								tileGrid.PlaceTile(tile.Dereference(),
									x, y, i + startLayer);
							}
						}
					}
				}

				if (includeActions) {
					foreach (ActionTileDataReference action in actionTiles) {
						if (!action.ConfirmResourceExists())
							throw new ResourceReferenceException(typeof(ActionTileData), action.Name);
						tileGrid.PlaceActionTile(action.Dereference(), action.Position);
					}
				}


				return tileGrid;
			}


			//-----------------------------------------------------------------------------
			// Propreties
			//-----------------------------------------------------------------------------

			public int ResourceChecksum {
				get { return resourceChecksum; }
			}

			public bool IncludesTiles {
				get { return includeTiles; }
			}

			public bool IncludesActions {
				get { return includeActions; }
			}

			public int StartLayer {
				get { return startLayer; }
			}

			public int LayerCount {
				get { return layerCount; }
			}

			public int Width {
				get { return size.X; }
			}

			public int Height {
				get { return size.Y; }
			}

			public Point2I Size {
				get { return size; }
			}
		}
	}
}
