using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.Menus {
	public class Slot : ISlotConnection {

		private ISlotConnection[] connections;
		private Point2I position;
		private int width;
		private SlotGroup group;
		private ISlotItem item;
		private bool disabled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Slot(SlotGroup group, Point2I position, int width) {
			this.group			= group;
			this.position		= position;
			this.width			= width;
			this.connections	= new ISlotConnection[4];
			this.item			= null;
			this.disabled		= false;
		}


		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

		// Selects the slot item.
		public SlotGroup Select() {
			group.SetCurrentSlot(this);
			return group;
		}

		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		// Sets the item for the slot.
		public void SetItem(ISlotItem item) {
			this.item = item;
		}

		//-----------------------------------------------------------------------------
		// Connections
		//-----------------------------------------------------------------------------

		// Sets a single slot connections.
		public void SetConnection(int direction, ISlotConnection connection) {
			connections[direction] = connection;
		}

		// Sets all 4 slot connections.
		public void SetConnection(ISlotConnection[] connections) {
			this.connections = connections;
		}

		// Gets the slot connection in the specified direction.
		public ISlotConnection GetConnectionAt(int direction) {
			return connections[direction];
		}

		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Draws the slot and its item.
		public void Draw(Graphics2D g) {
			if (item != null) {
				item.DrawSlot(g, position);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ISlotItem SlotItem {
			get { return item; }
			set { item = value; }
		}

		public SlotGroup SlotGroup {
			get { return group; }
		}

		public Point2I Position {
			get { return position; }
			set { position = value; }
		}

		public int Width {
			get { return width; }
			set { width = value; }
		}

		public bool Disabled {
			get { return disabled; }
			set { disabled = value; }
		}
	}
}
