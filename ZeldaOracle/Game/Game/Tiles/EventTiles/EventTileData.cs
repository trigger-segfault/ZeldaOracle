using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	
	public class EventTileData : BaseTileData {

		private Point2I			size;
		private Point2I			position;
		private SpriteAnimation	sprite;		// NOTE: This would only be visible in the editor.
		//private bool			isVisible;	// Is the event visible in-game?
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EventTileData() {
			sprite			= new SpriteAnimation();
			position		= Point2I.Zero;
			size			= Point2I.One;
		}
		
		public EventTileData(Type type) {
			this.type		= type;
			this.position	= Point2I.Zero;
			this.size		= Point2I.One;
			this.sprite		= null;
		}
		
		
		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------
		
		public override SpriteAnimation Sprite {
			get { return sprite; }
			set { sprite = value; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Point2I Position {
			get { return position; }
			set { position = value; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}
	}
}
