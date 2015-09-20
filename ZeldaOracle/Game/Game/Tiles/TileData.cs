using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;

namespace ZeldaOracle.Game.Tiles {

	public class TileData {

		private Type			type;
		private TileFlags		flags;
		private Point2I			size;
		private Sprite			sprite;
		private Sprite			spriteAsObject;
		private Animation		animation;
		private Animation		breakAnimation;	// The animation to play when the tile is broken.
		private CollisionModel	collisionModel;
		private Point2I			sheetLocation;	// Location on the tileset.
		private Tileset			tileset;



		// TODO: Properties


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileData() {
			type			= null;
			size			= Point2I.One;
			flags			= TileFlags.Default;
			sprite			= null;
			spriteAsObject	= null;
			animation		= null;
			breakAnimation	= null;
			collisionModel	= null;
			sheetLocation	= Point2I.Zero;
			tileset			= null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Type Type {
			get { return type; }
			set { type = value; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}

		public TileFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		public Sprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		public Sprite SpriteAsObject {
			get { return spriteAsObject; }
			set { spriteAsObject = value; }
		}

		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}

		public Animation BreakAnimation {
			get { return breakAnimation; }
			set { breakAnimation = value; }
		}

		public CollisionModel CollisionModel {
			get { return collisionModel; }
			set { collisionModel = value; }
		}

		public Point2I SheetLocation {
			get { return sheetLocation; }
			set { sheetLocation = value; }
		}
		
		public Tileset Tileset {
			get { return tileset; }
			set { tileset = value; }
		}
		
	}
}
