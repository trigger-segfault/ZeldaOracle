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

			Interactions.Enable();
			Interactions.InteractionType = InteractionType.Sword;
			Interactions.InteractionEventArgs = new WeaponInteractionEventArgs() {
				Weapon = itemSword,
				Tool = this,
			};
		}
		
		public override void OnHitProjectile(Projectile projectile) {
			// Deflect projectiles
			if (Player.WeaponState != Player.HoldSwordState &&
				projectile.ProjectileType == ProjectileType.Physical)
			{
				projectile.Intercept();
				AudioSystem.PlaySound(GameData.SOUND_SHIELD_DEFLECT);
			}
		}

		public override void OnHitMonster(Monster monster) {
			// Get the appropriate interaction type
			InteractionType interactionType = InteractionType.Sword;
			if (Player.WeaponState == Player.SwingBigSwordState)
				interactionType = InteractionType.BiggoronSword;
			else if (Player.WeaponState == Player.SpinSwordState)
				interactionType = InteractionType.SwordSpin;

			// Trigger the monster's sword reaction
			monster.TriggerInteraction(interactionType, unit,
				new WeaponInteractionEventArgs()
			{
				Weapon = itemSword,
				Tool = this
			});

			// Stab if holding sword
			if (Player.WeaponState == Player.HoldSwordState)
				Player.HoldSwordState.Stab(false);
			else if (Player.WeaponState == Player.SwingSwordState)
				Player.SwingSwordState.AllowSwordHold = false;
		}
		
		public override void OnParry(Unit other, Vector2F contactPoint) {
			if (Player.WeaponState == Player.HoldSwordState) {
				// Stab if holding sword
				Player.HoldSwordState.Stab(true);
			}
			else if (Player.WeaponState == Player.SwingSwordState) {
				// Don't allow the player to hold his sword upon completing the swing
				Player.SwingSwordState.AllowSwordHold = false;
			}
		}

		public override void Update() {
			base.Update();
			
			// Check for touching collectible items
			if (IsPhysicsEnabled) {
				for (int i = 0; i < unit.RoomControl.EntityCount; i++) {
					Entity entity = unit.RoomControl.Entities[i];
					if (entity is Collectible && entity.Physics.IsEnabled &&
						PositionedCollisionBox.Intersects(
							entity.Physics.PositionedSoftCollisionBox))
					{
						Collectible collectible = (Collectible) entity;
						if (collectible.IsCollectible &&
							collectible.IsCollectibleWithItems)
							collectible.Collect();
					}
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Player Player {
			get { return (unit as Player); }
		}
	}
}
