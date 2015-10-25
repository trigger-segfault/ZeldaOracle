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


		// The flags describing the item.
		protected ItemFlags	flags;
		private int			equipSlot;
		protected int		currentAmmo;
		protected Ammo[]	ammo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemWeapon() {
			this.flags			= ItemFlags.None;
			this.equipSlot		= 0;
			this.currentAmmo	= -1;
			this.ammo			= null;
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
		
		// Return true if the given amount of ammo exists for the current type.
		public bool HasAmmo(int amount = 1) {
			return (ammo[currentAmmo].Amount >= amount);
		}

		// Use up the given amount of ammo of the current type.
		public void UseAmmo(int amount = 1) {
			ammo[currentAmmo].Amount -= amount;
		}

		public Ammo GetAmmoAt(int index) {
			return ammo[index];
		}


		//-----------------------------------------------------------------------------
		// Buttons
		//-----------------------------------------------------------------------------
		
		public bool IsButtonDown() {
			if (IsTwoHanded)
				return inventory.GetSlotButton(0).IsDown() ||
					inventory.GetSlotButton(1).IsDown();
			return inventory.GetSlotButton(equipSlot).IsDown();
		}
		
		public bool IsButtonPressed() {
			if (IsTwoHanded)
				return inventory.GetSlotButton(0).IsPressed() ||
					inventory.GetSlotButton(1).IsPressed();
			return inventory.GetSlotButton(equipSlot).IsPressed();
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------
		
		public virtual bool IsUsable() {
			if (Player.IsInAir && !flags.HasFlag(ItemFlags.UsableWhileJumping))
				return false;
			if (Player.Physics.IsInHole && !flags.HasFlag(ItemFlags.UsableWhileInHole))
				return false;
			if (((Player.CurrentState is PlayerHoldSwordState) ||
				(Player.CurrentState is PlayerSwingState) ||
				(Player.CurrentState is PlayerSpinSwordState)) &&
				flags.HasFlag(ItemFlags.UsableWithSword))
			{
				return true;
			}
			return (Player.CurrentState is PlayerNormalState);
		}

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public virtual void Interrupt() {}

		// Called when the items button is down (A or B).
		public virtual void OnButtonDown() {}
		
		// Called when the items button is pressed (A or B).
		public virtual void OnButtonPress() {}
		
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
		public bool HasFlag(ItemFlags flags) {
			return this.flags.HasFlag(flags);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected virtual void DrawAmmo(Graphics2D g, Point2I position, int lightOrDark) {
			Color color = (lightOrDark == GameData.VARIANT_LIGHT ? new Color(16, 16, 16) : Color.Black);
			g.DrawString(GameData.FONT_SMALL, ammo[currentAmmo].Amount.ToString("00"), position + new Point2I(8, 8), color);
		}
		
		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			if (maxLevel > Item.Level1)
				DrawLevel(g, position, lightOrDark);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets or sets the item flags.
		public ItemFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		public int CurrentEquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}

		public InputControl CurrentControl {
			get { return inventory.GetSlotButton(equipSlot); }
		}

		public bool IsTwoHanded {
			get { return flags.HasFlag(ItemFlags.TwoHanded); }
		}

		public int NumAmmos {
			get {
				if (ammo != null)
					return ammo.Length;
				return 0;
			}
		}

		public int CurrentAmmo {
			get { return currentAmmo; }
			set { currentAmmo = GMath.Clamp(value, 0, ammo.Length - 1); }
		}
	}
}
