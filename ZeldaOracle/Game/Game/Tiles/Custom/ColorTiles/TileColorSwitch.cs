using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
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
				color = PuzzleColor.Blue;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_BLUE;
			}
			else {
				color = PuzzleColor.Red;
				CustomSprite = GameData.SPR_TILE_COLOR_SWITCH_RED;
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
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return color; }
		}

		public bool SyncWithDungeon {
			get { return syncWithDungeon; }
		}
	}
}
