using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	
	public class EventTileDataInstance : IPropertyObject {

		private Room			room;
		private Point2I			position;
		private EventTileData	data;
		private Properties		modifiedProperties; // Properties modified from the tiledata
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public EventTileDataInstance() {
			this.room				= null;
			this.position			= Point2I.Zero;
			this.data				= null;
			this.modifiedProperties	= new Properties();
			this.modifiedProperties.PropertyObject = this;
		}

		public EventTileDataInstance(EventTileData tileData, Point2I position) {
			this.room				= null;
			this.position			= position;
			this.data				= tileData;
			this.modifiedProperties	= new Properties();
			this.modifiedProperties.PropertyObject = this;
			this.modifiedProperties.BaseProperties = tileData.Properties;
		}
		

		//-----------------------------------------------------------------------------
		// Instance Properties
		//-----------------------------------------------------------------------------
		
		public EventTileData EventTileData {
			get { return data; }
			set { data = value; }
		}
		
		public Room Room {
			get { return room; }
			set { room = value; }
		}
		
		public Point2I Position {
			get { return position; }
			set { position = value; }
		}

		public Properties Properties {
			get { return modifiedProperties; }
			set {
				modifiedProperties = value;
				modifiedProperties.PropertyObject = this;
			}
		}


		//-----------------------------------------------------------------------------
		// Data Properties
		//-----------------------------------------------------------------------------

		public Type Type {
			get { return data.Type; }
		}
		
		public Point2I Size {
			get { return data.Size; }
		}

		public Properties BaseProperties {
			get { return data.Properties; }
		}

		public Sprite Sprite {
			get { return data.Sprite; }
		}
	}
}
