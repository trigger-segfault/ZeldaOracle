using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	public class TileData : BaseTileData {

		private SpriteAnimation[]	spriteList;
		private Point2I				size;
		private SpriteAnimation		spriteAsObject;
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileData() {
			spriteList			= new SpriteAnimation[0];
			size				= Point2I.One;
			spriteAsObject		= new SpriteAnimation();
			breakAnimation		= null;

			// General.
			properties.Set("flags", (int) TileFlags.Default);
			properties.Set("solidity", (int) TileSolidType.NotSolid);
			properties.Set("collision_model", "");
			properties.Set("environment_type", (int) TileEnvironmentType.Normal);
			
			// Interaction Options.
			properties.Set("move_once", false);
			properties.Set("move_direction", -1);
			properties.Set("cuttable_sword_level", 0);
			properties.Set("pickupable_bracelet_level", 0);
			properties.Set("ledge_direction", Directions.Down);

			// Events.
			properties.Set("on_move", "")
				.SetDocumentation("On Move", "script", "", "Events",
				"Occurs when the tile is moved.", true, false);
		}
		
		public TileData(TileFlags flags) : this() {
			this.Flags = flags;
		}

		public TileData(Type type, TileFlags flags) : this() {
			this.type = type;
			this.Flags = flags;
		}

		public TileData(TileData copy) : base(copy) {
			size				= copy.size;
			spriteAsObject		= new SpriteAnimation(copy.spriteAsObject);
			breakAnimation		= copy.breakAnimation;
			spriteList			= new SpriteAnimation[copy.spriteList.Length];
			
			for (int i = 0; i < spriteList.Length; i++)
				spriteList[i] = new SpriteAnimation(copy.spriteList[i]);
		}

		public override void Clone(BaseTileData copy) {
			base.Clone(copy);
			if (copy is TileData) {
				TileData copyTileData = (TileData) copy;
				size				= copyTileData.size;
				spriteAsObject		= new SpriteAnimation(copyTileData.spriteAsObject);
				breakAnimation		= copyTileData.breakAnimation;
				events				= new ObjectEventCollection(copyTileData.events);

				if (copyTileData.spriteList.Length > 0) {
					spriteList = new SpriteAnimation[copyTileData.spriteList.Length];
					for (int i = 0; i < spriteList.Length; i++) {
						spriteList[i] = new SpriteAnimation(copyTileData.spriteList[i]);
					}
				}
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
			get { return (TileFlags) properties.GetInteger("flags", 0); }
			set { properties.Set("flags", (int) value); }
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
			get { return properties.GetResource<CollisionModel>("collision_model", null); }
			set { properties.SetAsResource<CollisionModel>("collision_model", value); }
		}

		public TileEnvironmentType EnvironmentType {
			get { return properties.GetEnum<TileEnvironmentType>("environment_type", TileEnvironmentType.Normal); }
			set { properties.Set("environment_type", (int) value); }
		}

		public TileSolidType SolidType {
			get { return properties.GetEnum<TileSolidType>("solidity", TileSolidType.NotSolid); }
			set { properties.Set("solidity", (int) value); }
		}

		public int LedgeDirection {
			get { return properties.GetInteger("ledge_direction", Directions.Down); }
			set { properties.Set("ledge_direction", value); }
		}
	}
}
