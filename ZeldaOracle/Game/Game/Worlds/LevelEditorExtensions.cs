using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Worlds.Editing {
	/// <summary>A static class for defining level extensions for use with the editor.</summary>
	public static class LevelEditorExtensions {

		//-----------------------------------------------------------------------------
		// Coordinate/Position Conversion
		//-----------------------------------------------------------------------------

		// Level -> Room --------------------------------------------------------------

		/// <summary>Converts the level position to a room position and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelToRoomPosition(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I position = GMath.Wrap(levelPosition, level.RoomPixelSize);
			if (clamp)
				return GMath.Clamp(position, level.PixelBounds);
			return position;
		}

		/// <summary>Converts the level position to a room position and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelToRoomPosition(this Level level,
			Point2I levelPosition, out Room room, bool clamp = false)
		{
			Point2I position = GMath.Wrap(levelPosition, level.RoomPixelSize);
			if (clamp)
				position = GMath.Clamp(position, level.PixelBounds);
			room = level.GetRoomAt(GMath.FloorDiv(levelPosition,
				level.RoomPixelSize));
			return position;
		}

		/// <summary>Converts the level coord to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelToRoomCoord(this Level level, Point2I levelCoord,
			bool clamp = false)
		{
			Point2I coord = GMath.Wrap(levelCoord, level.RoomSize);
			if (clamp)
				return GMath.Clamp(coord, level.TileBounds);
			return coord;
		}

		/// <summary>Converts the level coord to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelToRoomCoord(this Level level, Point2I levelCoord,
			out Room room, bool clamp = false)
		{
			Point2I coord = GMath.Wrap(levelCoord, level.RoomSize);
			if (clamp)
				coord = GMath.Clamp(coord, level.TileBounds);
			room = level.GetRoomAt(GMath.FloorDiv(levelCoord, level.RoomSize));
			return coord;
		}

		/// <summary>Converts the level position to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelPositionToRoomCoord(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I coord = GMath.Wrap(levelPosition, level.RoomSize);
			coord = GMath.FloorDiv(coord, GameSettings.TILE_SIZE);
			if (clamp)
				return GMath.Clamp(coord, level.TileBounds);
			return coord;
		}

		/// <summary>Converts the level position to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelPositionToRoomCoord(this Level level,
			Point2I levelPosition, out Room room, bool clamp = false)
		{
			Point2I coord = GMath.Wrap(levelPosition, level.RoomPixelSize);
			coord = GMath.FloorDiv(coord, GameSettings.TILE_SIZE);
			if (clamp)
				coord = GMath.Clamp(coord, level.TileBounds);
			room = level.GetRoomAt(GMath.FloorDiv(levelPosition, level.RoomPixelSize));
			return coord;
		}

		/// <summary>Converts the level position to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelCoordToRoomPosition(this Level level,
			Point2I levelCoord, bool clamp = false)
		{
			Point2I position = GMath.Wrap(levelCoord, level.RoomSize) *
				GameSettings.TILE_SIZE;
			if (clamp)
				return GMath.Clamp(position, level.PixelBounds);
			return position;
		}

		/// <summary>Converts the level position to a room coord and optionally
		/// clamps to be within the level boundaries.</summary>
		public static Point2I LevelCoordToRoomPosition(this Level level,
			Point2I levelCoord, out Room room, bool clamp = false)
		{
			Point2I position = GMath.Wrap(levelCoord, level.RoomSize) *
				GameSettings.TILE_SIZE;
			if (clamp)
				position = GMath.Clamp(position, level.PixelBounds);
			room = level.GetRoomAt(GMath.FloorDiv(levelCoord,
				level.RoomSize));
			return position;
		}

		// Room -> Level --------------------------------------------------------------

		/// <summary>Converts the room location to a level position.</summary>
		public static Point2I RoomToLevelPosition(this Level level,
			Point2I roomLocation)
		{
			return roomLocation * level.RoomSize * GameSettings.TILE_SIZE;
		}

		/// <summary>Converts the room location to a level coord.</summary>
		public static Point2I RoomToLevelCoord(this Level level,
			Point2I roomLocation)
		{
			return roomLocation * level.RoomSize;
		}

		/// <summary>Converts the room location and position to a level position.</summary>
		public static Point2I RoomToLevelPosition(this Level level,
			Point2I roomLocation, Point2I roomPosition)
		{
			return roomLocation * level.RoomSize * GameSettings.TILE_SIZE +
				roomPosition;
		}

		/// <summary>Converts the room location and coord to a level coord.</summary>
		public static Point2I RoomToLevelCoord(this Level level, Point2I roomLocation,
			Point2I roomCoord)
		{
			return roomLocation * level.RoomSize + roomCoord;
		}

		/// <summary>Converts the room position to a level position.</summary>
		public static Point2I RoomToLevelPosition(this Level level, Room room,
			Point2I roomPosition)
		{
			return room.LevelPosition + roomPosition;
		}

		/// <summary>Converts the room coord to a level coord.</summary>
		public static Point2I RoomToLevelCoord(this Level level, Room room,
			Point2I roomCoord)
		{
			return room.LevelCoord + roomCoord;
		}

		// Level -> Room Object/Location ----------------------------------------------

		/// <summary>Gets the room at the specified level position.</summary>
		public static Room LevelPositionToRoom(this Level level,
			Point2I levelPosition)
		{
			return level.GetRoomAt(GMath.FloorDiv(levelPosition,
				GameSettings.TILE_SIZE, level.RoomSize));
		}

		/// <summary>Gets the room at the specified level coord.</summary>
		public static Room LevelCoordToRoom(this Level level, Point2I levelCoord) {
			return level.GetRoomAt(GMath.FloorDiv(levelCoord, level.RoomSize));
		}

		/// <summary>Gets the room location at the specified level position.</summary>
		public static Point2I LevelPositionToRoomLocation(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelPosition,
				GameSettings.TILE_SIZE, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location;
		}
		
		/// <summary>Gets the room location at the specified level position.</summary>
		public static Rectangle2I LevelPositionToRoomLocation(this Level level,
			Rectangle2I levelPosition, bool clamp = false)
		{
			Point2I location1 = GMath.FloorDiv(levelPosition.Min,
				GameSettings.TILE_SIZE, level.RoomSize);
			Point2I location2 = GMath.CeilingDiv(levelPosition.Max,
				GameSettings.TILE_SIZE, level.RoomSize);
			if (clamp) {
				location1 = GMath.Clamp(location1, level.RoomBounds);
				location2 = GMath.Clamp(location2, level.RoomBounds);
			}
			return Rectangle2I.FromEndPoints(location1, location2);
		}

		/// <summary>Gets the room location at the specified level coord.</summary>
		public static Point2I LevelCoordToRoomLocation(this Level level,
			Point2I levelCoord, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelCoord, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location;
		}

		/// <summary>Gets the room location at the specified level coord.</summary>
		public static Rectangle2I LevelCoordToRoomLocation(this Level level,
			Rectangle2I levelCoord, bool clamp = false)
		{
			Point2I location1 = GMath.FloorDiv(levelCoord.Min, level.RoomSize);
			Point2I location2 = GMath.CeilingDiv(levelCoord.Max, level.RoomSize);
			if (clamp) {
				location1 = GMath.Clamp(location1, level.RoomBounds);
				location2 = GMath.Clamp(location2, level.RoomBounds);
			}
			return Rectangle2I.FromEndPoints(location1, location2);
		}

		// Room Location -> Level -----------------------------------------------------

		/// <summary>Converts the room location to a level position.</summary>
		public static Point2I RoomLocationToLevelPosition(this Level level,
			Point2I roomLocation, bool clamp = false)
		{
			if (clamp)
				roomLocation = GMath.Clamp(roomLocation, level.RoomBounds);
			return roomLocation * level.RoomSize * GameSettings.TILE_SIZE;
		}
		
		/// <summary>Converts the room location to a level position.</summary>
		public static Rectangle2I RoomLocationToLevelPosition(this Level level,
			Rectangle2I roomLocation, bool clamp = false)
		{
			if (clamp) {
				Point2I min = GMath.Clamp(roomLocation.Min, level.RoomBounds);
				Point2I max = GMath.Clamp(roomLocation.Max, level.RoomBounds);
				roomLocation = Rectangle2I.FromEndPoints(min, max);
			}
			return roomLocation * level.RoomSize * GameSettings.TILE_SIZE;
		}
		
		/// <summary>Converts the room location to a level coord.</summary>
		public static Point2I RoomLocationToLevelCoord(this Level level,
			Point2I roomLocation, bool clamp = false)
		{
			if (clamp)
				roomLocation = GMath.Clamp(roomLocation, level.RoomBounds);
			return roomLocation * level.RoomSize;
		}
		
		/// <summary>Converts the room location to a level coord.</summary>
		public static Rectangle2I RoomLocationToLevelCoord(this Level level,
			Rectangle2I roomLocation, bool clamp = false)
		{
			if (clamp) {
				Point2I min = GMath.Clamp(roomLocation.Min, level.RoomBounds);
				Point2I max = GMath.Clamp(roomLocation.Max, level.RoomBounds);
				roomLocation = Rectangle2I.FromEndPoints(min, max);
			}
			return roomLocation * level.RoomSize;
		}

		// Postion <-> Coord ----------------------------------------------------------

		/// <summary>Converts the level position to a level coord.</summary>
		public static Point2I LevelPositionToCoord(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I levelCoord = GMath.FloorDiv(levelPosition, GameSettings.TILE_SIZE);
			if (clamp)
				return GMath.Clamp(levelCoord, level.TileBounds);
			return levelCoord;
		}

		/// <summary>Converts the level position to a level coord.</summary>
		public static Rectangle2I LevelPositionToCoord(this Level level,
			Rectangle2I levelPosition, bool clamp = false)
		{
			Point2I levelCoord1 = GMath.FloorDiv(levelPosition.Min,
				GameSettings.TILE_SIZE);
			Point2I levelCoord2 = GMath.CeilingDiv(levelPosition.Max,
				GameSettings.TILE_SIZE);
			if (clamp) {
				levelCoord1 = GMath.Clamp(levelCoord1, level.TileBounds);
				levelCoord2 = GMath.Clamp(levelCoord2, level.TileBounds);
			}
			return Rectangle2I.FromEndPoints(levelCoord1, levelCoord2);
		}

		/// <summary>Converts the level coord to a level position.</summary>
		public static Point2I LevelCoordToPosition(this Level level,
			Point2I levelCoord, bool clamp = false)
		{
			Point2I levelPosition = levelCoord * GameSettings.TILE_SIZE;
			if (clamp)
				return GMath.Clamp(levelPosition, level.PixelBounds);
			return levelPosition;
		}

		/// <summary>Converts the level coord to a level position.</summary>
		public static Rectangle2I LevelCoordToPosition(this Level level,
			Rectangle2I levelCoord, bool clamp = false)
		{
			Rectangle2I levelPosition = levelCoord * GameSettings.TILE_SIZE;
			if (clamp) {
				Point2I min = GMath.Clamp(levelPosition.Min, level.PixelBounds);
				Point2I max = GMath.Clamp(levelPosition.Max, level.PixelBounds);
				return Rectangle2I.FromEndPoints(min, max);
			}
			return levelPosition;
		}

		// Postion/Coord -> Room ------------------------------------------------------

		/// <summary>Gets the room's level position from the level position.</summary>
		public static Point2I LevelToRoomLocationPosition(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelPosition,
				GameSettings.TILE_SIZE, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location * level.RoomSize * GameSettings.TILE_SIZE;
		}
		
		/// <summary>Gets the room's level position from the level position.</summary>
		public static Rectangle2I LevelToRoomLocationPosition(this Level level,
			Rectangle2I levelPosition, bool clamp = false)
		{
			Point2I position1 = GMath.FloorI(levelPosition.Min,
				GameSettings.TILE_SIZE * level.RoomSize);
			Point2I position2 = GMath.CeilingI(levelPosition.Max,
				GameSettings.TILE_SIZE * level.RoomSize);
			if (clamp) {
				position1 = GMath.Clamp(position1, level.PixelBounds);
				position2 = GMath.Clamp(position2, level.PixelBounds);
			}
			return Rectangle2I.FromEndPoints(position1, position2);
		}

		/// <summary>Gets the room's level coord from the level coord.</summary>
		public static Point2I LevelToRoomLocationCoord(this Level level,
			Point2I levelCoord, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelCoord, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location * level.RoomSize;
		}

		/// <summary>Gets the room's level coord from the level coord.</summary>
		public static Rectangle2I LevelToRoomLocationCoord(this Level level,
			Rectangle2I levelCoord, bool clamp = false)
		{
			Point2I coord1 = GMath.FloorI(levelCoord.Min, level.RoomSize);
			Point2I coord2 = GMath.CeilingI(levelCoord.Max, level.RoomSize);
			if (clamp) {
				coord1 = GMath.Clamp(coord1, level.TileBounds);
				coord2 = GMath.Clamp(coord2, level.TileBounds);
			}
			return Rectangle2I.FromEndPoints(coord1, coord2);
		}

		/// <summary>Gets the room's level coord from the level position.</summary>
		public static Point2I LevelPositionToRoomLocationCoord(this Level level,
			Point2I levelPosition, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelPosition,
				GameSettings.TILE_SIZE, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location * level.RoomSize;
		}

		/// <summary>Gets the room's level coord from the level position.</summary>
		public static Rectangle2I LevelPositionToRoomLocationCoord(this Level level,
			Rectangle2I levelPosition, bool clamp = false)
		{
			Point2I location1 = GMath.FloorDiv(levelPosition.Max,
				GameSettings.TILE_SIZE, level.RoomSize);
			Point2I location2 = GMath.CeilingDiv(levelPosition.Min,
				GameSettings.TILE_SIZE, level.RoomSize);
			if (clamp) {
				location1 = GMath.Clamp(location1, level.RoomBounds);
				location2 = GMath.Clamp(location2, level.RoomBounds);
			}
			Rectangle2I location = Rectangle2I.FromEndPoints(location1, location2);
			return location * level.RoomSize;
		}

		/// <summary>Gets the room's level position from the level coord.</summary>
		public static Point2I LevelCoordToRoomLocationPosition(this Level level,
			Point2I levelCoord, bool clamp = false)
		{
			Point2I location = GMath.FloorDiv(levelCoord, level.RoomSize);
			if (clamp)
				return GMath.Clamp(location, level.RoomBounds);
			return location * level.RoomSize * GameSettings.TILE_SIZE;
		}

		/// <summary>Gets the room's level position from the level coord.</summary>
		public static Rectangle2I LevelCoordToRoomLocationPosition(this Level level,
			Rectangle2I levelCoord, bool clamp = false)
		{
			Point2I location1 = GMath.FloorDiv(levelCoord.Min, level.RoomSize);
			Point2I location2 = GMath.CeilingDiv(levelCoord.Max, level.RoomSize);
			if (clamp) {
				location1 = GMath.Clamp(location1, level.RoomBounds);
				location2 = GMath.Clamp(location2, level.RoomBounds);
			}
			Rectangle2I location = Rectangle2I.FromEndPoints(location1, location2);
			return location * level.RoomSize * GameSettings.TILE_SIZE;
		}


		//-----------------------------------------------------------------------------
		// Contains
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the level contains the level position.</summary>
		public static bool ContainsLevelPosition(this Level level,
			Point2I levelPosition)
		{
			return level.PixelBounds.Contains(levelPosition);
		}

		/// <summary>Returns true if the level contains the level coord.</summary>
		public static bool ContainsLevelCoord(this Level level, Point2I levelCoord) {
			return level.TileBounds.Contains(levelCoord);
		}


		//-----------------------------------------------------------------------------
		// Room Location
		//-----------------------------------------------------------------------------

		// Clamp Room -----------------------------------------------------------------

		/// <summary>Gets the room location within the bounds of the level.</summary>
		public static Point2I ClampRoomLocation(this Level level,
			Point2I roomLocation)
		{
			return GMath.Clamp(roomLocation, level.RoomBounds);
		}

		/// <summary>Gets the room at the location within the bounds of the level.</summary>
		public static Room ClampRoom(this Level level, Point2I roomLocation) {
			return level.GetRoomAt(GMath.Clamp(roomLocation, level.RoomBounds));
		}


		//-----------------------------------------------------------------------------
		// Tile Accessors
		//-----------------------------------------------------------------------------

		// Tiles ----------------------------------------------------------------------

		/// <summary>Gets the tile at the specified coord in the level.</summary>
		public static TileDataInstance GetTileAt(this Level level, int x, int y,
			int layer, bool includeShared = false)
		{
			return GetTileAt(level, new Point2I(x, y), layer, includeShared);
		}

		/// <summary>Gets the tile at the specified coord in the level.</summary>
		public static TileDataInstance GetTileAt(this Level level, Point2I levelCoord,
			int layer, bool includeShared = false)
		{
			Room room;
			Point2I location = LevelToRoomCoord(level, levelCoord, out room);
			if (room != null)
				return room.GetTile(location, layer, includeShared);
			return null;
		}

		// Actions --------------------------------------------------------------------
		
		/// <summary>Gets all action tiles at the specified position in the level.</summary>
		public static IEnumerable<ActionTileDataInstance> GetActionTilesAt(
			this Level level, Point2I levelPosition, bool includeShared = false)
		{
			Room room;
			Point2I position = LevelToRoomPosition(level, levelPosition, out room);
			if (room != null)
				return room.GetActionTilesAt(position, includeShared);
			return Enumerable.Empty<ActionTileDataInstance>();
		}

		/// <summary>Gets the first action tiles at the specified position in the
		/// level.</summary>
		public static ActionTileDataInstance GetActionTileAt(this Level level,
			Point2I levelPosition, bool includeShared = false)
		{
			Room room;
			Point2I position = LevelToRoomPosition(level, levelPosition, out room);
			if (room != null)
				return room.GetActionTileAt(position, includeShared);
			return null;
		}
		
		/// <summary>Gets the index of the action tile in its room's list.</summary>
		public static int IndexOfActionTile(this Level level, ActionTileDataInstance actionTile) {
			return actionTile.Room.IndexOfActionTile(actionTile);
		}


		//-----------------------------------------------------------------------------
		// Tile Mutators
		//-----------------------------------------------------------------------------

		// Tiles ----------------------------------------------------------------------
		
		/// <summary>Places the tile at the specified coord in the level.</summary>
		public static void PlaceTile(this Level level, TileInstanceLocation tile,
			bool safe = false)
		{
			PlaceTile(level, tile.Tile, tile.Location, tile.Layer, safe);
		}

		/// <summary>Places the tile at the specified coord in the level.</summary>
		public static void PlaceTile(this Level level, TileDataInstance tile,
			int x, int y, int layer, bool safe = false)
		{
			PlaceTile(level, tile, new Point2I(x, y), layer, safe);
		}

		/// <summary>Places the tile at the specified coord in the level.</summary>
		public static void PlaceTile(this Level level, TileDataInstance tile,
			Point2I levelCoord, int layer, bool safe = false)
		{
			Room room;
			Point2I location = LevelToRoomCoord(level, levelCoord, out room);
			if (room != null || !safe)
				room.PlaceTile(tile, location, layer);
		}

		/// <summary>Creates a tile at the specified coord in the level.</summary>
		public static TileDataInstance CreateTile(this Level level, TileData data,
			int x, int y, int layer, bool safe = false)
		{
			return CreateTile(level, data, new Point2I(x, y), layer, safe);
		}

		/// <summary>Creates a tile at the specified coord in the level.</summary>
		public static TileDataInstance CreateTile(this Level level, TileData data,
			Point2I levelCoord, int layer, bool safe = false)
		{
			Room room;
			Point2I location = LevelToRoomCoord(level, levelCoord, out room);
			if (room != null || !safe)
				return room.CreateTile(data, location, layer);
			return null;
		}

		/// <summary>Removes the tile at the specified coord from the level.</summary>
		public static void RemoveTile(this Level level, int x, int y, int layer,
			bool safe = false)
		{
			RemoveTile(level, new Point2I(x, y), layer, safe);
		}

		/// <summary>Removes the tile at the specified coord from the level.</summary>
		public static void RemoveTile(this Level level, Point2I levelCoord, int layer,
			bool safe = false)
		{
			Room room;
			Point2I location = LevelToRoomCoord(level, levelCoord, out room);
			if (room != null || !safe)
				room.RemoveTile(location, layer);
		}

		/// <summary>Removes the tile from the level.</summary>
		public static void RemoveTile(this Level level, TileDataInstance tile) {
			tile.Room.RemoveTile(tile);
		}

		/// <summary>Updates the area the tile contains in the room based on its size.</summary>
		public static void UpdateTileSize(this Level level, TileDataInstance tile,
			Point2I oldSize)
		{
			tile.Room.UpdateTileSize(tile, oldSize);
		}

		// Actions --------------------------------------------------------------------

		/// <summary>Places an action tile at the specified position in the level.</summary>
		public static void PlaceActionTile(this Level level,
			ActionTileInstancePosition action, bool safe = false)
		{
			PlaceActionTile(level, action.Action, action.Position, safe);
		}

		/// <summary>Places an action tile at the specified position in the level.</summary>
		public static void PlaceActionTile(this Level level,
			ActionTileDataInstance action, int x, int y, bool safe = false)
		{
			PlaceActionTile(level, action, new Point2I(x, y), safe);
		}

		/// <summary>Places an action tile at the specified position in the level.</summary>
		public static void PlaceActionTile(this Level level,
			ActionTileDataInstance action, Point2I levelPosition, bool safe = false)
		{
			Room room;
			Point2I position = LevelToRoomPosition(level, levelPosition, out room);
			if (room != null || !safe)
				room.PlaceActionTile(action, position);
		}

		/// <summary>Creates an action tile at the specified position in the level.</summary>
		public static ActionTileDataInstance CreateActionTile(this Level level,
			ActionTileData data, int x, int y, bool safe = false)
		{
			return CreateActionTile(level, data, new Point2I(x, y), safe);
		}

		/// <summary>Creates an action tile at the specified position in the level.</summary>
		public static ActionTileDataInstance CreateActionTile(this Level level,
			ActionTileData data, Point2I levelPosition, bool safe = false)
		{
			Room room;
			Point2I position = LevelToRoomPosition(level, levelPosition, out room);
			if (room != null || !safe)
				return room.CreateActionTile(data, position);
			return null;
		}

		/// <summary>Removes the action tile from the level.</summary>
		public static void RemoveActionTile(this Level level,
			ActionTileDataInstance action)
		{
			action.Room.RemoveActionTile(action);
		}

		// General --------------------------------------------------------------------

		/// <summary>Removes the tile or action from the level.</summary>
		public static void Remove(this Level level, BaseTileDataInstance baseTile) {
			baseTile.Room.Remove(baseTile);
		}
	}
}
