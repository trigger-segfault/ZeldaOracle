using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles;
using System;

namespace ZeldaOracle.Game.Entities.Players.Tools {

	public class PlayerToolSword : UnitTool, IInterceptable {

		private ItemSword itemSword;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolSword() {
			// Interactions
			Interactions.Enable();
			Interactions.InteractionType = InteractionType.Sword;
			Interactions.ProtectParentAction = true;

			// Tool
			toolType = UnitToolType.Sword;
		}
		
		public bool Intercept() {
			// Stab if holding sword
			if (Player.WeaponState == Player.HoldSwordState) {
				Player.HoldSwordState.Stab(false);
				return true;
			}
			else if (Player.WeaponState == Player.SwingSwordState) {
				Player.SwingSwordState.AllowSwordHold = false;
				return true;
			}
			return false;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			itemSword = (ItemSword) unit.GameControl.Inventory.GetItem("sword");

			Interactions.InteractionEventArgs = new WeaponInteractionEventArgs() {
				Weapon = itemSword,
				Tool = this,
			};
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
