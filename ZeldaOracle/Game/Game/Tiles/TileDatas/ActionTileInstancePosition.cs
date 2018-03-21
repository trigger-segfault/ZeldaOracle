using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>Stores information on an action tile at a temporary position.</summary>
	public struct ActionTileInstancePosition {
		/// <summary>The action tile instance.</summary>
		public ActionTileDataInstance Action { get; set; }
		/// <summary>The position of the action tile.</summary>
		public Point2I Position { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileDataInstance actionTile,
			int x, int y)
		{
			Action	= actionTile;
			Position	= new Point2I(x, y);
		}

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileDataInstance actionTile,
			Point2I position)
		{
			Action	= actionTile;
			Position	= position;
		}
	}
}
