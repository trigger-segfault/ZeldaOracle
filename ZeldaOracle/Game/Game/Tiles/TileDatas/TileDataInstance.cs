using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using System;

namespace ZeldaOracle.Game.Tiles {
	public class TileDataInstance : BaseTileDataInstance {

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
		}

		public TileDataInstance(TileData tileData) :
			this(tileData, -Point2I.One, -1) {
		}

		public TileDataInstance(TileData tileData, int x, int y, int layer) :
			this(tileData, new Point2I(x, y), layer) { }

		public TileDataInstance(TileData tileData, Point2I location, int layer) :
			base(tileData)
		{
			this.location	= location;
			this.layer		= layer;
		}

		public TileDataInstance(TileDataInstance tile) :
			this()
		{
			Clone(tile);
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

		public bool IsAtLocation(Point2I location) {
			return (this.location == location);
		}

		public bool IsAtLocation(int x, int y) {
			return (x == location.X && y == location.Y);
		}

		/// <summary>Returns true if this tile contains the tile location in the room.</summary>
		public bool ContainsLocation(Point2I location) {
			return new Rectangle2I(Location, TileSize).Contains(location);
		}

		/// <summary>Returns true if this tile contains the tile location in the room.</summary>
		public bool ContainsLocation(int x, int y) {
			return ContainsLocation(new Point2I(x, y));
		}

		/// <summary>Returns true if this tile contains the tile location in the level.</summary>
		public bool ContainsLevelCoord(Point2I location) {
			return new Rectangle2I(LevelCoord, TileSize).Contains(location);
		}

		/// <summary>Returns true if this tile contains the tile location in the level.</summary>
		public bool ContainsLevelCoord(int x, int y) {
			return ContainsLevelCoord(new Point2I(x, y));
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

		/// <summary>Gets or sets the pixel position of the tile in the room.
		/// Setter cannot be called for normal tiles.</summary>
		public override Point2I Position {
			get { return location * GameSettings.TILE_SIZE; }
			set {
				throw new NotImplementedException("Cannot call " +
					"TileDataInstance.Position setter!");
			}
		}

		/// <summary>Gets the pixel position of the tile in the level.</summary>
		public override Point2I LevelPosition {
			get { return room.LevelPosition + Position; }
		}

		/// <summary>Gets the pixel bounds of the tile in the room.</summary>
		public override Rectangle2I Bounds {
			get { return new Rectangle2I(Position, PixelSize); }
		}

		/// <summary>Gets the pixel bounds of the tile in the level.</summary>
		public override Rectangle2I LevelBounds {
			get { return new Rectangle2I(LevelPosition, PixelSize); }
		}

		/// <summary>Gets or sets the size of the tile in pixels.
		/// Setter cannot be called for normal tiles.</summary>
		public override Point2I PixelSize {
			get { return TileSize * GameSettings.TILE_SIZE; }
			set {
				throw new NotImplementedException("Cannot call " +
					"TileDataInstance.Size setter!");
			}
		}

		/// <summary>Gets or sets the size of the tile in tiles.</summary>
		public override Point2I TileSize {
			get {
				return GMath.Max(Point2I.One, properties.GetPoint("size",
					Point2I.One));
			}
			set { properties.Set("size", value); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the TileData this instance was constructed from.</summary>
		public TileData TileData {
			get { return (tileData as TileData); }
			set { base.BaseData = value; }
		}

		/// <summary>Gets or sets the location of the tile in the room.</summary>
		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		/// <summary>Gets the coordinates of the tile in tiles from the start of the
		/// level.</summary>
		public Point2I LevelCoord {
			get { return room.LevelCoord + location; }
		}

		/// <summary>Gets the boundaries of the tile in the room in tiles.</summary>
		public Rectangle2I TileBounds {
			get { return new Rectangle2I(location, TileSize); }
		}

		/// <summary>Gets the boundaries of the tile in the level in tiles.</summary>
		public Rectangle2I LevelTileBounds {
			get { return new Rectangle2I(LevelCoord, TileSize); }
		}

		/// <summary>Gets or sets the layer this tile is located on.</summary>
		public int Layer {
			get { return layer; }
			set { layer = value; }
		}

		public ISprite[] SpriteList {
			get { return TileData.SpriteList; }
		}

		public ISprite SpriteAsObject {
			get { return TileData.SpriteAsObject; }
		}

		public DepthLayer BreakLayer {
			get { return TileData.BreakLayer; }
		}

		public Animation BreakAnimation {
			get { return TileData.BreakAnimation; }
		}

		public Sound BreakSound {
			get { return TileData.BreakSound; }
		}

		public TileFlags Flags {
			get { return properties.GetEnum("flags", TileFlags.Default); }
			set { properties.SetEnum("flags", value); }
		}

		public CollisionModel CollisionModel {
			get { return TileData.CollisionModel; }
			set { TileData.CollisionModel = value; }
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
			get { return properties.GetEnum("solidity", TileSolidType.NotSolid); }
			set { properties.SetEnum("solidity", value); }
		}

		public bool IsSolid {
			get { return SolidType != TileSolidType.NotSolid; }
		}

		public int ConveyorAngle {
			get { return properties.GetInteger("conveyor_angle", -1); }
			set { properties.Set("conveyor_angle", value); }
		}

		public float ConveyorSpeed {
			get { return properties.GetFloat("conveyor_speed", 0.0f); }
			set { properties.Set("conveyor_speed", value); }
		}

		public bool DrawAsEntity {
			get { return TileData.DrawAsEntity; }
		}

		public Rectangle2I HurtArea {
			get {
				return new Rectangle2I(
					properties.GetPoint("hurt_area_point", new Point2I(-1, -1)),
					properties.GetPoint("hurt_area_size", new Point2I(18, 18)));
			}
			set {
				properties.Set("hurt_area_point", value.Point);
				properties.Set("hurt_area_size", value.Size);
			}
		}

		public int HurtDamage {
			get { return properties.GetInteger("hurt_damage", 0); }
			set { properties.Set("hurt_damage", value); }
		}

		/// <summary>Gets the tile to appear when
		/// this one is removed while on layer 1.</summary>
		public TileData TileBelow {
			get { return TileData.TileBelow; }
		}
	}
}
