using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {
	/*
	 * Track Index (and sprite index).
	 * 0 - horizontal
	 * 1 - vertical
	 * 2 - right/up
	 * 3 - up/left
	 * 4 - left/down
	 * 5 - down/right
	*/
	
	/// <summary>The orientations a minecart track can be in.</summary>
	/*public enum MinecartTrackOrientation {
		/// <summary>Track goes from left to right.</summary>
		Horizontal = 0,
		/// <summary>Track goes from top to bottom.</summary>
		Vertical,
		/// <summary>Track goes from top to right.</summary>
		UpRight,
		/// <summary>Track goes from top to left.</summary>
		UpLeft,
		/// <summary>Track goes from bottom to left.</summary>
		DownLeft,
		/// <summary>Track goes from bottom to right.</summary>
		DownRight
	}*/

	public static class MinecartTrackOrientationExtensions {
		public static bool HasDirection(this MinecartTrackOrientation orientation, Direction dir) {
			if (dir == Direction.Right)
				return orientation == MinecartTrackOrientation.Horizontal ||
						orientation == MinecartTrackOrientation.UpRight ||
						orientation == MinecartTrackOrientation.DownRight;
			else if (dir == Direction.Up)
				return orientation == MinecartTrackOrientation.Vertical ||
						orientation == MinecartTrackOrientation.UpRight ||
						orientation == MinecartTrackOrientation.UpLeft;
			else if (dir == Direction.Left)
				return orientation == MinecartTrackOrientation.Horizontal ||
						orientation == MinecartTrackOrientation.UpLeft ||
						orientation == MinecartTrackOrientation.DownLeft;
			else if (dir == Direction.Down)
				return orientation == MinecartTrackOrientation.Vertical ||
						orientation == MinecartTrackOrientation.DownRight ||
						orientation == MinecartTrackOrientation.DownLeft;
			return false;
		}
	}

	public class TileMinecartTrack : Tile, ZeldaAPI.MinecartTrack {
		
		private MinecartTrackOrientation trackOrientation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMinecartTrack() {

		}
		

		//-----------------------------------------------------------------------------
		// Track methods
		//-----------------------------------------------------------------------------

		public IEnumerable<Direction> GetDirections() {
			foreach (Direction direction in Direction.Range) {
				if (trackOrientation.HasDirection(direction))
					yield return direction;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			trackOrientation = Properties.GetEnum("track_orientation", MinecartTrackOrientation.Horizontal);
			Graphics.PlayAnimation(SpriteList[(int)trackOrientation]);

			// Spawn a minecart entity.
			if (SpawnsMinecart) {
				Minecart minecart = new Minecart(this);
				RoomControl.SpawnEntity(minecart, Position);
			}
		}
		

		//-----------------------------------------------------------------------------
		// API Methods
		//-----------------------------------------------------------------------------

		public void SwitchTrackDirection() {
			MinecartTrackOrientation switchedOrientation = Properties.GetEnum("switched_track_orientation", trackOrientation);

			if (trackOrientation != switchedOrientation) {
				Properties.Set("switched_track_orientation", (int)trackOrientation);
				Properties.Set("track_orientation", (int)switchedOrientation);
				trackOrientation = switchedOrientation;
				Graphics.PlayAnimation(SpriteList[(int)trackOrientation]);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			MinecartTrackOrientation orientation = args.Properties.GetEnum("track_orientation", MinecartTrackOrientation.Horizontal);
			/*ISprite sprite = null;
			switch (orientation) {
			case MinecartTrackOrientation.Horizontal: sprite = GameData.SPR_TILE_MINECART_TRACK_HORIZONTAL; break;
			case MinecartTrackOrientation.Vertical: sprite = GameData.SPR_TILE_MINECART_TRACK_VERTICAL; break;
			case MinecartTrackOrientation.UpRight: sprite = GameData.SPR_TILE_MINECART_TRACK_UP_RIGHT; break;
			case MinecartTrackOrientation.UpLeft: sprite = GameData.SPR_TILE_MINECART_TRACK_UP_LEFT; break;
			case MinecartTrackOrientation.DownLeft: sprite = GameData.SPR_TILE_MINECART_TRACK_DOWN_LEFT; break;
			case MinecartTrackOrientation.DownRight: sprite = GameData.SPR_TILE_MINECART_TRACK_DOWN_RIGHT; break;
			}
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}*/
			DrawTileDataIndex(g, args, (int) orientation);
			if (args.Properties.GetBoolean("minecart", false)) {
				if (orientation.HasDirection(Direction.Left) || orientation.HasDirection(Direction.Right))
					g.DrawSprite(GameData.SPR_MINECART_HORIZONTAL, args.SpriteSettings, args.Position, args.Color);
				else
					g.DrawSprite(GameData.SPR_MINECART_VERTICAL, args.SpriteSettings, args.Position, args.Color);
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.NotCoverable;
			data.ResetCondition = TileResetCondition.LeaveArea;

			data.Properties.SetEnumInt("track_orientation", MinecartTrackOrientation.Horizontal)
				.SetDocumentation("Track Orientation", "enum", typeof(MinecartTrackOrientation), "Minecart", "The initial orientation of the track.").Hide();
			data.Properties.SetEnumInt("switched_track_orientation", MinecartTrackOrientation.Horizontal)
				.SetDocumentation("Switched Track Orientation", "enum", typeof(MinecartTrackOrientation), "Minecart", "The switched orientation of the track.");
			data.Properties.Set("minecart", false)
				.SetDocumentation("Has Minecart", "Minecart", "Does this track currently have a minecart on it?");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsHorizontal {
			get { return (trackOrientation == MinecartTrackOrientation.Horizontal); }
		}
		
		public bool IsVertical {
			get { return (trackOrientation == MinecartTrackOrientation.Vertical); }
		}

		public bool SpawnsMinecart {
			get { return Properties.GetBoolean("minecart", false); }
			set { Properties.Set("minecart", value); }
		}

		//-----------------------------------------------------------------------------
		// API Properties
		//-----------------------------------------------------------------------------

	}
}
