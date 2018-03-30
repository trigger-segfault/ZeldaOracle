using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	public class ActionChangeTileSizeProperty : ActionChangeProperty {

		private Dictionary<Point2I, TileDataInstance> overwrittenTiles;
		

		public ActionChangeTileSizeProperty(TileDataInstance tileData, Property property, Point2I oldSize, Point2I newSize) :
			base(tileData, property, oldSize, newSize)
		{
			overwrittenTiles = new Dictionary<Point2I, TileDataInstance>();
			if (tileData.Room.ContainsTile(tileData)) {
				foreach (TileDataInstance tile in tileData.Room.GetTilesInArea(
					new Rectangle2I(tileData.Location, tileData.TileSize), tileData.Layer))
				{
					if (tile != tileData)
						overwrittenTiles.Add(tile.Location, tile);
				}
			}
		}

		public override void Undo(EditorControl editorControl) {
			base.Undo(editorControl);
			TileData.Room.UpdateTileSize(TileData, NewSize);
			foreach (var pair in overwrittenTiles) {
				TileData.Room.PlaceTile(pair.Value, pair.Key, TileData.Layer);
			}
		}

		public override void Redo(EditorControl editorControl) {
			base.Redo(editorControl);
			TileData.Room.UpdateTileSize(TileData, OldSize);
		}

		public TileDataInstance TileData {
			get { return (TileDataInstance) PropertyObject; }
		}

		public Point2I OldSize {
			get { return (Point2I) OldValue; }
			set { OldValue = value; }
		}

		public Point2I NewSize {
			get { return (Point2I) NewValue; }
			set { NewValue = value; }
		}
	}
}
