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
		private bool syncWithArea;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorSwitch() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public override void OnToggle(bool switchState) {

			// Sync color switch across area
			if (syncWithArea && RoomControl.Area != null) {
				Area area = RoomControl.Area;
				area.ColorSwitchColor = color;

				// Raise/lower color barriers if there are any.
				if (RoomControl.GetTilesOfType<TileColorBarrier>().Any()) {
					GameControl.PushRoomState(new RoomStateColorBarrier(color));
				}

				// Sync other color switches in the same room.
				foreach (TileColorSwitch tile in RoomControl.GetTilesOfType<TileColorSwitch>()) {
					if (tile != this && tile.SyncWithArea)
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
			
			// Sync color with area
			syncWithArea = Properties.GetBoolean("sync_with_area", false);
			if (syncWithArea && RoomControl.Area != null) {
				SetSwitchState(RoomControl.Area.ColorSwitchColor == PuzzleColor.Red);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool syncWithArea = args.Properties.GetBoolean("sync_with_area", false);
			bool switchState = args.Properties.GetBoolean("switch_state", false);
			if (syncWithArea && args.Level != null) {
				Area area = args.Level.Area;
				if (area != null)
					switchState = area.ColorSwitchColor == PuzzleColor.Red;
			}
			//ISprite sprite = GameData.SPR_TILE_COLOR_SWITCH_RED;
			//if (switchState)
			//	sprite = GameData.SPR_TILE_COLOR_SWITCH_BLUE;
			DrawTileDataIndex(g, args, switchState ? 1 : 0);
			/*g.DrawSprite(
				sprite,
				args.SpriteDrawSettings,
				args.Position,
				args.Color);*/
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.AbsorbSeeds;

			data.Properties.Set("switch_once", false)
				.SetDocumentation("Switch Once", "Switch", "Can the color switch only be switched once?");
			data.Properties.Set("switch_state", false)
				.SetDocumentation("Switch State", "Switch", "The switch state of the color switch (false = blue, true = red).");
			data.Properties.Set("sync_with_area", false)
				.SetDocumentation("Sync With Area", "Switch", "Will the color switch's state be syncronized with other color switches in the area?");

			data.Events.AddEvent("toggle", "Toggle", "Switch", "Occurs when the Color Switch changes color.",
				new ScriptParameter(typeof(ZeldaAPI.ColorSwitch), "colorSwitch"));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return color; }
			set { SetSwitchState(color == PuzzleColor.Red); }
		}

		public bool SyncWithArea {
			get { return syncWithArea; }
		}


		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

	}
}
