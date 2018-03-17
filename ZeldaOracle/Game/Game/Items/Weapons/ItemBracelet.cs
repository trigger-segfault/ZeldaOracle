using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBracelet : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		//public ItemBracelet() : base("bracelet") {
		public ItemBracelet(string id) : base(id) {
			/*SetName("Power Bracelet", "Power Gloves");
			SetDescription("A strength booster.", "Used to lift large objects.");
			SetMessage(
				"You got the <red>Power Bracelet<red>! " +
					"Hold the button and press <dpad> to lift heavy objects!",
				"You got the <red>Power Glove<red>! You can now lift heavy objects.");
			SetSprite(
				GameData.SPR_ITEM_ICON_BRACELET,
				GameData.SPR_ITEM_ICON_POWER_GLOVES);
			MaxLevel = Item.Level2;
			HoldType = RewardHoldTypes.TwoHands;*/

			Flags = WeaponFlags.None;
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
