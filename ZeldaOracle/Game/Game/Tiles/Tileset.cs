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
					data.SolidType = TileSolidType.Solid;
					data.CollisionModel = GameData.MODEL_BLOCK;
					break;
				case 'F':
					data.EnvironmentType	= TileEnvironmentType.Waterfall;
					data.LedgeDirection		= Directions.Down;
					data.CollisionModel		= GameData.MODEL_BLOCK;
					break;
				case 'G': data.Flags |= TileFlags.Digable; break;
				case 'H': data.EnvironmentType = TileEnvironmentType.Hole; break;
				case 'V': data.EnvironmentType = TileEnvironmentType.Lava; break;
				case 'W': data.EnvironmentType = TileEnvironmentType.Water; break;
				case 'I': data.EnvironmentType = TileEnvironmentType.Ice; break;
				case 'R': data.EnvironmentType = TileEnvironmentType.Stairs; break;
				case 'D': data.EnvironmentType = TileEnvironmentType.Ladder; break;
				case 'A': data.SolidType = TileSolidType.HalfSolid; break;
				case 'P': data.EnvironmentType = TileEnvironmentType.Puddle; break;
				case 'O': data.EnvironmentType = TileEnvironmentType.Ocean; break;
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
