using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.Custom.SideScroll {
	public class TileDisappearingPlatform : Tile {
		
		private int appearTime;
		private int disappearTime;
		private int duration;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDisappearingPlatform() {
			
		}
		
		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			Properties properties = GetSyncedProperties(Properties, RoomControl.Room);
			appearTime = GMath.Max(0, properties.Get<int>("appear_time", 0));
			disappearTime = GMath.Max(0, properties.Get<int>("disappear_time", 0));
			duration = GMath.Max(GMath.Max(appearTime, disappearTime) + 1, properties.Get<int>("duration", 2));

			IsSolid = IsPlatformSolid;
			Graphics.IsVisible = IsPlatformVisible;
		}
		
		public override void Update() {
			base.Update();

			IsSolid = IsPlatformSolid;
			Graphics.IsVisible = IsPlatformVisible;

			if (Time == appearTime) {
				AudioSystem.PlaySound(GameData.SOUND_MYSTERY_SEED);
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Time {
			get { return RoomControl.CurrentRoomTicks % duration; }
		}

		public bool IsPlatformSolid {
			get { return CheckPlatformSolid(GameControl.RoomTicks % duration, appearTime, disappearTime, duration); }
		}

		public bool IsPlatformVisible {
			get { return CheckPlatformVisible(GameControl.RoomTicks % duration, appearTime, disappearTime, duration); }
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			if (args.Time == 0f || args.Room == null) {
				Tile.DrawTileData(g, args);
			}
			else {
				Properties properties = GetSyncedProperties(args.Properties, args.Room);
				int appearTime = GMath.Max(0, properties.Get<int>("appear_time", 0));
				int disappearTime = GMath.Max(0, properties.Get<int>("disappear_time", 0));
				int duration = GMath.Max(GMath.Max(appearTime, disappearTime) + 1, properties.Get<int>("duration", 2));
				if (CheckPlatformVisible(args.Time, appearTime, disappearTime, duration))
					Tile.DrawTileData(g, args);
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.Set("appear_time", 0)
				.SetDocumentation("Appear Time", "Disappearing", "The time in frames when the block starts appearing.");
			data.Properties.Set("disappear_time", 90)
				.SetDocumentation("Disappear Time", "Disappearing", "The time in frames when the block starts disappearing.");
			data.Properties.Set("duration", 180)
				.SetDocumentation("Total Duration", "Disappearing", "The total duration of the time loop.");
			data.Properties.Set("sync_with", "")
				.SetDocumentation("Sync With", "tile_type", "TileDisappearingPlatform", "Disappearing", "Substitute the time properties with the specified disappearing platform.");
		}

		private static Properties GetSyncedProperties(Properties properties, Room room) {
			string syncName = properties.Get<string>("sync_with", "");
			if (!string.IsNullOrWhiteSpace(syncName)) {
				TileDataInstance tile = room.FindTileOfTypeByID<TileDisappearingPlatform>(syncName);
				if (tile != null)
					return tile.Properties;
			}
			return properties;
		}

		private static bool CheckPlatformSolid(float time, int appearTime, int disappearTime, int duration) {
			int disappearEnd = CalcDisappearEnd(appearTime, disappearTime, duration);
			if (appearTime < disappearEnd)
				return (time >= appearTime && time < disappearEnd);
			else
				return (time >= appearTime || time < disappearEnd);
		}

		private static bool CheckPlatformVisible(float time, int appearTime, int disappearTime, int duration) {
			time = (int) time % duration;
			int timeAppear = GMath.Wrap((int) time - appearTime, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int timeDisappear = GMath.Wrap((int) time - disappearTime, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int appearEnd = CalcAppearEnd(appearTime, disappearTime, duration);
			int disappearEnd = CalcDisappearEnd(appearTime, disappearTime, duration);
			if (IsAppearing(time, appearTime, disappearTime, duration))
				return timeAppear % 2 == 1;
			else if (IsDisappearing(time, appearTime, disappearTime, duration))
				return timeDisappear % 2 == 0;
			else if (disappearTime > appearEnd)
				return (time >= appearEnd && time < disappearTime);
			else
				return (time >= appearEnd || time < disappearTime);
		}

		private static int CalcAppearEnd(int appearTime, int disappearTime, int duration) {
			int appearDuration = GMath.Min(duration - 1, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int wrapDuration = GMath.Max(duration - 1, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int appearEnd = appearTime + appearDuration;
			if (disappearTime >= appearTime) {
				if (appearEnd > disappearTime)
					appearEnd = disappearTime;
				else
					appearEnd %= wrapDuration;
			}
			else {
				if (appearEnd >= duration) {
					appearEnd %= wrapDuration;
					if (appearEnd > disappearTime)
						appearEnd = disappearTime;
				}
			}
			return appearEnd;
		}

		private static int CalcDisappearEnd(int appearTime, int disappearTime, int duration) {
			int appearDuration = GMath.Min(duration - 1, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int wrapDuration = GMath.Max(duration - 1, GameSettings.TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION);
			int disappearEnd = disappearTime + appearDuration;
			if (appearTime > disappearTime) {
				if (disappearEnd > appearTime)
					disappearEnd = appearTime;
				else
					disappearEnd %= wrapDuration;
			}
			else {
				if (disappearEnd >= duration) {
					disappearEnd %= wrapDuration;
					if (disappearEnd > appearTime)
						disappearEnd = appearTime;
				}
			}
			return disappearEnd;
		}

		private static bool IsAppearing(float time, int appearTime, int disappearTime, int duration) {
			int appearEnd = CalcAppearEnd(appearTime, disappearTime, duration);
			if (appearTime <= appearEnd)
				return (time >= appearTime && time < appearEnd);
			else
				return (time >= appearTime || time < appearEnd);
		}

		private static bool IsDisappearing(float time, int appearTime, int disappearTime, int duration) {
			int disappearEnd = CalcDisappearEnd(appearTime, disappearTime, duration);
			if (disappearTime <= disappearEnd)
				return (time >= disappearTime && time < disappearEnd);
			else
				return (time >= disappearTime || time < disappearEnd);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		public override bool IsStatic {
			get { return false; }
		}
	}
}
