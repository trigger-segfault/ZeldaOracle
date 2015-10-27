using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Units;

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
				new Rectangle2I(14 - 8, 2 - 16, 2, 14),
				new Rectangle2I(8 - 8, 0 - 16, 8, 11),
				new Rectangle2I(0 - 8, 2 - 16, 2, 14),
				new Rectangle2I(0 - 8, 7 - 16, 9, 9)
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
