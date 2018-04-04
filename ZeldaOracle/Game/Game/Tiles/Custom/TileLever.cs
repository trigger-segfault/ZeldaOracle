using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Tiles {

	public class TileLever : SwitchTileBase, ZeldaAPI.Lever {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLever() {

		}
		

		//-----------------------------------------------------------------------------
		// Lever Methods
		//-----------------------------------------------------------------------------

		public override void OnToggle(bool switchState) {
			AudioSystem.PlaySound(GameData.SOUND_SWITCH);
		}
		
		public override void SetSwitchState(bool switchState) {
			base.SetSwitchState(switchState);
			//if (switchState)
				//Graphics.PlaySprite(GameData.SPR_TILE_LEVER_RIGHT);
			//else
				//Graphics.PlaySprite(GameData.SPR_TILE_LEVER_LEFT);
			Graphics.PlayAnimation(SpriteList[SwitchState ? 1 : 0]);
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool switchState = args.Properties.Get<bool>("switch_state", false);
			Tile.DrawTileDataIndex(g, args, switchState ? 1 : 0);
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.AbsorbSeeds;

			data.Properties.Set("switch_once", false)
				.SetDocumentation("Switch Once", "Lever", "Can the lever only be switched once?");
			data.Properties.Set("switch_state", false)
				.SetDocumentation("Switch State", "Lever", "True if the lever is facing left.");

			data.Events.AddEvent("toggle", "Toggle", "Switch", "Occurs when the lever is toggled.",
				new ScriptParameter(typeof(ZeldaAPI.Lever), "lever"));
		}


		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		bool ZeldaAPI.Lever.IsFacingLeft {
			get { return !SwitchState; }
		}
		
		bool ZeldaAPI.Lever.IsFacingRight {
			get { return SwitchState; }
		}
	}
}
