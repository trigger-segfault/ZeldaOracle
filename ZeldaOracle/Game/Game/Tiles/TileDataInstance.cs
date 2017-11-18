using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {
	public class TileDataInstance : BaseTileDataInstance, IEventObject, IIDObject {

		private Point2I	location;
		private int		layer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public TileDataInstance() {
			this.room		= null;
			this.location	= Point2I.Zero;
			this.layer		= 0;
			this.tileData	= null;
			this.properties = new Properties();
			this.properties.PropertyObject = this;
		}

		public TileDataInstance(TileData tileData) :
			this(tileData, -1, -1, -1)
		{
		}

		public TileDataInstance(TileData tileData, int x, int y, int layer) {
			this.room		= null;
			this.location	= new Point2I(x, y);
			this.layer		= layer;
			this.tileData	= tileData;
			this.properties = new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = tileData.Properties;
			this.modifiedProperties.PropertyObject = this;
			this.modifiedProperties.BaseProperties = tileData.Properties;
		}

		public override void Clone(BaseTileDataInstance copy) {
			base.Clone(copy);
			if (copy is TileDataInstance) {
				this.location	= ((TileDataInstance) copy).location;
				this.layer		= ((TileDataInstance) copy).layer;
			}
		}
		
		public override BaseTileDataInstance Duplicate() {
			TileDataInstance copy = new TileDataInstance();
			copy.Clone(this);
			return copy;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public bool IsAtLocation(int x, int y) {
			return (x == location.X && y == location.Y);
		}


		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------

		public void SetFlags(TileFlags flagsToSet, bool enabled) {
			TileFlags flags = Flags;
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
			Flags = flags;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods and Properties
		//-----------------------------------------------------------------------------

		public override Point2I GetPosition() {
			return (location * GameSettings.TILE_SIZE);
		}

		public override Rectangle2I GetBounds() {
			return new Rectangle2I(
					location * GameSettings.TILE_SIZE,
					Size * GameSettings.TILE_SIZE);
		}
		
		public override SpriteAnimation Sprite {
			get { return tileData.Sprite; }
			set { } // Don't see a need to set this.
		}

		public override SpriteAnimation CurrentSprite {
			get {
				if (TileData.SpriteList.Length > 0)
					return TileData.SpriteList[properties.GetInteger("sprite_index")];
				return new SpriteAnimation();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileData TileData {
			get { return (tileData as TileData); }
			set { base.BaseData = value; }
		}
		
		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		public string ID {
			get { return properties.GetString("id"); }
		}

		public int Layer {
			get { return layer; }
			set { layer = value; }
		}
		
		public Point2I Size {
			get { return properties.GetPoint("size", Point2I.One); }
			set { properties.Set("size", value); }
		}

		public SpriteAnimation[] SpriteList {
			get { return TileData.SpriteList; }
		}

		public SpriteAnimation SpriteAsObject {
			get { return TileData.SpriteAsObject; }
		}

		public Animation BreakAnimation {
			get { return TileData.BreakAnimation; }
		}

		public Sound BreakSound {
			get { return TileData.BreakSound; }
		}

		public TileFlags Flags {
			get { return (TileFlags) properties.GetInteger("flags", 0); }
			set { properties.Set("flags", (int) value); }
		}

		public CollisionModel CollisionModel {
			get { return properties.GetResource<CollisionModel>("collision_model", null); }
			set { properties.SetAsResource<CollisionModel>("collision_model", value); }
		}

		public TileSpawnOptions SpawnOptions {
			get {
				return new TileSpawnOptions() {
					PoofEffect = properties.GetBoolean("spawn_poof_effect", false),
					SpawnDelayAfterPoof = properties.GetInteger("spawn_delay_after_poof", 31),
				};
			}
		}

		public TileSolidType SolidType {
			get { return properties.GetEnum<TileSolidType>("solidity", TileSolidType.NotSolid); }
			set { properties.Set("solidity", (int) value); }
		}

		public TileResetCondition ResetCondition {
			get { return properties.GetEnum<TileResetCondition>("reset_condition", TileResetCondition.LeaveRoom); }
			set { properties.Set("reset_condition", (int) value); }
		}

		public int ConveyorAngle {
			get { return properties.GetInteger("conveyor_angle", -1); }
			set { properties.Set("conveyor_angle", value); }
		}

		public float ConveyorSpeed {
			get { return properties.GetFloat("conveyor_speed", 0.0f); }
			set { properties.Set("conveyor_speed", value); }
		}
	}
}
