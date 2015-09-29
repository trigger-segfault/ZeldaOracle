using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Properties;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	
	public class EventTileData {

		private Type			type;
		private Point2I			size;
		private Properties		properties;
		private Properties		modifiedProperties;
		private Point2I			position;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EventTileData() {
			type				= null;
			position			= Point2I.Zero;
			size				= Point2I.One;
			properties			= new Properties();
			modifiedProperties	= new Properties();
		}
		
		public EventTileData(Type type) : this() {
			this.type = type;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Type Type {
			get { return type; }
			set { type = value; }
		}
		
		public Point2I Position {
			get { return position; }
			set { position = value; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		public Properties ModifiedProperties {
			get { return modifiedProperties; }
		}
	}
}
