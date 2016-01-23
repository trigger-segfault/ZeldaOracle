using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Players.Tools {
	public class PlayerToolSword : UnitTool {

		private ItemSword itemSword;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolSword() {
			toolType = UnitToolType.Sword;
			IsPhysicsEnabled = true;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			itemSword = (ItemSword) unit.GameControl.Inventory.GetItem("item_sword");
		}

		public override void OnCollideEntity(Entity entity) {
			// Collect collectible entities.
			if (entity is Collectible) {
				Collectible collectible = (Collectible) entity;
				if (collectible.IsPickupable && collectible.IsCollectibleWithItems)
					collectible.Collect();
			}
		}

		public override void OnHitProjectile(Projectile projectile) {
			if (Player.CurrentState != Player.HoldSwordState &&
				projectile.ProjectileType == ProjectileType.Physical)
			{
				projectile.Intercept();
				AudioSystem.PlaySound(GameData.SOUND_SHIELD_DEFLECT);
			}
		}

		public override void OnHitMonster(Monster monster) {
			InteractionType interactionType = InteractionType.Sword;
			if (Player.CurrentState == Player.SwingBigSwordState)
				interactionType = InteractionType.BiggoronSword;
			else if (Player.CurrentState == Player.SpinSwordState)
				interactionType = InteractionType.SwordSpin;

			// Trigger the monster's sword reaction.
			monster.TriggerInteraction(interactionType, unit, new WeaponInteractionEventArgs() {
				Weapon = itemSword
			});

			// Stab if holding sword.
			if (Player.CurrentState == Player.HoldSwordState)
				Player.HoldSwordState.Stab(false);
			else if (Player.CurrentState == Player.SwingSwordState)
				Player.SwingSwordState.AllowSwordHold = false;
		}
		
		public override void OnParry(Unit other, Vector2F contactPoint) {
			// Stab if holding sword.
			if (Player.CurrentState == Player.HoldSwordState)
				Player.HoldSwordState.Stab(true);
			else if (Player.CurrentState == Player.SwingSwordState)
				Player.SwingSwordState.AllowSwordHold = false;
		}

		public override void Update() {

			base.Update();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Player Player {
			get { return (unit as Player); }
		}
	}
}
