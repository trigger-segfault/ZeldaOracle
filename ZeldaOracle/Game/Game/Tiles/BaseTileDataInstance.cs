using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	public abstract class BaseTileDataInstance : IPropertyObject, IEventObject {

		protected Room				room;
		protected BaseTileData		tileData;
		protected Properties		properties;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public BaseTileDataInstance() {
			room		= null;
			tileData	= null;
			properties	= new Properties(this);
		}

		public BaseTileDataInstance(BaseTileData tileData) {
			this.room		= null;
			this.tileData	= tileData;
			this.properties	= new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = tileData.Properties;
		}


		//-----------------------------------------------------------------------------
		// Virtual properties
		//-----------------------------------------------------------------------------

		public abstract SpriteAnimation CurrentSprite { get; }

		public abstract SpriteAnimation Sprite { get; set; }


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
				if (tileData == null)
					properties.BaseProperties = null;
				else
					properties.BaseProperties = tileData.Properties;
			}
		}
		
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		public ObjectEventCollection Events {
			get { return tileData.Events; }
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

		public string ID {
			get { return properties.GetString("id", ""); }
		}
	}
}
