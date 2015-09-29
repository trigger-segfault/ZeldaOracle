using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	public class TileDataInstance {

		private Room		room;
		private Point2I		location;
		private int			layer;
		private TileData	tileData;
		private Properties	modifiedProperties; // Properties modified from the tiledata


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public TileDataInstance() {
			this.room		= null;
			this.location	= Point2I.Zero;
			this.layer		= 0;
			this.tileData	= null;
			this.modifiedProperties = new Properties();
		}

		public TileDataInstance(TileData tileData, int x, int y, int layer) {
			this.room		= null;
			this.location	= new Point2I(x, y);
			this.layer		= layer;
			this.tileData	= tileData;
			this.modifiedProperties = new Properties();
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
			set { tileData = value; }
		}
		
		public Properties ModifiedProperties {
			get { return modifiedProperties; }
			set { modifiedProperties = value; }
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

		public Sprite Sprite {
			get { return tileData.Sprite; }
		}

		public Sprite SpriteAsObject {
			get { return tileData.SpriteAsObject; }
		}

		public Animation Animation {
			get { return tileData.Animation; }
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
