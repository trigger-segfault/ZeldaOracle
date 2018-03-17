using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items {
	public class ItemBombs : ItemWeapon {
		
		private EntityTracker<Bomb> bombTracker;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		//public ItemBombs() : base("bombs") {
		public ItemBombs(string id) : base(id) {
			/*SetName("Bombs");
			SetDescription("Very explosive.");
			SetMessage(
				"You got <red>Bombs<red>! Use them to blow open false walls. " +
					"Press <a> or <b> to set a Bomb. " +
					"If you also press <dpad>, you can throw the Bomb.",
				"You can now hold more <red>Bombs<red> than before!",
				"You can now hold even more <red>Bombs<red> than before!");
			SetSprite(GameData.SPR_ITEM_ICON_BOMB);
			SetAmmo("bombs");
			SetMaxAmmo(10, 20, 30);
			MaxLevel	= Item.Level3;
			HoldType    = RewardHoldTypes.TwoHands;
			IncreaseAmmoOnLevelUp = true;*/

			Flags = WeaponFlags.UsableWhileInHole;

			bombTracker	= new EntityTracker<Bomb>(1);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override bool OnButtonPress() {
			if (bombTracker.IsEmpty) {
				if (HasAmmo()) {
					// Conjure a new bomb
					UseAmmo();
					Bomb bomb = new Bomb();
					bombTracker.TrackEntity(bomb);
					Player.PickupEntity(bomb);
					return true;
				}
			}
			else {
				// Pickup a bomb from the ground
				foreach (Bomb bomb in bombTracker.Entities) {
					if (bomb != null && Player.Interactions.IsMeetingEntity(
						bomb, InteractionType.Bracelet, new HitBox(
							GameSettings.PLAYER_BRACELET_BOXES[Player.Direction],
							Player.Interactions.InteractionZRange)))
					{
						Player.PickupEntity(bomb);
						return true;
					}
				}
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
		}
	}
}
