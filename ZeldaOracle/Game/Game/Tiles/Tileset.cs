using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {

	public class Tileset {
		
		private TileData[,]	tileData;
		private Point2I		size;
		//private GridSheet	sheet;
		private Point2I		defaultTile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Tileset(int width, int height) {
			this.size			= new Point2I(width, height);
			this.defaultTile	= Point2I.Zero;
			this.tileData		= new TileData[width, height];
		}
		

		//-----------------------------------------------------------------------------
		// Functions
		//----------------------------------------------------------------------------

		public Tile CreateTile(Point2I sheetLocation) {
			TileData data = tileData[sheetLocation.X, sheetLocation.Y];
			Tile tile = new Tile();

			// TODO: needs to instantiate other tile sub-class types.
			
			tile.Flags			= data.Flags;
			tile.Sprite			= data.Sprite;
			tile.CollisionModel	= data.CollisionModel;
			tile.Size			= data.Size;
			tile.AnimationPlayer.Animation = data.Animation;

			return tile;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileData[,] TileData {
			get { return tileData; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}
		
		public int Width {
			get { return size.X; }
			set { size.X = value; }
		}
		
		public int Height {
			get { return size.Y; }
			set { size.Y = value; }
		}
		
		public Point2I DefaultTile {
			get { return defaultTile; }
			set { defaultTile = value; }
		}

	}
}
