using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.Tools {
	public class PlayerToolShield : UnitTool {

		private ItemShield itemShield;
		private Rectangle2I[] shieldCollisionBoxes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolShield() {
			toolType = UnitToolType.Shield;
			IsPhysicsEnabled = true;
			
			shieldCollisionBoxes = new Rectangle2I[] {
				new Rectangle2I(14 - 8, 2 - 13, 2, 14),	// Right
				new Rectangle2I( 7 - 8, 0 - 13, 9, 11),	// Up
				new Rectangle2I( 0 - 8, 2 - 13, 2, 14),	// Left
				new Rectangle2I( 0 - 8, 7 - 13, 9,  9)	// Down
			};
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			itemShield = (ItemShield) unit.GameControl.Inventory.GetItem("item_shield");
		}

		public override void OnCollideEntity(Entity entity) {

			if (entity is Monster) {
				//Monster monster = (Monster) entity;
				//monster.TriggerInteraction(monster.HandlerShield, itemShield);
			}
		}

		public override void OnHitProjectile(Projectile projectile) {
			projectile.Intercept();
			AudioSystem.PlaySound(GameData.SOUND_SHIELD_DEFLECT);
		}

		public override void OnHitMonster(Monster monster) {
			monster.TriggerInteraction(InteractionType.Shield, unit, new WeaponInteractionEventArgs() {
				Weapon = itemShield
			});
		}

		public override void Update() {
			collisionBox = shieldCollisionBoxes[Player.Direction];

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
