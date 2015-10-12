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

namespace ZeldaOracle.Game.Tiles {
	public class TileDataInstance : IPropertyObject {

		private Room		room;
		private Point2I		location;
		private int			layer;
		private TileData	tileData;
		private Properties	properties;


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

		public TileDataInstance(TileData tileData, int x, int y, int layer) {
			this.room		= null;
			this.location	= new Point2I(x, y);
			this.layer		= layer;
			this.tileData	= tileData;
			this.properties = new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = tileData.Properties;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Room Room {
			get { return room; }
			set { room = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
		}
		
		public int Layer {
			get { return layer; }
			set { layer = value; }
		}

		public TileData TileData {
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
		
		public Type Type {
			get { return tileData.Type; }
		}
		
		public Point2I Size {
			get { return tileData.Size; }
		}

		public TileFlags Flags {
			get { return tileData.Flags; }
		}

		public SpriteAnimation Sprite {
			get { return tileData.Sprite; }
		}

		public SpriteAnimation CurrentSprite {
			get {
				if (tileData.SpriteList.Length > 0)
					return tileData.SpriteList[properties.GetInteger("sprite_index")];
				return new SpriteAnimation();
			}
		}

		public SpriteAnimation[] SpriteList {
			get { return tileData.SpriteList; }
		}

		public SpriteAnimation SpriteAsObject {
			get { return tileData.SpriteAsObject; }
		}

		public Animation BreakAnimation {
			get { return tileData.BreakAnimation; }
		}

		public CollisionModel CollisionModel {
			get { return tileData.CollisionModel; }
		}

		public Point2I SheetLocation {
			get { return tileData.SheetLocation; }
		}
		
		public Tileset Tileset {
			get { return TileData.Tileset; }
		}

		public Properties BaseProperties {
			get { return TileData.Properties; }
		}
	}
}
