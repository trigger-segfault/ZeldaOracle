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
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemBow : ItemWeapon {
		
		private EntityTracker<Arrow> arrowTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBow() {
			Flags =
				WeaponFlags.UsableInMinecart |
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWhileInHole;

			arrowTracker = new EntityTracker<Arrow>(2);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override bool OnButtonPress() {
			if (arrowTracker.IsMaxedOut || !HasAmmo())
				return false;
			
			UseAmmo();
			Player.Direction = Player.UseDirection;
			
			// Shoot and track the arrow projectile.
			Arrow arrow = new Arrow();
			arrow.Interactions.InteractionType = InteractionType.Arrow;
			Player.ShootFromDirection(arrow, Player.Direction,
				GameSettings.PROJECTILE_ARROW_SPEED,
				Directions.ToVector(Player.Direction) * 8.0f);
			arrowTracker.TrackEntity(arrow);

			AudioSystem.PlaySound(GameData.SOUND_SHOOT_PROJECTILE);

			// Begin the busy state
			Player.BeginBusyState(10, Player.Animations.Throw);

			return true;
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			DrawAmmo(g, position);
		}
	}
}
