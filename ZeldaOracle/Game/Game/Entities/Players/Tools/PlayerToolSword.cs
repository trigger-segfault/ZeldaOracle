using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Units;

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
			
			if (entity is Collectible) {
				Collectible collectible = (Collectible) entity;
				if (collectible.IsPickupable && collectible.IsCollectibleWithItems)
					collectible.Collect();
			}
			else if (entity is Monster) {
				//Monster monster = (Monster) entity;
				//monster.TriggerInteraction(monster.HandlerShield, itemShield);
			}
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
