using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.ResourceData;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>Argumemts used to draw a tile data's in-game type.</summary>
	public struct TileDataDrawArgs {
		/// <summary>The base action data for this tile.</summary>
		public TileData Tile { get; }
		/// <summary>The properties for this tile.</summary>
		public Properties Properties { get; }
		/// <summary>The pixel position of this tile.</summary>
		public Point2I Position { get; }
		/// <summary>The zone this tile is in.</summary>
		public Zone Zone { get; }
		/// <summary>The colorization of the tile.</summary>
		public Color Color { get; }
		/// <summary>The level containing the tile.</summary>
		public Level Level { get; }
		/// <summary>The room containing the tile.</summary>
		public Room Room { get; }
		/// <summary>The animation playback time.</summary>
		public float Time { get; }
		/// <summary>True if extra details in the tile are drawn.</summary>
		public bool Extras { get; }
		/// <summary>The reward database.</summary>
		public RewardManager RewardManager { get; }

		/// <summary>Gets sprite drawing settings based on the arguments.</summary>
		public SpriteSettings SpriteSettings {
			get { return new SpriteSettings(Zone.StyleDefinitions, Time); }
		}

		/// <summary>Constructs the tile data drawing arguments.</summary>
		public TileDataDrawArgs(TileData tile, Properties properties, Point2I position, Zone zone, Color color) {
			this.Tile			= tile;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Color			= color;
			this.Level			= TileDataDrawing.Level;
			this.Room			= TileDataDrawing.Room;
			this.Time			= TileDataDrawing.PlaybackTime;
			this.Extras			= TileDataDrawing.Extras;
			this.RewardManager	= TileDataDrawing.RewardManager;
		}
	}

	/// <summary>Argumemts used to draw an action data's in-game type.</summary>
	public struct ActionDataDrawArgs {
		/// <summary>The base action data for this action.</summary>
		public ActionTileData Action { get; }
		/// <summary>The properties for this action.</summary>
		public Properties Properties { get; }
		/// <summary>The pixel position of this action.</summary>
		public Point2I Position { get; }
		/// <summary>The zone this action is in.</summary>
		public Zone Zone { get; }
		/// <summary>The colorization of the action.</summary>
		public Color Color { get; }
		/// <summary>The level containing the action.</summary>
		public Level Level { get; }
		/// <summary>The room containing the action.</summary>
		public Room Room { get; }
		/// <summary>The animation playback time.</summary>
		public float Time { get; }
		/// <summary>True if extra details in the action are drawn.</summary>
		public bool Extras { get; }
		/// <summary>The reward database.</summary>
		public RewardManager RewardManager { get; }

		/// <summary>Gets sprite drawing settings based on the arguments.</summary>
		public SpriteSettings SpriteSettings {
			get { return new SpriteSettings(Zone.StyleDefinitions, Time); }
		}

		/// <summary>Constructs the action data drawing arguments.</summary>
		public ActionDataDrawArgs(ActionTileData action, Properties properties, Point2I position, Zone zone, Color color) {
			this.Action			= action;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Color			= color;
			this.Level			= TileDataDrawing.Level;
			this.Room			= TileDataDrawing.Room;
			this.Time			= TileDataDrawing.PlaybackTime;
			this.Extras			= TileDataDrawing.Extras;
			this.RewardManager	= TileDataDrawing.RewardManager;
		}
	}

	/// <summary>The function called to draw a tile's type.</summary>
	public delegate void TileDataDrawer(Graphics2D g, TileDataDrawArgs args);

	/// <summary>The function called to draw an action's type.</summary>
	public delegate void ActionDataDrawer(Graphics2D g, ActionDataDrawArgs args);

	/// <summary>A static class used to draw a tile type in the editor
	/// by using static reflection.</summary>
	public static class TileDataDrawing {
		/// <summary>The collection of tile drawing functions.</summary>
		private static Dictionary<Type, TileDataDrawer> tileDrawFunctions;
		/// <summary>The collection of action drawing functions.</summary>
		private static Dictionary<Type, ActionDataDrawer> actionDrawFunctions;


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The reward manager argument.</summary>
		public static RewardManager RewardManager { get; set; }
		/// <summary>The tile's level argument.</summary>
		public static Level Level { get; set; }
		/// <summary>The tile's room argument.</summary>
		public static Room Room { get; set; }
		/// <summary>The animation playback time argument.</summary>
		public static float PlaybackTime { get; set; }
		/// <summary>The argument specifying if extra details are drawn.</summary>
		public static bool Extras { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the type drawing function dictionaries.</summary>
		static TileDataDrawing() {
			tileDrawFunctions = new Dictionary<Type, TileDataDrawer>();
			actionDrawFunctions = new Dictionary<Type, ActionDataDrawer>();
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data.</summary>
		public static void DrawTile(Graphics2D g, BaseTileData data, Point2I position,
			Zone zone)
		{
			DrawTile(g, data, data.Properties, position, zone, Color.White);
		}

		/// <summary>Draws the tile data.</summary>
		public static void DrawTile(Graphics2D g, BaseTileData data, Point2I position,
			Zone zone, Color color)
		{
			DrawTile(g, data, data.Properties, position, zone, color);
		}

		/// <summary>Draws the tile data instance.</summary>
		public static void DrawTile(Graphics2D g, BaseTileDataInstance data,
			Point2I position, Zone zone)
		{
			DrawTile(g, data.BaseData, data.Properties, position, zone, Color.White);
		}

		/// <summary>Draws the tile data instance.</summary>
		public static void DrawTile(Graphics2D g, BaseTileDataInstance data,
			Point2I position, Zone zone, Color color)
		{
			DrawTile(g, data.BaseData, data.Properties, position, zone, color);
		}

		/// <summary>Draws the tile data and uses its sprite object if one exists.</summary>
		public static void DrawTileObject(Graphics2D g, TileData data,
			Point2I position, Zone zone)
		{
			DrawTileObject(g, data, position, zone, Color.White);
		}

		/// <summary>Draws the tile data and uses its sprite object if one exists.</summary>
		public static void DrawTileObject(Graphics2D g, TileData data,
			Point2I position, Zone zone, Color color)
		{
			if (data.SpriteAsObject != null) {
				DrawSpriteObject(g, data, position, zone, color);
			}
			else {
				DrawTile(g, data, data.Properties, position, zone, color);
			}
		}

		/// <summary>Draws the tile data instance and uses its sprite object if
		/// one exists.</summary>
		public static void DrawTileObject(Graphics2D g, TileDataInstance data,
			Point2I position, Zone zone)
		{
			DrawTileObject(g, data, position, zone, Color.White);
		}

		/// <summary>Draws the tile data instance and uses its sprite object if
		/// one exists.</summary>
		public static void DrawTileObject(Graphics2D g, TileDataInstance data,
			Point2I position, Zone zone, Color color)
		{
			if (data.SpriteAsObject != null) {
				DrawSpriteObject(g, data.TileData, position, zone, color);
			}
			else {
				DrawTile(g, data.BaseData, data.Properties, position, zone, color);
			}
		}

		/// <summary>Draws the tile data and uses its preview sprite if one exists.</summary>
		public static void DrawTilePreview(Graphics2D g, BaseTileData data,
			Point2I position, Zone zone)
		{
			DrawTilePreview(g, data, position, zone, Color.White);
		}

		/// <summary>Draws the tile data and uses its preview sprite if one exists.</summary>
		public static void DrawTilePreview(Graphics2D g, BaseTileData data,
			Point2I position, Zone zone, Color color)
		{
			if (data.HasPreviewSprite) {
				DrawPreview(g, data, position, zone, color);
			}
			else {
				DrawTile(g, data, data.Properties, position, zone, color);
			}
		}

		/// <summary>Draws the tile data instance and uses its preview sprite if
		/// one exists.</summary>
		public static void DrawTilePreview(Graphics2D g, BaseTileDataInstance data,
			Point2I position, Zone zone)
		{
			DrawTilePreview(g, data, position, zone, Color.White);
		}

		/// <summary>Draws the tile data instance and uses its preview sprite if
		/// one exists.</summary>
		public static void DrawTilePreview(Graphics2D g, BaseTileDataInstance data,
			Point2I position, Zone zone, Color color)
		{
			if (data.HasPreviewSprite) {
				DrawPreview(g, data.BaseData, position, zone, color);
			}
			else {
				DrawTile(g, data.BaseData, data.Properties, position, zone, color);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile's preview sprite.</summary>
		private static void DrawPreview(Graphics2D g, BaseTileData data,
			Point2I position, Zone zone, Color color)
		{
			g.DrawSprite(
				data.PreviewSprite,
				new SpriteSettings(zone.StyleDefinitions, PlaybackTime),
				position,
				color);
		}

		/// <summary>Draws the tile's sprite object.</summary>
		private static void DrawSpriteObject(Graphics2D g, TileData data,
			Point2I position, Zone zone, Color color)
		{
			g.DrawSprite(
				data.SpriteAsObject,
				new SpriteSettings(zone.StyleDefinitions, PlaybackTime),
				position,
				color);
		}

		/// <summary>Attempts to draw the tile based on its type or entity type.</summary>
		private static void DrawTile(Graphics2D g, BaseTileData data,
			Properties properties, Point2I position, Zone zone, Color color)
		{
			// Create the arguments to call
			if (data is TileData) {
				TileDataDrawArgs args = new TileDataDrawArgs((TileData) data,
					properties, position, zone, color);

				TileDataDrawer func = ResourceDataDrawing.GetDrawer<TileDataDrawer>(
					data, typeof(Entity), data.EntityType);
				if (func == null)
					func = ResourceDataDrawing.GetDrawer<TileDataDrawer>(data);
				func?.Invoke(g, args);
			}
			else if (data is ActionTileData) {
				ActionDataDrawArgs args = new ActionDataDrawArgs((ActionTileData) data,
					properties, position, zone, color);

				ActionDataDrawer func =  ResourceDataDrawing.GetDrawer<ActionDataDrawer>(
					data, typeof(Entity), data.EntityType);
				if (func == null)
					func = ResourceDataDrawing.GetDrawer<ActionDataDrawer>(data);
				func?.Invoke(g, args);
			}

			// Attempt to draw through the entity type first
			/*if (ResourceDataDrawing.DrawData(data.GetType(), data.EntityType,
				typeof(Entity), g, args))
				return;

			ResourceDataDrawing.DrawData(data.GetType(), data.Type, data.OutputType,
				g, args);*/
		}

		/// <summary>Attempts to draw the tile based on its type or entity type.</summary>
		/*private static void DrawTile(Graphics2D g, BaseTileData data,
			Properties properties, Point2I position, Zone zone, Color color)
		{
			// Attempt to draw through the entity type first
			if (DrawTileEntity(g, data, properties, position, zone, color))
				return;

			Type[] types;
			if (data is TileData) {
				if (data.Type == null)
					types = new Type[] { typeof(Tile) };
				else
					types = TypeHelper.GetInheritance<Tile>(data.Type, false);
			}
			else if (data is ActionTileData) {
				if (data.Type == null)
					types = new Type[] { typeof(ActionTile) };
				else
					types = TypeHelper.GetInheritance<ActionTile>(data.Type, false);
			}
			else
				throw new ArgumentException("Invalidate BaseTileData!");
			
			foreach (Type type in types) {
				if (DrawTileType(type, g, data, properties, position, zone, color))
					return;
			}
		}*/

		/// <summary>Attempts to draw the tile based on its spawned entity type.
		/// Returns false if no function was found.</summary>
		private static bool DrawTileEntity(Graphics2D g, BaseTileData data,
			Properties properties, Point2I position, Zone zone, Color color)
		{
			if (data.EntityType == null)
				return false;

			Type[] types = TypeHelper.GetInheritance<Entity>(data.EntityType, false);

			foreach (Type type in types) {
				if (DrawTileEntityType(type, g, data, properties, position, zone, color))
					return true;
			}

			return false;
		}

		/// <summary>Attempts to draw the tile based on its spawned entity type.
		/// Returns false if no function was found.</summary>
		private static bool DrawTileEntityType(Type entityType, Graphics2D g,
			BaseTileData data, Properties properties, Point2I position,
			Zone zone, Color color)
		{
			if (data is TileData) {
				TileData tile = (TileData) data;
				TileDataDrawer drawFunc;
				if (!tileDrawFunctions.TryGetValue(entityType, out drawFunc)) {
					MethodInfo methodInfo = entityType.GetMethod("DrawTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(Graphics2D), typeof(TileDataDrawArgs));
					if (methodInfo != null) {
						drawFunc = ReflectionHelper.GetFunction
							<TileDataDrawer>(methodInfo);
					}
					tileDrawFunctions.Add(entityType, drawFunc);
				}
				if (drawFunc == null)
					return false;
				var args = new TileDataDrawArgs(tile, properties, position, zone, color);
				drawFunc(g, args);
				return true;
			}
			else if (data is ActionTileData) {
				ActionTileData actionTile = (ActionTileData) data;
				ActionDataDrawer drawFunc;
				if (!actionDrawFunctions.TryGetValue(entityType, out drawFunc)) {
					MethodInfo methodInfo = entityType.GetMethod("DrawTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(Graphics2D), typeof(ActionDataDrawArgs));
					if (methodInfo != null) {
						drawFunc = ReflectionHelper.GetFunction
							<ActionDataDrawer>(methodInfo);
					}
					actionDrawFunctions.Add(entityType, drawFunc);
				}
				if (drawFunc == null)
					return false;
				var args = new ActionDataDrawArgs(actionTile, properties, position, zone, color);
				drawFunc(g, args);
				return true;
			}
			return false;
		}

		/// <summary>Attempts to draw the tile based on its type.
		/// Returns false if no function was found.</summary>
		private static bool DrawTileType(Type tileType, Graphics2D g, BaseTileData data,
			Properties properties, Point2I position, Zone zone, Color color)
		{
			if (data is TileData) {
				TileData tile = (TileData) data;
				TileDataDrawer drawFunc;
				if (!tileDrawFunctions.TryGetValue(tileType, out drawFunc)) {
					MethodInfo methodInfo = tileType.GetMethod("DrawTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(Graphics2D), typeof(TileDataDrawArgs));
					//MethodInfo methodInfo = ResourceDataDrawing.GetDrawer(data.GetType(), data.Type ?? typeof(Tile), data.OutputType);
					if (methodInfo != null) {
						drawFunc = ReflectionHelper.GetFunction
							<TileDataDrawer>(methodInfo);
					}
					tileDrawFunctions.Add(tileType, drawFunc);
				}
				if (drawFunc == null)
					return false;
				var args = new TileDataDrawArgs(tile, properties, position, zone, color);
				drawFunc(g, args);
				Tile.DrawTileDataAbove(g, args);
				return true;
			}
			else if (data is ActionTileData) {
				ActionTileData actionTile = (ActionTileData) data;
				ActionDataDrawer drawFunc;
				if (!actionDrawFunctions.TryGetValue(tileType, out drawFunc)) {
					MethodInfo methodInfo = tileType.GetMethod("DrawTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(Graphics2D), typeof(ActionDataDrawArgs));
					//MethodInfo methodInfo = ResourceDataDrawing.GetDrawer(data.GetType(), data.Type ?? typeof(ActionTile), data.OutputType);
					if (methodInfo != null) {
						drawFunc = ReflectionHelper.GetFunction
							<ActionDataDrawer>(methodInfo);
					}
					actionDrawFunctions.Add(tileType, drawFunc);
				}
				if (drawFunc == null)
					return false;
				var args = new ActionDataDrawArgs(actionTile, properties, position, zone, color);
				drawFunc(g, args);
				return true;
			}
			return false;
		}
	}
}
