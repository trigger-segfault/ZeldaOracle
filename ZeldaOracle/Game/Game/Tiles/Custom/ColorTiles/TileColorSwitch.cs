using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorSwitch : SwitchTileBase, ZeldaAPI.ColorSwitch {

		private PuzzleColor color;
		private bool syncWithDungeon;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorSwitch() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public override void OnToggle(bool switchState) {

			// Sync color switch across dungeon.
			if (syncWithDungeon && RoomControl.Dungeon != null) {
				Dungeon dungeon = RoomControl.Dungeon;
				dungeon.ColorSwitchColor = color;

				// Raise/lower color barriers if there are any.
				if (RoomControl.GetTilesOfType<TileColorBarrier>().Any()) {
					GameControl.PushRoomState(new RoomStateColorBarrier(color));
				}

				// Sync other color switches in the same room.
				foreach (TileColorSwitch tile in RoomControl.GetTilesOfType<TileColorSwitch>()) {
					if (tile != this && tile.SyncWithDungeon)
						tile.SetSwitchState(SwitchState);
				}
			}

			AudioSystem.PlaySound(GameData.SOUND_SWITCH);
		}

		public override void SetSwitchState(bool switchState) {
			base.SetSwitchState(switchState);

			if (switchState) {
				color = PuzzleColor.Red;
				Graphics.PlayAnimation(SpriteList[1]);
			}
			else {
				color = PuzzleColor.Blue;
				Graphics.PlayAnimation(SpriteList[0]);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			
			// Sync color with dungeon.
			syncWithDungeon = Properties.GetBoolean("sync_with_dungeon", false);
			if (syncWithDungeon && RoomControl.Dungeon != null) {
				SetSwitchState(RoomControl.Dungeon.ColorSwitchColor == PuzzleColor.Blue);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool syncWithDungeon = args.Properties.GetBoolean("sync_with_dungeon", false);
			bool switchState = args.Properties.GetBoolean("switch_state", false);
			if (syncWithDungeon && args.Level != null) {
				Dungeon dungeon = args.Level.Dungeon;
				if (dungeon != null)
					switchState = dungeon.ColorSwitchColor == PuzzleColor.Blue;
			}
			//ISprite sprite = GameData.SPR_TILE_COLOR_SWITCH_RED;
			//if (switchState)
			//	sprite = GameData.SPR_TILE_COLOR_SWITCH_BLUE;
			DrawTileDataIndex(g, args, switchState ? 1 : 0);
			/*g.DrawISprite(
				sprite,
				args.SpriteDrawSettings,
				args.Position,
				args.Color);*/
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return color; }
		}

		public bool SyncWithDungeon {
			get { return syncWithDungeon; }
		}


		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorSwitch.Color {
			get { return (ZeldaAPI.Color)color; }
		}
	}
}
