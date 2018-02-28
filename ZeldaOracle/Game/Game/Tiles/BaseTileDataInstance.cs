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
			this.properties							= new Properties(this);
			this.properties.BaseProperties			= tileData.Properties;
			this.modifiedProperties					= new Properties(this);
			this.modifiedProperties.BaseProperties	= tileData.Properties;
			this.properties.SetAll(copy.properties);
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
			properties.Clone(modifiedProperties);
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods and properties
		//-----------------------------------------------------------------------------

		public abstract BaseTileDataInstance Duplicate();

		public abstract Point2I GetPosition();
		
		public abstract Rectangle2I GetBounds();

		public abstract ISprite CurrentSprite { get; }

		public abstract ISprite Sprite { get; set; }


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
		
		public Type Type {
			get { return tileData.Type; }
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

		public bool HasDefinedEvents {
			get { return events.HasDefinedEvents; }
		}

		public bool HasModifiedModifiedProperties {
			get { return modifiedProperties.HasModifiedProperties; }
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
			get { return properties.GetEnum("reset_condition", TileResetCondition.LeaveRoom); }
			set { properties.Set("reset_condition", (int) value); }
		}
	}
}
