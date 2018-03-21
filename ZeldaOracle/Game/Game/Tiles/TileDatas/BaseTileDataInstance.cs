using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles {
	public abstract class BaseTileDataInstance : IEventObject, IIDObject {

		protected Room				room;
		protected BaseTileData		tileData;
		protected Properties		properties;			// The default properties for the tile.
		protected Properties		modifiedProperties; // The properties that tiles are spawned with.
		protected EventCollection   events;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public BaseTileDataInstance() {
			this.room				= null;
			this.tileData			= null;
			this.properties			= new Properties(this);
			this.modifiedProperties	= new Properties(this);
			this.events				= new EventCollection(this);
		}

		public BaseTileDataInstance(BaseTileData tileData) {
			this.room		= null;
			this.tileData	= tileData;
			this.properties							= new Properties(this);
			this.properties.BaseProperties			= tileData.Properties;
			this.modifiedProperties					= new Properties(this);
			this.modifiedProperties.BaseProperties	= this.properties;
			this.events								= new EventCollection(tileData.Events, this);
		}

		public virtual void Clone(BaseTileDataInstance copy) {
			this.room		= copy.Room;
			this.tileData	= copy.tileData;
			this.properties							= new Properties(copy.properties, this);
			this.properties.BaseProperties			= tileData.Properties;
			this.modifiedProperties					= new Properties(this);
			this.modifiedProperties.BaseProperties	= this.properties;
			this.events								= new EventCollection(copy.Events, this);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void ResetState() {
			// Copy the properties into the modified properties.
			//modifiedProperties.Clone(properties);
			modifiedProperties.Clear();
		}

		public void OverrideDefaultState() {
			// Copy the modified properties back into the default properties.
			properties.SetAll(modifiedProperties);
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods and properties
		//-----------------------------------------------------------------------------

		public abstract BaseTileDataInstance Duplicate();

		/// <summary>Gets or sets the pixel position of the base tile in the room.
		/// Setter only applies for actions.</summary>
		public abstract Point2I Position { get; set; }

		/// <summary>Gets the pixel position of the base tile in the level.</summary>
		public abstract Point2I LevelPosition { get; }

		/// <summary>Gets the pixel bounds of the base tile in the room.</summary>
		public abstract Rectangle2I Bounds { get; }

		/// <summary>Gets the pixel bounds of the base tile in the level.</summary>
		public abstract Rectangle2I LevelBounds { get; }

		/// <summary>Gets or sets the size of the base tile in pixels.
		/// Setter only applies for actions.</summary>
		public abstract Point2I PixelSize { get; set; }

		/// <summary>Gets or sets the size of the base tile in tiles.</summary>
		public abstract Point2I TileSize { get; set; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Room Room {
			get { return room; }
			set { room = value; }
		}

		public BaseTileData BaseData {
			get { return tileData; }
			set {
				tileData = value;
				if (tileData == null) {
					properties.BaseProperties = null;
					events = new EventCollection(this);
				}
				else {
					properties.BaseProperties = tileData.Properties;
					events = new EventCollection(value.Events, this);
				}
			}
		}
		
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}
		
		public Properties ModifiedProperties {
			get { return modifiedProperties; }
		}

		public EventCollection Events {
			get { return events; }
			set { events = value; }
		}

		public Properties BaseProperties {
			get { return tileData.Properties; }
		}

		/// <summary>Gets the overridden type fof the tile.</summary>
		public Type Type {
			get { return tileData.Type; }
		}

		/// <summary>Gets the type of entity this tile spawns.</summary>
		public Type EntityType {
			get { return tileData.EntityType; }
		}

		public Point2I SheetLocation {
			get { return tileData.SheetLocation; }
		}
		
		public Tileset Tileset {
			get { return tileData.Tileset; }
		}

		public bool HasModifiedProperties {
			get { return properties.HasModifiedProperties; }
		}

		public bool HasModifiedModifiedProperties {
			get { return modifiedProperties.HasModifiedProperties; }
		}

		public bool HasDefinedEvents {
			get { return events.HasDefinedEvents; }
		}

		public bool HasPreviewSprite {
			get { return tileData.HasPreviewSprite; }
		}

		public ISprite PreviewSprite {
			get { return tileData.PreviewSprite; }
		}

		public string ID {
			get { return properties.GetString("id", ""); }
		}
		
		public TileResetCondition ResetCondition {
			get { return properties.GetEnum("reset_condition",
				TileResetCondition.LeaveRoom); }
			set { properties.SetEnum("reset_condition", value); }
		}

		/// <summary>Gets or sets if the tile is shared between child rooms.</summary>
		public bool IsShared {
			get { return properties.Get("shared", false); }
			set { properties.Set("shared", value); }
		}

		/// <summary>Gets or sets if the tile is enabled and spawns during room start.</summary>
		public bool IsEnabled {
			get { return properties.Get("enabled", true); }
			set { properties.Set("enabled", value); }
		}

		/// <summary>Gets or sets if the tile is enabled in the modified properties.</summary>
		public bool IsModifiedEnabled {
			get { return modifiedProperties.Get("enabled", true); }
			set { modifiedProperties.Set("enabled", value); }
		}
		
		// Special Tiles --------------------------------------------------------------

		/// <summary>Returns true if the tile should be treated as a monster.</summary>
		public bool IsMonster {
			get { return properties.Contains("monster_id"); }
		}

		/// <summary>Gets or sets the ID unique to each monster in the room.</summary>
		public int MonsterID {
			get { return modifiedProperties.Get("monster_id", 0); }
			set { modifiedProperties.Set("monster_id", value); }
		}

		/// <summary>Gets if the tile is an unlooted reward tile.</summary>
		public bool IsUnlootedReward {
			get { return properties.ContainsEquals("looted", false); }
		}

		/// <summary>Gets or sets if the tile is looted.</summary>
		public bool IsLooted {
			get { return modifiedProperties.Get("looted", false); }
			set { modifiedProperties.Set("looted", value); }
		}
	}
}
