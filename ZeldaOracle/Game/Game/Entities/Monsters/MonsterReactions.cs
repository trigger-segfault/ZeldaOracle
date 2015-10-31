using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum InteractionType {
		None = -1,

		EmberSeed = 0,			// Hit by an ember seed.
		ScentSeed,				// Hit by a scent seed.
		PegasusSeed,			// Hit by a pegasus seed.
		GaleSeed,				// Hit by a gale seed.
		MysterySeed,			// Hit by a mystery seed.
		Fire,					// Touches fire.
		Arrow,					// Hit by an arrow.
		SwordBeam,				// Hit by a sword beam projectile.
		RodFire,				// Hit by a projectile from the fire-rod.
		Sword,					// Hit by a sword.
		BiggoronSword,			// Hit by a biggoron sword.
		Boomerang,				// Hit by a boomerang.
		BombExplosion,			// Hit by a bomb explosion.
		Shield,					// Hit by a shield.
		SwitchHook,				// Hit by the switch hook.
		Shovel,					// Hit by a shovel being used.
		Pickup,					// Attempt to use the bracelet to pickup.
		ButtonAction,			// The A button is pressed while colliding.
		SwordHitShield,			// Their sword hits my shield.
		BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		ShieldHitShield,		// Their shield hits my shield.
		ThrownObject,			// Hit by a thrown object (thrown tiles, not bombs).
		MineCart,				// Hit by a minecart.
		Block,					// Hit by a block (either moving or spawned on top of).

		Count,
	};

	public class WeaponInteractionEventArgs : EventArgs {
        public ItemWeapon Weapon { get; set; }
	}

	public partial class Monster : Unit {
		// Member vs Static
		// Has Item parameter?

		public delegate void InteractionMemberDelegate(Entity sender, EventArgs args);

		public delegate void InteractionStaticDelegate(Monster monster, Entity sender, EventArgs args);
		//public delegate void StaticInteractionDelegateItem(Monster monster, Entity sender, ItemWeapon item);
		


		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public static class Reactions {
			
			
			//-----------------------------------------------------------------------------
			// Basic Monster Reactions
			//-----------------------------------------------------------------------------

			// Do nothing.
			public const InteractionStaticDelegate None = null;

			// Instantly kill the monster.
			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				monster.Kill();
			}
			
			// Instantly kill the monster.
			public static void SoftKill(Monster monster, Entity sender, EventArgs args) {
				monster.Kill(); // TODO: Soft Kill
			}

			// Damage the monster for 1 damage.
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(1, sender.Center);
			}
			
			// Damage the monster for 2 damage.
			public static void Damage2(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(2, sender.Center);
			}
			
			// Damage the monster for 3 damage.
			public static void Damage3(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(3, sender.Center);
			}
			
			// Damage the monster for 4 damage.
			public static void Damage4(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(4, sender.Center);
			}

			// Bump the monster.
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				monster.Bump(sender.Center);
			}
			
			// Burn the monster for 1 damage.
			public static void Burn(Monster monster, Entity sender, EventArgs args) {
				monster.Burn(1);
			}
			
			// Stun the monster.
			public static void Stun(Monster monster, Entity sender, EventArgs args) {
			}
			
			// Send the monster in a gale.
			public static void Gale(Monster monster, Entity sender, EventArgs args) {
			}
			
			// Switch places with the monster (Only for switch-hook interactions).
			public static void SwitchHook(Monster monster, Entity sender, EventArgs args) {
			}
			

			//-----------------------------------------------------------------------------
			// Customizable Monster Reactions
			//-----------------------------------------------------------------------------

			// Damage the monster for the given amount.
			public static InteractionStaticDelegate Damage(int amount) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					monster.Hurt(amount, sender.Center);
				};
			}

			// Damage the monster for the given amount based on the used item's level.
			public static InteractionStaticDelegate DamageByLevel(int amountLevel1, int amountLevel2, int amountLevel3) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					WeaponInteractionEventArgs weaponArgs = args as WeaponInteractionEventArgs;
					int amount = 0;
					if (weaponArgs.Weapon.Level == Item.Level1)
						amount = amountLevel1;
					else if (weaponArgs.Weapon.Level == Item.Level2)
						amount = amountLevel2;
					else if (weaponArgs.Weapon.Level == Item.Level3)
						amount = amountLevel3;
					monster.Hurt(amount, sender.Center);
				};
			}
			

			//-----------------------------------------------------------------------------
			// Conditions
			//-----------------------------------------------------------------------------
			/*
			// Damage the monster for the given amount based on the used item's level.
			public static InteractionDelegate IfLevel(int level) {
				return delegate(Monster monster) {

				};
			}*/
			
		}
		
			
		//-----------------------------------------------------------------------------
		// Sender Reactions
		//-----------------------------------------------------------------------------

		public static class SenderReactions {

			// Do nothing.
			public const Delegate None = null;

			
			//-----------------------------------------------------------------------------
			// Basic Sender Reactions
			//-----------------------------------------------------------------------------

			public static void Destroy(Monster monster, Entity sender, EventArgs args) {
				sender.Destroy();
			}

			public static void Intercept(Monster monster, Entity sender, EventArgs args) {
				if (sender is Projectile)
					(sender as Projectile).Intercept();
			}
			
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Bump(monster.Center);
			}
			
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Hurt(1, monster.Center);
			}

			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Kill();
				else
					sender.Destroy();
			}

			//-----------------------------------------------------------------------------
			// Customizable Sender Reactions
			//-----------------------------------------------------------------------------

		}
	}
}
