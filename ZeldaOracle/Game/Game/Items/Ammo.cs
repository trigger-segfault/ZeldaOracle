using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control.Menus;

namespace ZeldaOracle.Game.Items {
	public class Ammo : ISlotItem {

		private string		id;
		private Item		container;

		private string		name;
		private string		description;
		private string		obtainMessage;
		private string		cantCollectMessage;
		private ISprite		sprite;

		private bool		isAmountBased;
		private int			amount;
		private int			maxAmount;

		private bool		isObtained;
		private bool		isLost;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Ammo(string id) {
			this.id         = id;
			container		= null;
			
			name			= "";
			description		= "";
			obtainMessage		= "";
			cantCollectMessage	= "";
			sprite			= new EmptySprite();

			isAmountBased	= true;
			amount			= 0;
			maxAmount		= 0;

			isObtained		= false;
			isLost			= false;
		}

		public Ammo(string id, string name, string description, ISprite sprite,
			int amount, int maxAmount)
		{
			this.id				= id;
			container			= null;

			this.name			= name;
			this.description	= description;
			obtainMessage		= "";
			cantCollectMessage	= "";
			this.sprite			= sprite;

			isAmountBased		= true;
			this.amount			= amount;
			this.maxAmount		= maxAmount;

			isObtained		= false;
			isLost		= false;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		/// <summary>Draws the item inside the inventory.</summary>
		public virtual void DrawSlot(Graphics2D g, Point2I position) {
			g.DrawSprite(sprite, position);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the id of the ammo.</summary>
		public string ID {
			get { return id; }
		}

		/// <summary>Gets the item containing this ammo.</summary>
		public Item Container {
			get { return container; }
			set { container = value; }
		}

		/// <summary>Gets if the item needs a container in order to be collected.</summary>
		public bool NeedsContainer {
			get { return container != null; }
		}

		/// <summary>Gets if the ammos container is available and thus it can be
		/// picked up. Also returns true if the ammo does not need a container.</summary>
		public bool IsContainerAvailable {
			get { return (container == null || container.IsAvailable); }
		}

		/// <summary>Gets the name of the ammo.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets the description of the ammo.</summary>
		public string Description {
			get { return description; }
			set { description = value; }
		}

		/// <summary>Gets the message when the player picks up this ammo type
		/// for the first time.</summary>
		public string ObtainMessage {
			get { return obtainMessage; }
			set { obtainMessage = value; }
		}

		/// <summary>Gets the message when the player tries to pick up the ammo but
		/// has no container for it.</summary>
		public string CantCollectMessage {
			get { return cantCollectMessage; }
			set { cantCollectMessage = value; }
		}

		/// <summary>Gets the sprite of the ammo.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets if the ammo uses an amount and is not
		/// instead, all or nothing.</summary>
		public bool IsAmountBased {
			get { return isAmountBased; }
			set { isAmountBased = value; }
		}

		/// <summary>Gets or sets the current amount of the ammo.</summary>
		public int Amount {
			get { return amount; }
			set {
				if (isObtained && !isLost)
					amount = GMath.Clamp(value, 0, maxAmount);
			}
		}

		/// <summary>Gets or sets the max amount of the ammo.</summary>
		public int MaxAmount {
			get { return maxAmount; }
			set {
				maxAmount = GMath.Max(value, 0);
				if (amount > maxAmount)
					amount = maxAmount;
			}
		}

		/// <summary>Gets or sets if the ammo has been obtained.</summary>
		public bool IsObtained {
			get { return isObtained; }
			set { isObtained = value; }
		}

		/// <summary>Gets or sets if the ammo has been lost.</summary>
		public bool IsLost {
			get { return isLost; }
			set { isLost = value; }
		}

		/// <summary>Gets if the ammo is out.</summary>
		public bool IsEmpty {
			get { return !isAmountBased || amount == 0; }
		}

		/// <summary>Gets if the ammo is at capacity.</summary>
		public bool IsFull {
			get { return !isAmountBased || amount == maxAmount; }
		}
	}
}
