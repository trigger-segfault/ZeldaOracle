using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.Menus {
	public class SlotGroup : ISlotConnection {

		private List<Slot> slots;
		private Slot currentSlot;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SlotGroup() {
			this.slots = new List<Slot>();
			this.currentSlot = null;
		}


		//-----------------------------------------------------------------------------
		// Slots
		//-----------------------------------------------------------------------------

		// Adds the slot to the slot group.
		public Slot AddSlot(Point2I position, int width) {
			Slot slot = new Slot(this, position, width);
			slots.Add(slot);
			if (slots.Count == 1)
				currentSlot = slot;
			return slot;
		}

		// Gets the slot at the specified index.
		public Slot GetSlotAt(int index) {
			return slots[index];
		}

		// Sets the current slot in the slot group.
		public void SetCurrentSlot(Slot slot) {
			this.currentSlot = slot;
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Draws all the slots in the slot group.
		public void Draw(Graphics2D g) {
			for (int i = 0; i < slots.Count; i++) {
				slots[i].Draw(g);
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of slots.</summary>
		public int NumSlots {
			get { return slots.Count; }
		}

		/// <summary>Gets the currently selected slot.</summary>
		public Slot CurrentSlot {
			get { return currentSlot; }
		}

		/// <summary>Gets the index of the currently selected slot.</summary>
		public int CurrentSlotIndex {
			get {
				for (int i = 0; i < slots.Count; i++) {
					if (slots[i] == currentSlot)
						return i;
				}
				return -1;
			}
		}

		/// <summary>Get the slot at the given index.</summary>
		public Slot this[int index] {
			get { return slots[index]; }
		}

		/// <summary>Get the list of slots.</summary>
		public List<Slot> Slots {
			get { return slots; }
		}
	}
}
