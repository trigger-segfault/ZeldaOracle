using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	[Serializable]
	public class ActionTileDataReference :
		BaseTileDataReference<ActionTileData, ActionTileDataInstance>
	{
		/// <summary>The position of the action in the clipboard.</summary>
		private Point2I position;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ActionTileDataReference(ActionTileDataInstance actionData) : base(actionData) {
			this.position	= actionData.Position;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected override ActionTileDataInstance SetupInstance(ActionTileData data) {
			ActionTileDataInstance tileData = new ActionTileDataInstance(data);
			tileData.Position	= position;
			return tileData;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the position of the action in the clipboard.</summary>
		public Point2I Position {
			get { return position; }
		}
	}
}
