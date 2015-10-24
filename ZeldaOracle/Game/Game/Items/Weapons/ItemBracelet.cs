using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBracelet : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBracelet(int level = 0) {
			this.id				= "item_bracelet";
			this.name			= new string[] { "Power Bracelet", "Power Gloves" };
			this.description	= new string[] { "A strength booster.", "Used to lift large objects." };
			this.level			= level;
			this.maxLevel		= 1;
			this.flags			= ItemFlags.None;

			sprite = new Sprite[] {
				GameData.SPR_ITEM_ICON_BRACELET,
				GameData.SPR_ITEM_ICON_POWER_GLOVES
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Grab, pull, and pickup objects.
		public override void OnButtonDown() {
			// Check for a tile to grab.
			Tile grabTile = Player.Physics.GetMeetingSolidTile(Player.Position, Player.Direction);
			if (grabTile != null && !grabTile.Flags.HasFlag(TileFlags.NotGrabbable) && Player.CurrentState != Player.GrabState) {
				Player.GrabState.BraceletEquipSlot = CurrentEquipSlot;
				Player.BeginState(Player.GrabState);
				Player.Movement.StopMotion();
			}
		}
	}
}
