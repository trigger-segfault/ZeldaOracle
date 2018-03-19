using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control.Menus;

namespace ZeldaOracle.Game.Items {
	public class Ammo : ISlotItem {

		private AmmoData	ammoData;
		private string		id;
		private Item		container;

		private string		name;
		private string		description;
		private string		obtainMessage;
		private string		cantCollectMessage;
		private string		fullMessage;
		private ISprite		sprite;

		private bool		isAmountBased;
		private int			maxAmount;

		// TODO: Store these as properties for save format
		private int         amount;
		private bool		isObtained;
		private bool		isLost;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the standard ammo.</summary>
		public Ammo() {
			ammoData		= null;
			id				= "";
			container		= null;
			
			name			= "";
			description		= "";
			obtainMessage		= "";
			cantCollectMessage	= "";
			fullMessage			= "";
			sprite			= new EmptySprite();

			isAmountBased	= true;
			amount			= 0;
			maxAmount		= 0;

			isObtained		= false;
			isLost			= false;
		}

		/// <summary>Constructs the standard ammo with the specified ID.</summary>
		public Ammo(string id) : this() {
			this.id			= id;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ammo from the ammo data.</summary>
		public static Ammo CreateAmmo(AmmoData data) {
			Ammo ammo = ReflectionHelper.Construct<Ammo>(data.Type);

			// Load item data
			ammo.ammoData		= data;
			ammo.id				= data.ResourceName;
			ammo.sprite			= data.Sprite;
			ammo.name			= data.Name;
			ammo.description	= data.Description;
			ammo.obtainMessage		= data.ObtainMessage;
			ammo.cantCollectMessage	= data.CantCollectMessage;
			ammo.fullMessage		= data.FullMessage;
			ammo.isAmountBased	= data.IsAmountBased;
			ammo.Amount			= data.Amount;
			ammo.MaxAmount		= data.MaxAmount;

			return ammo;
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

		/// <summary>Gets the ammo data used to construct this ammo.</summary>
		public AmmoData AmmoData {
			get { return ammoData; }
		}

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

		/// <summary>Gets the message when the player tries to pick up the ammo but
		/// the container is full.</summary>
		public string FullMessage {
			get { return fullMessage; }
			set { fullMessage = value; }
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

		/// <summary>Gets if the ammo has been obtained and is not lost.</summary>
		public bool IsAvailable {
			get { return isObtained && !isLost; }
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
