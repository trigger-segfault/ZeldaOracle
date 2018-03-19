using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Players.States.SwingStates;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Common.Input;

namespace ZeldaOracle.Game.Items {

	public abstract class ItemWeapon : ItemEquipment {

		/// <summary>The flags describing how the weapon can be used.</summary>
		private WeaponFlags	flags;
		private int			equipSlot;

		// TODO: Store these as properties for save format
		private int			ammoIndex;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemWeapon() {
			flags			= WeaponFlags.None;
			equipSlot		= 0;
			ammoIndex		= -1;
		}



		//-----------------------------------------------------------------------------
		// Protected Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the ammo types used by this weapon.</summary>
		public override void SetAmmo(params string[] ammos) {
			base.SetAmmo(ammos);
			ammoIndex = 0;
		}

		/// <summary>Sets the ammo types used by this weapon.</summary>
		public override void SetAmmo(params Ammo[] ammos) {
			base.SetAmmo(ammos);
			ammoIndex = 0;
		}


		//-----------------------------------------------------------------------------
		// Equipping
		//-----------------------------------------------------------------------------

		public void Equip(int slot) {
			equipSlot = slot;
			base.Equip();
		}


		//-----------------------------------------------------------------------------
		// Ammo
		//-----------------------------------------------------------------------------

		/// <summary>Return true if the given amount of ammo exists for the current
		/// type.</summary>
		public bool HasAmmo(int amount = 1) {
			return (CurrentAmmo.Amount >= amount);
		}

		/// <summary>Use up the given amount of ammo of the current type.</summary>
		public void UseAmmo(int amount = 1) {
			CurrentAmmo.Amount -= amount;
		}


		//-----------------------------------------------------------------------------
		// Buttons
		//-----------------------------------------------------------------------------
		
		public bool IsButtonDown() {
			if (IsTwoHanded)
				return Inventory.GetSlotButton(0).IsDown() ||
					Inventory.GetSlotButton(1).IsDown();
			return Inventory.GetSlotButton(equipSlot).IsDown();
		}
		
		public bool IsButtonPressed() {
			if (IsTwoHanded)
				return Inventory.GetSlotButton(0).IsPressed() ||
					Inventory.GetSlotButton(1).IsPressed();
			return Inventory.GetSlotButton(equipSlot).IsPressed();
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------
		
		public virtual bool IsUsable() {
			if (Player.StateParameters.ProhibitWeaponUse)
				return false;
			if (Player.IsInMinecart && !flags.HasFlag(WeaponFlags.UsableInMinecart))
				return false;
			else if (Player.IsInAir && !flags.HasFlag(WeaponFlags.UsableWhileJumping))
				return false;
			else if (Player.Physics.IsInHole && !flags.HasFlag(WeaponFlags.UsableWhileInHole))
				return false;
			else if ((Player.WeaponState is PlayerHoldSwordState ||
				Player.WeaponState is PlayerSwingState ||
				Player.WeaponState is PlayerSeedShooterState ||
				Player.WeaponState is PlayerSpinSwordState) &&
				!flags.HasFlag(WeaponFlags.UsableWithSword))
			{
				return false;
			}
			else if (Player.IsSwimmingUnderwater)
				return flags.HasFlag(WeaponFlags.UsableUnderwater);
			else
				return true;
		}
		
		public virtual void OnWeaponEquip() { }

		public virtual void OnWeaponUnequip() { }

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public virtual void Interrupt() {}

		// Called when the items button is down (A or B).
		public virtual void OnButtonDown() {}
		
		// Called when the items button is pressed (A or B).
		public virtual bool OnButtonPress() { return false; }
		
		// Update the item.
		public virtual void Update() { }

		// Draws under link's sprite.
		public virtual void DrawUnder() { }

		// Draws over link's sprite.
		public virtual void DrawOver() { }


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Gets if the item has the specified flags.
		public bool HasFlag(WeaponFlags flags) {
			return this.flags.HasFlag(flags);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the item after it's added to the inventory list.</summary>
		protected override void OnInitialize() {
			base.OnInitialize();
			if (UsesAmmo && ammoIndex == -1)
				ammoIndex = 0;
		}

		protected virtual void DrawAmmo(Graphics2D g, Point2I position) {
			g.DrawString(GameData.FONT_SMALL,
				CurrentAmmo.Amount.ToString("00"),
				position + new Point2I(8, 8),
				EntityColors.Black);
		}
		
		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			if (UsesAmmo)
				DrawAmmo(g, position);
			if (MaxLevel > Item.Level1)
				DrawLevel(g, position);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the item flags.</summary>
		public WeaponFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		public int CurrentEquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}

		public InputControl CurrentControl {
			get { return Inventory.GetSlotButton(equipSlot); }
		}

		public bool IsTwoHanded {
			get { return flags.HasFlag(WeaponFlags.TwoHanded); }
		}

		public int AmmoIndex {
			get { return ammoIndex; }
			set { ammoIndex = GMath.Clamp(value, 0, AmmoCount - 1); }
		}

		public Ammo CurrentAmmo {
			get {
				if (!UsesAmmo || ammoIndex == -1)
					return null;
				return GetAmmoAt(ammoIndex);
			}
		}
	}
}
