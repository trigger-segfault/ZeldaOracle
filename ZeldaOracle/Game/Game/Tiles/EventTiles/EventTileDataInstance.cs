using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	
	public class EventTileDataInstance : BaseTileDataInstance {

		private Point2I			position;
		private SpriteAnimation	sprite;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EventTileDataInstance(EventTileData tileData, Point2I position) :
			base(tileData)
		{
			this.position	= position;
			this.sprite		= tileData.Sprite;
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		// The current sprite/animation to visually display.
		public override SpriteAnimation CurrentSprite {
			get {
				if (sprite.IsAnimation)
					return sprite.Animation.GetSubstrip(SubStripIndex);
				return sprite;
			}
		}

		public override SpriteAnimation Sprite {
			get { return sprite; }
			set { sprite = value; }
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public EventTileData EventTileData {
			get { return (EventTileData) tileData; }
			set { tileData = value; }
		}
		
		public Point2I Position {
			get { return position; }
			set { position = value; }
		}

		public Point2I Size {
			get { return EventTileData.Size; }
		}

		public int SubStripIndex {
			get { return properties.GetInteger("substrip_index"); }
			set { properties.Set("substrip_index", value); }
		}
	}
}
