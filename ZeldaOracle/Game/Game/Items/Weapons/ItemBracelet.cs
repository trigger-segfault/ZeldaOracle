using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters;

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

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_BRACELET,
				GameData.SPR_ITEM_ICON_POWER_GLOVES
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Grab, pull, and pickup objects.</summary>
		public override void OnButtonDown() {
			// Attempt to grab a tile
			if (!Player.IsBeingKnockedBack) {
				Tile grabTile = Player.GrabState.GetGrabTile();
				if (grabTile != null) {
					grabTile.OnGrab(Player.Direction, this);
					return;
				}
			}
		}

		public override bool OnButtonPress() {
			// Trigger grab interactions
			Rectangle2F braceletBox =
				GameSettings.PLAYER_BRACELET_BOXES[Player.Direction];
			WeaponInteractionEventArgs args = new WeaponInteractionEventArgs() {
				Weapon = this
			};
			HitBox hitBox = new HitBox(braceletBox,
				Player.Interactions.InteractionZRange);
			RoomControl.InteractionManager.TriggerInstantReaction(
				Player, InteractionType.Bracelet, hitBox, args);
			return (Player.WeaponState == Player.CarryState);
		}
	}
}
