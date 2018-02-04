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
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public struct TileDataDrawArgs {
		public TileData Tile { get; }
		public Properties Properties { get; }
		public Point2I Position { get; }
		public Zone Zone { get; }
		public Level Level { get; }
		public Room Room { get; }
		public float Time { get; }
		public Color Color { get; }
		public bool Extras { get; }
		public RewardManager RewardManager { get; }

		public SpriteDrawSettings SpriteDrawSettings {
			get { return new SpriteDrawSettings(Zone.StyleDefinitions, Time); }
		}

		public TileDataDrawArgs(TileData tile, Properties properties, Point2I position, Zone zone, Level level, Room room, float time, Color color, bool extras, RewardManager rewardManager) {
			this.Tile			= tile;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Level			= level;
			this.Room			= room;
			this.Time			= time;
			this.Color			= color;
			this.Extras			= extras;
			this.RewardManager	= rewardManager;
		}
	}

	public struct ActionTileDataDrawArgs {
		public ActionTileData ActionTile { get; }
		public Properties Properties { get; }
		public Point2I Position { get; }
		public Zone Zone { get; }
		public Level Level { get; }
		public Room Room { get; }
		public float Time { get; }
		public Color Color { get; }
		public bool Extras { get; }
		public RewardManager RewardManager { get; }

		public SpriteDrawSettings SpriteDrawSettings {
			get { return new SpriteDrawSettings(Zone.StyleDefinitions, Time); }
		}

		public ActionTileDataDrawArgs(ActionTileData actionTile, Properties properties, Point2I position, Zone zone, Level level, Room room, float time, Color color, bool extras, RewardManager rewardManager) {
			this.ActionTile		= actionTile;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Level			= level;
			this.Room			= room;
			this.Time			= time;
			this.Color			= color;
			this.Extras			= extras;
			this.RewardManager	= rewardManager;
		}
	}

	public delegate void TileDrawFunction(Graphics2D g, TileDataDrawArgs args);

	public delegate void ActionTileDrawFunction(Graphics2D g, ActionTileDataDrawArgs args);

	public static class TileDataDrawing {
		private static Dictionary<Type, TileDrawFunction> tileDrawFunctions;
		private static Dictionary<Type, ActionTileDrawFunction> actionTileDrawFunctions;

		public static RewardManager RewardManager { get; set; }
		public static Level Level { get; set; }
		public static float PlaybackTime { get; set; }
		public static bool Extras { get; set; }
		public static Room Room { get; set; }


		static TileDataDrawing() {
			tileDrawFunctions = new Dictionary<Type, TileDrawFunction>();
			actionTileDrawFunctions = new Dictionary<Type, ActionTileDrawFunction>();
		}

		public static void DrawTile(Graphics2D g, BaseTileData baseTileData, Point2I position, Zone zone) {
			DrawTile(g, baseTileData, baseTileData.Properties, position, zone, Color.White);
		}

		public static void DrawTile(Graphics2D g, BaseTileData baseTileData, Point2I position, Zone zone, Color color) {
			DrawTile(g, baseTileData, baseTileData.Properties, position, zone, color);
		}

		public static void DrawTile(Graphics2D g, BaseTileDataInstance baseTileData, Point2I position, Zone zone) {
			DrawTile(g, baseTileData.BaseData, baseTileData.Properties, position, zone, Color.White);
		}

		public static void DrawTile(Graphics2D g, BaseTileDataInstance baseTileData, Point2I position, Zone zone, Color color) {
			DrawTile(g, baseTileData.BaseData, baseTileData.Properties, position, zone, color);
		}

		public static void DrawTilePreview(Graphics2D g, BaseTileData baseTileData, Point2I position, Zone zone) {
			DrawTilePreview(g, baseTileData, position, zone, Color.White);
		}

		public static void DrawTilePreview(Graphics2D g, BaseTileData baseTileData, Point2I position, Zone zone, Color color) {
			if (baseTileData.HasPreviewSprite) {
				DrawPreview(g, baseTileData, position, zone, color);
			}
			else {
				DrawTile(g, baseTileData, baseTileData.Properties, position, zone, color);
			}
		}

		public static void DrawTilePreview(Graphics2D g, BaseTileDataInstance baseTileData, Point2I position, Zone zone) {
			DrawTilePreview(g, baseTileData, position, zone, Color.White);
		}

		public static void DrawTilePreview(Graphics2D g, BaseTileDataInstance baseTileData, Point2I position, Zone zone, Color color) {
			if (baseTileData.HasPreviewSprite) {
				DrawPreview(g, baseTileData.BaseData, position, zone, color);
			}
			else {
				DrawTile(g, baseTileData.BaseData, baseTileData.Properties, position, zone, color);
			}
		}

		private static void DrawPreview(Graphics2D g, BaseTileData baseTileData, Point2I position, Zone zone, Color color) {
			g.DrawSprite(
				baseTileData.PreviewSprite,
				new SpriteDrawSettings(zone.StyleDefinitions, PlaybackTime),
				position,
				color);
		}


		private static void DrawTile(Graphics2D g, BaseTileData baseTileData, Properties properties, Point2I position, Zone zone, Color color) {
			
			if (baseTileData is TileData) {
				TileData tile = (TileData) baseTileData;
				Type type = tile.Type ?? typeof(Tile);
				TileDrawFunction drawFunc;
				if (!tileDrawFunctions.TryGetValue(type, out drawFunc)) {
					MethodInfo methodInfo = type.GetMethod("DrawTileData", BindingFlags.Static | BindingFlags.Public);
					if (methodInfo != null)
						drawFunc = ReflectionHelper.GetFunction<TileDrawFunction>(methodInfo);
					tileDrawFunctions.Add(type, drawFunc);
				}
				var args = new TileDataDrawArgs(tile, properties, position, zone, Level, Room, PlaybackTime, color, Extras, RewardManager);
				drawFunc(g, args);
				Tile.DrawTileDataAbove(g, args);
			}
			else if (baseTileData is ActionTileData) {
				ActionTileData actionTile = (ActionTileData) baseTileData;
				Type type = actionTile.Type ?? typeof(ActionTile);
				ActionTileDrawFunction drawFunc;
				if (!actionTileDrawFunctions.TryGetValue(type, out drawFunc)) {
					MethodInfo methodInfo = type.GetMethod("DrawTileData", BindingFlags.Static | BindingFlags.Public);
					if (methodInfo != null)
						drawFunc = ReflectionHelper.GetFunction<ActionTileDrawFunction>(methodInfo);
					actionTileDrawFunctions.Add(type, drawFunc);
				}
				drawFunc(g, new ActionTileDataDrawArgs(actionTile, properties, position, zone, Level, Room, PlaybackTime, color, Extras, RewardManager));
			}
		}
	}
}
