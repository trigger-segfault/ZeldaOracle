using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Players.Tools {
	public class PlayerToolShield : UnitTool {

		private ItemShield itemShield;
		private Rectangle2I[] shieldCollisionBoxes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolShield() {
			// Interactions
			Interactions.Enable();
			Interactions.InteractionType = InteractionType.Shield;
			Interactions.ProtectParentAction = true;

			// Tool
			toolType = UnitToolType.Shield;
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

		public override void Initialize() {
			base.Initialize();

			itemShield = (ItemShield) unit.GameControl.Inventory.GetItem("item_shield");
			
			Interactions.InteractionEventArgs = new WeaponInteractionEventArgs() {
				Weapon = itemShield,
				Tool = this,
			};
		}

		public override void OnEquip() {
			Interactions.InteractionBox = shieldCollisionBoxes[Player.Direction];
		}

		public override void Update() {
			Interactions.InteractionBox = shieldCollisionBoxes[Player.Direction];

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
