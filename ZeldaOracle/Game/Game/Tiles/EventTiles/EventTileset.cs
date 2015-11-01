using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public class EventTileset : BaseTileset<EventTileData> {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public EventTileset(string id, SpriteSheet sheet, Point2I size) :
			this(id, sheet, size.X, size.Y)
		{
		}

		public EventTileset(string id, SpriteSheet sheet, int width, int height) :
			base(id, sheet, width, height)
		{
		}
		

		//----------------------------------------------------------------------------
		// Overridden Methods
		//----------------------------------------------------------------------------

		protected override EventTileData CreateDefaultTileData(Point2I location) {
			return null;
		}

	}
}
