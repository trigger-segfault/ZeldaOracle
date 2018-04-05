using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.ResourceData;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>The data structure detailing a normal tile that is confined
	/// to the tile grid.</summary>
	public class TileData : BaseTileData {
		
		private ISprite[]			spriteList;
		private ISprite             spriteAbove;
		private ISprite				spriteAsObject;
		private DepthLayer          breakLayer;
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		private Sound				breakSound;
		private CollisionModel		model;
		/// <summary>The tile to appear when this one is removed while on layer 1.</summary>
		private TileData            tileBelow;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------


		public TileData() {
			spriteList			= new ISprite[0];
			spriteAbove			= null;
			spriteAsObject		= null;
			breakAnimation		= null;
			breakLayer    = DepthLayer.EffectTileBreak;
			model               = null;
			tileBelow			= null;

			// General
			properties.Set("size", Point2I.One)
				.SetDocumentation("Size", "General", "The size of the tile in tiles.");
			properties.SetEnumInt("flags", TileFlags.Default)
				.SetDocumentation("Tile Flags", "enum_flags", typeof(TileFlags), "General", "");
			properties.SetEnumInt("solidity", TileSolidType.NotSolid)
				.SetDocumentation("Solid Type", "enum", typeof(TileSolidType), "General", "");
			properties.Set("ledge_direction", Direction.Down)
				.SetDocumentation("Ledge Direction", "direction", "", "General", "");
			//properties.Set("collision_model", "")
			//	.SetDocumentation("Collision Model", "collision_model", "", "General", "");
			properties.SetEnumInt("environment_type", TileEnvironmentType.Normal)
				.SetDocumentation("Environment Type", "enum", typeof(TileEnvironmentType), "General", "");
			properties.SetEnumStr("polarity", Polarity.None)
				.SetDocumentation("Polarity", "enum", typeof(Polarity), "General", "The magnetic polarity (north or south) for interaction with the magnetic gloves.");

			// Motion
			properties.Set("path", "")
				.SetDocumentation("Path", "path", "", "Motion", "A path the tile follows in.");
			properties.Set("conveyor_angle", -1)
				.SetDocumentation("Conveyor Angle", "angle", "", "Motion", "");
			properties.Set("conveyor_speed", 0.0f)
				.SetDocumentation("Conveyor Speed", "Motion", "");

			// Interaction Options.
			properties.Set("move_once", false)
				.SetDocumentation("Move Once", "Interactions", "");
			properties.Set("move_direction", -1)
				.SetDocumentation("Move Direction", "direction", "", "Interactions", "");
			properties.Set("cuttable_sword_level", 0)
				.SetDocumentation("Cuttable Sword Level", "Interactions", "");
			properties.Set("pickupable_bracelet_level", 0)
				.SetDocumentation("Pickupable Bracelet Level", "Interactions", "");
			properties.Set("raised_on_buttons", false)
				.SetDocumentation("Raised on Buttons", "Interactions", "True if a the tile appears raised when pushed onto a button.");

			properties.Set("hurt_area_point", new Point2I(-1, -1));
			properties.Set("hurt_area_size", new Point2I(18, 18));
			properties.Set("hurt_damage", 0);

			// Spawning.
			properties.Set("spawn_from_ceiling", false)
				.SetDocumentation("Spawn from Ceiling", "Spawning", "");
			properties.Set("spawn_poof_effect", false)
				.SetDocumentation("Spawn with Poof Effect", "Spawning", "");
			properties.Set("spawn_delay_after_poof", 31)
				.SetDocumentation("Spawn Delay after Poof", "Spawning", "");

			// Drawing
			properties.Set("draw_as_entity", false)
				.SetDocumentation("Draw as Entity", "", "", "Drawing", "", true, false);

			// Events
			events.AddEvent("moved", "Moved", "Movement", "Occurs when the tile is moved.");
		}
		
		public TileData(TileFlags flags) : this() {
			this.Flags = flags;
		}

		public TileData(Type type, TileFlags flags) : this() {
			this.type = type;
			this.Flags = flags;
		}

		public TileData(TileData copy) : base(copy) {
			spriteAbove			= copy.spriteAbove;
			spriteAsObject		= copy.spriteAsObject;
			breakAnimation		= copy.breakAnimation;
			breakSound			= copy.breakSound;
			spriteList			= new ISprite[copy.spriteList.Length];
			model				= copy.model;
			tileBelow			= copy.tileBelow;
			
			for (int i = 0; i < spriteList.Length; i++)
				spriteList[i] = copy.spriteList[i];
		}

		/// <summary>Clones the specified tile data.</summary>
		public override void Clone(BaseResourceData baseCopy) {
			base.Clone(baseCopy);

			TileData copy = (TileData) baseCopy;
			spriteAbove			= copy.spriteAbove;
			spriteAsObject		= copy.spriteAsObject;
			breakAnimation		= copy.breakAnimation;
			breakSound			= copy.breakSound;
			model				= copy.model;
			tileBelow			= copy.tileBelow;

			if (copy.spriteList.Length > 0) {
				spriteList = new ISprite[copy.spriteList.Length];
				for (int i = 0; i < spriteList.Length; i++) {
					spriteList[i] = copy.spriteList[i];
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public ISprite GetSpriteIndex(int index) {
			if (index < spriteList.Length)
				return spriteList[index];
			return null;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected override void InitializeData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, OutputType, Type, previousType);
		}

		/// <summary>Initializes data after a change in the final entity type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected override void InitializeEntityData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, typeof(Entity), EntityType, previousType);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public override Type OutputType {
			get { return typeof(Tile); }
		}

		/// <summary>Gets or sets the sprite of the tile data.</summary>
		public override ISprite Sprite {
			get {
				if (spriteList.Length > 0)
					return spriteList[0];
				return null;
			}
			set {
				if (value == null)
					spriteList = new ISprite[0];
				else
					spriteAsObject = value;
				spriteList = new ISprite[] { value };
			}
		}

		/// <summary>Gets or sets the size of the tile in pixels.
		/// Setter cannot be called for normal tiles.</summary>
		public override Point2I PixelSize {
			get { return TileSize * GameSettings.TILE_SIZE; }
			set {
				throw new NotImplementedException("Cannot call " +
					"TileData.Size setter!");
			}
		}

		/// <summary>Gets or sets the size of the tile in tiles.</summary>
		public override Point2I TileSize {
			get {
				return GMath.Max(Point2I.One, properties.Get<Point2I>("size",
					Point2I.One));
			}
			set { properties.Set("size", value); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ISprite[] SpriteList {
			get { return spriteList; }
			set { spriteList = value; }
		}

		public TileFlags Flags {
			get { return properties.GetEnum("flags", TileFlags.Default); }
			set { properties.SetEnum("flags", value); }
		}

		public ISprite SpriteAbove {
			get { return spriteAbove; }
			set { spriteAbove = value; }
		}

		public ISprite SpriteAsObject {
			get { return spriteAsObject; }
			set {
				if (value == null)
					spriteAsObject = null;
				else
					spriteAsObject = value;
			}
		}

		public DepthLayer BreakLayer {
			get { return breakLayer; }
			set { breakLayer = value; }
		}

		public Animation BreakAnimation {
			get { return breakAnimation; }
			set { breakAnimation = value; }
		}

		public Sound BreakSound {
			get { return breakSound; }
			set { breakSound = value; }
		}

		public CollisionModel CollisionModel {
			get { return model; }
			set { model = value; }
		}

		public TileEnvironmentType EnvironmentType {
			get { return properties.GetEnum("environment_type", TileEnvironmentType.Normal); }
			set { properties.SetEnum("environment_type", value); }
		}

		public TileSolidType SolidType {
			get { return properties.GetEnum("solidity", TileSolidType.NotSolid); }
			set { properties.SetEnum("solidity", value); }
		}

		public int LedgeDirection {
			get { return properties.Get<int>("ledge_direction", Direction.Down); }
			set { properties.Set("ledge_direction", value); }
		}

		public int ConveyorAngle {
			get { return properties.Get<int>("conveyor_angle", -1); }
			set { properties.Set("conveyor_angle", value); }
		}

		public float ConveyorSpeed {
			get { return properties.Get<float>("conveyor_speed", 0.0f); }
			set { properties.Set("conveyor_speed", value); }
		}

		public Rectangle2I HurtArea {
			get {
				return new Rectangle2I(
					properties.Get<Point2I>("hurt_area_point", new Point2I(-1, -1)),
					properties.Get<Point2I>("hurt_area_size", new Point2I(18, 18)));
			}
			set {
				properties.Set("hurt_area_point", value.Point);
				properties.Set("hurt_area_size", value.Size);
			}
		}

		public int HurtDamage {
			get { return properties.Get<int>("hurt_damage", 0); }
			set { properties.Set("hurt_damage", value); }
		}

		public bool DrawAsEntity {
			get { return properties.Get<bool>("draw_as_entity", false); }
			set { properties.Set("draw_as_entity", value); }
		}

		/// <summary>Gets or sets the tile to appear when
		/// this one is removed while on layer 1.</summary>
		public TileData TileBelow {
			get { return tileBelow; }
			set { tileBelow = value; }
		}
	}
}
