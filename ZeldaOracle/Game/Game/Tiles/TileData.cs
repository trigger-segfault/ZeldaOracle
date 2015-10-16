using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	public class TileData : BaseTileData {

		private SpriteAnimation[]	spriteList;
		private TileFlags			flags;
		private Point2I				size;
		private SpriteAnimation		spriteAsObject;
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		private CollisionModel		collisionModel;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileData() {
			spriteList			= new SpriteAnimation[0];
			size				= Point2I.One;
			flags				= TileFlags.Default;
			spriteAsObject		= new SpriteAnimation();
			breakAnimation		= null;
			collisionModel		= null;
		}
		
		public TileData(TileFlags flags) : this() {
			this.flags = flags;
		}
		
		public TileData(Type type, TileFlags flags) : this() {
			this.type	= type;
			this.flags	= flags;
		}

		public TileData(TileData copy) : base(copy) {
			size				= copy.size;
			flags				= copy.flags;
			spriteAsObject		= new SpriteAnimation(copy.spriteAsObject);
			breakAnimation		= copy.breakAnimation;
			collisionModel		= copy.collisionModel;
			spriteList			= new SpriteAnimation[copy.spriteList.Length];
			
			for (int i = 0; i < spriteList.Length; i++)
				spriteList[i] = new SpriteAnimation(copy.spriteList[i]);
		}

		public override void Clone(BaseTileData copy) {
			base.Clone(copy);
			if (copy is TileData) {
				TileData copyTileData = (TileData) copy;
				size				= copyTileData.size;
				flags				= copyTileData.flags;
				spriteAsObject		= new SpriteAnimation(copyTileData.spriteAsObject);
				breakAnimation		= copyTileData.breakAnimation;
				collisionModel		= copyTileData.collisionModel;
				spriteList			= new SpriteAnimation[copyTileData.spriteList.Length];
				
				for (int i = 0; i < spriteList.Length; i++)
					spriteList[i] = new SpriteAnimation(copyTileData.spriteList[i]);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override SpriteAnimation Sprite {
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteAnimation[] SpriteList {
			get { return spriteList; }
			set { spriteList = value; }
		}

		public Point2I Size {
			get { return size; }
			set { size = value; }
		}

		public TileFlags Flags {
			get { return flags; }
			set { flags = value; }
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
	}
}
