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
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public struct TileDataDrawArgs {
		public TileData Tile { get; }
		public Properties Properties { get; }
		public Point2I Position { get; }
		public Zone Zone { get; }
		public Level Level { get; }
		public float Time { get; }
		public Color Color { get; }
		public bool Extras { get; }
		public RewardManager RewardManager { get; }

		public SpriteDrawSettings SpriteDrawSettings {
			get { return new SpriteDrawSettings(Zone.StyleDefinitions, Zone.ImageVariantID, Time); }
		}

		public TileDataDrawArgs(TileData tile, Properties properties, Point2I position, Zone zone, Level level, float time, Color color, bool extras, RewardManager rewardManager) {
			this.Tile			= tile;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Level          = level;
			this.Time			= time;
			this.Color			= color;
			this.Extras			= extras;
			this.RewardManager	= rewardManager;
		}
	}

	public struct EventTileDataDrawArgs {
		public EventTileData EventTile { get; }
		public Properties Properties { get; }
		public Point2I Position { get; }
		public Zone Zone { get; }
		public Level Level { get; }
		public float Time { get; }
		public Color Color { get; }
		public bool Extras { get; }
		public RewardManager RewardManager { get; }

		public SpriteDrawSettings SpriteDrawSettings {
			get { return new SpriteDrawSettings(Zone.StyleDefinitions, Zone.ImageVariantID, Time); }
		}

		public EventTileDataDrawArgs(EventTileData eventTile, Properties properties, Point2I position, Zone zone, Level level, float time, Color color, bool extras, RewardManager rewardManager) {
			this.EventTile		= eventTile;
			this.Properties		= properties;
			this.Position		= position;
			this.Zone			= zone;
			this.Level          = level;
			this.Time			= time;
			this.Color			= color;
			this.Extras			= extras;
			this.RewardManager	= rewardManager;
		}
	}

	public delegate void TileDrawFunction(Graphics2D g, TileDataDrawArgs args);

	public delegate void EventTileDrawFunction(Graphics2D g, EventTileDataDrawArgs args);

	public static class TileDataDrawing {
		private static Dictionary<Type, TileDrawFunction> tileDrawFunctions;
		private static Dictionary<Type, EventTileDrawFunction> eventTileDrawFunctions;

		public static RewardManager RewardManager { get; set; }
		public static Level Level { get; set; }
		public static float PlaybackTime { get; set; }
		public static bool Extras { get; set; }


		static TileDataDrawing() {
			tileDrawFunctions = new Dictionary<Type, TileDrawFunction>();
			eventTileDrawFunctions = new Dictionary<Type, EventTileDrawFunction>();
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
				new SpriteDrawSettings(zone.StyleDefinitions, zone.ImageVariantID, PlaybackTime),
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
				var args = new TileDataDrawArgs(tile, properties, position, zone, Level, PlaybackTime, color, Extras, RewardManager);
				drawFunc(g, args);
				Tile.DrawTileDataAbove(g, args);
			}
			else if (baseTileData is EventTileData) {
				EventTileData eventTile = (EventTileData) baseTileData;
				Type type = eventTile.Type ?? typeof(EventTile);
				EventTileDrawFunction drawFunc;
				if (!eventTileDrawFunctions.TryGetValue(type, out drawFunc)) {
					MethodInfo methodInfo = type.GetMethod("DrawTileData", BindingFlags.Static | BindingFlags.Public);
					if (methodInfo != null)
						drawFunc = ReflectionHelper.GetFunction<EventTileDrawFunction>(methodInfo);
					eventTileDrawFunctions.Add(type, drawFunc);
				}
				drawFunc(g, new EventTileDataDrawArgs(eventTile, properties, position, zone, Level, PlaybackTime, color, Extras, RewardManager));
			}
		}
	}
}
