using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class TilesetSR : ConscriptRunner {

		private enum Modes {
			Root,
			Tileset,
		}
		
		private Tileset				tileset;
		private string				tilesetName;
		private bool                tilesetEnded;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			tilesetEnded = false;
			
			//=====================================================================================
			// TILE/TILESET BEGIN/END 
			//=====================================================================================
			AddCommand("TILESET", (int) Modes.Root,
				"string name, Point dimensions, bool usePreviewSprites = true",
			delegate (CommandParam parameters) {
				if (tilesetEnded)
					ThrowCommandParseError("Can only make one tileset per .conscript file!");
				tilesetName = parameters.GetString(0);
				tileset = new Tileset(tilesetName,
					parameters.GetPoint(1), parameters.GetBool(2));
				tileset.ConscriptPath = FileName;
				AddResource<Tileset>(tileset.ID, tileset);
				Mode = Modes.Tileset;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Tileset,
				"",
			delegate (CommandParam parameters) {
				tileset = null;
				tilesetName = "";
				Mode = Modes.Root;
				tilesetEnded = true;
			});
			//=====================================================================================
			// TILESET SETUP
			//=====================================================================================
			AddCommand("CLONE TILESET", (int) Modes.Tileset,
				"string tileset",
			delegate (CommandParam parameters) {
				Tileset cloneTileset = GetResource<Tileset>(parameters.GetString(0));
				tileset = new Tileset(cloneTileset);
				tileset.ID = tilesetName;
				tileset.ConscriptPath = FileName;
				SetResource<Tileset>(tilesetName, tileset);
			});
			//=====================================================================================
			AddCommand("RESIZE", (int) Modes.Tileset,
				"Point newDimensions",
			delegate (CommandParam parameters) {
				tileset.Resize(parameters.GetPoint(0));
			});
			//=====================================================================================
			// TILESET BUILDING
			//=====================================================================================
			AddCommand("ADDTILE", (int) Modes.Tileset,
				"Point tilesetIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.AddTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("SETTILE", (int) Modes.Tileset,
				"Point tilesetIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.SetTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("REMOVETILE", (int) Modes.Tileset,
				"Point tilesetIndex",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.RemoveTileData(location);
			});
			//=====================================================================================

		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			tileset		= null;
			tilesetName = "";
			tilesetEnded = false;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
