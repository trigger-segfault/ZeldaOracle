using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Tiles {

	public class Tileset : BaseTileset<TileData> {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Tileset(string id, SpriteSheet sheet, Point2I size) :
			this(id, sheet, size.X, size.Y)
		{
		}

		public Tileset(string id, SpriteSheet sheet, int width, int height) :
			base(id, sheet, width, height)
		{
		}
		

		//----------------------------------------------------------------------------
		// Functions
		//----------------------------------------------------------------------------

		// Configure a tile using single characters that represent basic tile types.
		public void ConfigureTile(TileData data, char configChar) {
			switch (configChar) {
				case 'S':
					data.Flags |= TileFlags.Solid;
					data.CollisionModel = GameData.MODEL_BLOCK;
					break;
				case 'F':
					data.Flags |= TileFlags.Waterfall | TileFlags.Solid | TileFlags.LedgeDown;
					data.CollisionModel = GameData.MODEL_BLOCK;
					break;
				case 'G': data.Flags |= TileFlags.Diggable;		break;
				case 'H': data.Flags |= TileFlags.Hole;			break;
				case 'V': data.Flags |= TileFlags.Lava;			break;
				case 'W': data.Flags |= TileFlags.Water;		break;
				case 'I': data.Flags |= TileFlags.Ice;			break;
				case 'R': data.Flags |= TileFlags.Stairs;		break;
				case 'D': data.Flags |= TileFlags.Ladder;		break;
				case 'A': data.Flags |= TileFlags.HalfSolid;	break;
				case 'P': data.Flags |= TileFlags.Puddle;		break;
				case 'O': data.Flags |= TileFlags.Water | TileFlags.Ocean; break;
			}
		}
		

		//----------------------------------------------------------------------------
		// Overridden Methods
		//----------------------------------------------------------------------------

		protected override TileData CreateDefaultTileData(Point2I location) {
			TileData tileData		= new TileData();
			tileData.Tileset		= this;
			tileData.SheetLocation	= location;
			tileData.Sprite			= new Sprite(sheet, location);
			return tileData;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
	}
}
