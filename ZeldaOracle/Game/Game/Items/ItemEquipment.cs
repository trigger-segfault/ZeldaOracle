using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items {
	public class ItemEquipment : Item {

		private bool isEquipped;
		private ISprite[] spriteEquipped;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEquipment(string id) : base(id) {
			isEquipped = false;
			spriteEquipped = null;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the equipped sprite of the item at the specified level.</summary>
		public ISprite GetSpriteEquipped(int level) {
			if (spriteEquipped == null)
				return null;
			if (level < spriteEquipped.Length)
				return spriteEquipped[level];
			return spriteEquipped.LastOrDefault();
		}
		

		//-----------------------------------------------------------------------------
		// Protected Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the leveled equipped sprites of the item.</summary>
		protected void SetSpriteEquipped(params ISprite[] sprites) {
			spriteEquipped = sprites;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		/// <summary>Equips the item.</summary>
		public void Equip() {
			if (!isEquipped) {
				isEquipped = true;
				OnEquip();
			}
		}

		/// <summary>Unequips the item.</summary>
		public void Unequip() {
			if (isEquipped) {
				isEquipped = false;
				OnUnequip();
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		/// <summary>Called when the item is equipped.</summary>
		public virtual void OnEquip() { }

		/// <summary>Called when the item is unequipped.</summary>
		public virtual void OnUnequip() { }

		/// <summary>Draws the item inside the inventory.</summary>
		protected override void DrawSprite(Graphics2D g, Point2I position) {
			ISprite spr = Sprite;
			if (isEquipped && spriteEquipped != null)
				spr = SpriteEquipped;
			g.DrawSprite(spr, position);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets if the item is equipped or not.</summary>
		public bool IsEquipped {
			get { return isEquipped; }
			set { isEquipped = value; }
		}
		
		/// <summary>Gets the equipped sprite of the item.</summary>
		public ISprite SpriteEquipped {
			get { return GetSpriteEquipped(Level); }
			//set { SetSpriteEquipped(value); }
		}
	}
}
