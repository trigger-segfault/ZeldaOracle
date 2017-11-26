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
		
		public EventTileDataInstance() {

		}

		public EventTileDataInstance(EventTileData tileData) :
			base(tileData)
		{
			this.sprite		= tileData.Sprite;
		}

		public EventTileDataInstance(EventTileData tileData, Point2I position) :
			base(tileData) {
			this.position   = position;
			this.sprite     = tileData.Sprite;
		}

		public override void Clone(BaseTileDataInstance copy) {
			base.Clone(copy);
			if (copy is EventTileDataInstance) {
				this.position	= ((EventTileDataInstance) copy).position;
				this.sprite		= new SpriteAnimation(((EventTileDataInstance) copy).sprite);
			}
		}
		
		public override BaseTileDataInstance Duplicate() {
			EventTileDataInstance copy = new EventTileDataInstance();
			copy.Clone(this);
			return copy;
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override Point2I GetPosition() {
			return position;
		}

		public override Rectangle2I GetBounds() {
			return new Rectangle2I(
					position,
					Size * GameSettings.TILE_SIZE);
		}

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
			set { base.BaseData = value; }
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
