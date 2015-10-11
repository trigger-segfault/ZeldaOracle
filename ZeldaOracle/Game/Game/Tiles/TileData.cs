using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Properties;

namespace ZeldaOracle.Game.Tiles {

	public class TileData {

		private Type				type;

		private TileFlags			flags;
		private Point2I				size;
		private SpriteAnimation[]	spriteList;
		private SpriteAnimation		spriteAsObject;
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		private CollisionModel		collisionModel;
		private Point2I				sheetLocation;	// Location on the tileset.
		private Tileset				tileset;
		private Properties			properties;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileData() {
			type				= null;
			size				= Point2I.One;
			flags				= TileFlags.Default;
			spriteList			= new SpriteAnimation[0];
			spriteAsObject		= new SpriteAnimation();
			breakAnimation		= null;
			collisionModel		= null;
			sheetLocation		= Point2I.Zero;
			tileset				= null;
			properties			= new Properties();

			properties.Set("id", "")
				.SetDocumentation("ID", "", "", "The id used to refer to this tile.", false);
			properties.Set("sprite_index", 0)
				.SetDocumentation("Sprite Index", "sprite_index", "", "The current sprite in the sprite list to draw.", true);
		}
		
		public TileData(TileFlags flags) : this() {
			this.flags = flags;
		}
		
		public TileData(Type type, TileFlags flags) : this() {
			this.type	= type;
			this.flags	= flags;
		}

		public TileData(TileData copy) : this() {
			type				= copy.type;
			size				= copy.size;
			flags				= copy.flags;
			spriteList			= new SpriteAnimation[copy.spriteList.Length];
			spriteAsObject		= new SpriteAnimation(copy.spriteAsObject);
			breakAnimation		= copy.breakAnimation;
			collisionModel		= copy.collisionModel;
			sheetLocation		= copy.sheetLocation;
			tileset				= copy.tileset;
			properties			= new Properties();

			properties.Merge(copy.properties, true);

			for (int i = 0; i < spriteList.Length; i++)
				spriteList[i] = new SpriteAnimation(copy.spriteList[i]);
		}

		public void Clone(TileData copy) {
			type				= copy.type;
			size				= copy.size;
			flags				= copy.flags;
			breakAnimation		= copy.breakAnimation;
			collisionModel		= copy.collisionModel;
			properties			= new Properties();

			properties.Merge(copy.properties, true);
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

		public SpriteAnimation Sprite {
			get {
				if (spriteList.Length > 0)
					return spriteList[0];
				return new SpriteAnimation();
			}
			set {
				if (value == null)
					spriteList = new SpriteAnimation[0];
				else
					spriteAsObject.Set(value);
				spriteList = new SpriteAnimation[] { value };
			}
		}

		public SpriteAnimation[] SpriteList {
			get { return spriteList; }
			set { spriteList = value; }
		}

		public SpriteAnimation SpriteAsObject {
			get { return spriteAsObject; }
			set {
				if (value == null)
					spriteAsObject.SetNull();
				else
					spriteAsObject.Set(value);
			}
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

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}
	}
}
