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

		/// <summary>Constructs an empty action tile instance position with the
		/// specified action tile.</summary>
		public ActionTileInstancePosition(int x, int y)
		{
			Action		= null;
			Position	= new Point2I(x, y);
		}

		/// <summary>Constructs an empty action tile instance position with the
		/// specified action tile.</summary>
		public ActionTileInstancePosition(Point2I position) {
			Action		= null;
			Position	= position;
		}

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileData actionData, int x, int y) {
			Action		= null;
			if (actionData != null)
				Action	= new ActionTileDataInstance(actionData);
			Position	= new Point2I(x, y);
		}

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileData actionData,
			Point2I position)
		{
			Action		= null;
			if (actionData != null)
				Action	= new ActionTileDataInstance(actionData);
			Position	= position;
		}

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileDataInstance actionTile,
			int x, int y)
		{
			Action		= actionTile;
			Position	= new Point2I(x, y);
		}

		/// <summary>Constructs the action tile instance position with the specified
		/// action tile.</summary>
		public ActionTileInstancePosition(ActionTileDataInstance actionTile,
			Point2I position)
		{
			Action		= actionTile;
			Position	= position;
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an action tile instance location from the action tile's
		/// room position.</summary>
		public static ActionTileInstancePosition FromRoom(
			ActionTileDataInstance actionTile)
		{
			return new ActionTileInstancePosition(actionTile, actionTile.Position);
		}

		/// <summary>Constructs an action tile instance location from the action tile's
		/// level position.</summary>
		public static ActionTileInstancePosition FromLevel(
			ActionTileDataInstance actionTile)
		{
			return new ActionTileInstancePosition(actionTile,
				actionTile.LevelPosition);
		}
	}
}
