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
	/// <summary>A grid of tiles that can be shifted around between levels.</summary>
	public partial class TileGrid {

		/// <summary>The structure used to serialize to the clipboard.</summary>
		[Serializable]
		private class TileGridReference {
			
			// UNUSED
			private int		resourceChecksum;

			private Point2I	size;
			private int		startLayer;
			private int		layerCount;
			private bool	includeTiles;
			private bool	includeActions;
			private TileInstanceReference[,,]			tiles;
			private List<ActionTileInstanceReference>	actionTiles;


			//-----------------------------------------------------------------------------
			// Constructors
			//-----------------------------------------------------------------------------

			/// <summary>Constructs a tile grid reference from the tile grid.</summary>
			public TileGridReference(TileGrid tileGrid) {
				resourceChecksum	= 0;// Resources.Checksum;

				size			= tileGrid.Size;
				startLayer		= tileGrid.StartLayer;
				layerCount		= tileGrid.LayerCount;
				includeTiles	= tileGrid.IncludesTiles;
				includeActions	= tileGrid.IncludesActions;
				tiles			= new TileInstanceReference[size.X, size.Y, layerCount];
				actionTiles		= new List<ActionTileInstanceReference>();
				
				if (tileGrid.IncludesTiles) {
					foreach (TileInstanceLocation tile in
						tileGrid.GetTilesAndLocations())
					{
						tiles[tile.Location.X, tile.Location.Y,
							tile.Layer - startLayer] = new TileInstanceReference(tile);
					}
				}

				if (tileGrid.IncludesActions) {
					foreach (ActionTileInstancePosition actionTile in
						tileGrid.GetActionTilesAndPositions())
					{
						actionTiles.Add(new ActionTileInstanceReference(actionTile));
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
								TileInstanceReference tile = tiles[x, y, i];
								if (tile == null)
									continue;
								if (!tile.ConfirmResourceExists())
									throw new ResourceReferenceException(
										typeof(TileData), tile.Name);
								tileGrid.PlaceTile(tile.Dereference(),
									x, y, i + startLayer);
							}
						}
					}
				}

				if (includeActions) {
					foreach (ActionTileInstanceReference action in actionTiles) {
						if (!action.ConfirmResourceExists())
							throw new ResourceReferenceException(
								typeof(ActionTileData), action.Name);
						tileGrid.PlaceActionTile(action.Dereference(),
							action.Position);
					}
				}


				return tileGrid;
			}


			//-----------------------------------------------------------------------------
			// Propreties
			//-----------------------------------------------------------------------------

			/// <summary>Gets the checksum to confirm the resources all match.</summary>
			public int ResourceChecksum {
				get { return resourceChecksum; }
			}

			/// <summary>Gets if tiles are included.</summary>
			public bool IncludesTiles {
				get { return includeTiles; }
			}

			/// <summary>Gets if action tiles are included.</summary>
			public bool IncludesActions {
				get { return includeActions; }
			}

			/// <summary>Gets the starting layer for tiles.</summary>
			public int StartLayer {
				get { return startLayer; }
			}

			/// <summary>Gets the number of tile layers.</summary>
			public int LayerCount {
				get { return layerCount; }
			}

			/// <summary>Gets the tile width.</summary>
			public int Width {
				get { return size.X; }
			}

			/// <summary>Gets the tile height.</summary>
			public int Height {
				get { return size.Y; }
			}

			/// <summary>Gets the tile size.</summary>
			public Point2I Size {
				get { return size; }
			}
		}
	}
}
