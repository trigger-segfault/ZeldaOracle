using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	/// <summary>A reference to an action tile instance that can be safetly copied
	/// to the clipboard.</summary>
	[Serializable]
	public class ActionTileInstanceReference
		: BaseTileInstanceReference<ActionTileData, ActionTileDataInstance>
	{
		/// <summary>The position of the action in the clipboard.</summary>
		private Point2I position;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reference to the action tile instance.</summary>
		public ActionTileInstanceReference(ActionTileDataInstance action)
			: base(action)
		{
			position	= action.Position;
		}

		/// <summary>Constructs a reference to the action tile instance with the
		/// temporary position.</summary>
		public ActionTileInstanceReference(ActionTileInstancePosition action)
			: base(action.Action)
		{
			position	= action.Position;
		}

		/// <summary>Constructs a reference to the tile action instance with the
		/// custom position.</summary>
		public ActionTileInstanceReference(ActionTileDataInstance action,
			Point2I position) : base(action)
		{
			this.position	= position;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected override ActionTileDataInstance SetupInstance(ActionTileData data) {
			ActionTileDataInstance actionInstance = new ActionTileDataInstance(data);
			actionInstance.Position	= position;
			return actionInstance;
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
