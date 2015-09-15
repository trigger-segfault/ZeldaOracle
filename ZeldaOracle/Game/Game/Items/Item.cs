using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Items {
	public abstract class Item {

		protected Inventory inventory;

		protected string	id;
		protected string[]	name;
		protected string[]	description;
		protected int		level;
		protected int		maxLevel;

		protected int		currentAmmo;
		protected Ammo[]	ammo;
		protected bool		obtained;
		protected bool		stolen;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		protected Item() {
			this.inventory		= null;
			this.id				= "";
			this.name			= new string[] { "" };
			this.description	= new string[] { "" };
			this.level			= 0;
			this.maxLevel		= 0;
			this.currentAmmo	= -1;
			this.ammo			= null;
			this.obtained		= false;
			this.stolen			= false;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public virtual void OnStart() { }

		// Called when the item is put away.
		public virtual void OnEnd() { }

		// Immediately interrupt this item (ex: if link falls in a hole).
		public virtual void Interrupt() { }

		// Draws under link's sprite.
		public virtual void DrawUnder() { }

		// Draws over link's sprite.
		public virtual void DrawOver() { }

		// Called when the item is added to the inventory list
		public virtual void OnAdded(Inventory inventory) {
			this.inventory = inventory;
		}

		// Called when the item's level is changed.
		public virtual void OnLevelUp() { }

		// Called when the item has been obtained.
		public virtual void OnObtained() { }

		// Called when the item has been unobtained.
		public virtual void OnUnobtained() { }

		// Called when the item has been stolen.
		public virtual void OnStolen() { }

		// Called when the stolen item has been returned.
		public virtual void OnReturned() { }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the id of the item
		public string ID {
			get { return id; }
		}

		// Gets the name of the item
		public virtual string Name {
			get { return name[level]; }
		}

		// Gets the description of the item
		public virtual string Description {
			get { return description[level]; }
		}

		// Gets the level of the item
		public int Level {
			get { return level; }
			set {
				if (level != value) {
					level = GMath.Clamp(value, 0, maxLevel);
					OnLevelUp();
				}
			}
		}

		// Gets the highest item level
		public int MaxLevel {
			get { return maxLevel; }
		}

		// Gets if the item has been obtained
		public bool Obtained {
			get { return obtained; }
			set {
				if (obtained != value) {
					obtained = value;
					if (obtained)
						OnObtained();
					else
						OnUnobtained();
				}
			}
		}

		// Gets if the item has been stolen
		public bool Stolen {
			get { return stolen; }
			set {
				if (stolen != value) {
					stolen = value;
					if (stolen)
						OnStolen();
					else
						OnReturned();
				}
			}
		}

	}
}
