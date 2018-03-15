using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward that gives a player a specified amount of ammo.</summary>
	public class RewardAmmo : Reward {

		/// <summary>The ID of the ammo to give.</summary>
		private string ammoID;
		/// <summary>The amount of the ammo to give.</summary>
		private int amount;
		/// <summary>The message displayed when the ammo container is already full.</summary>
		private string fullMessage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ammo reward.</summary>
		public RewardAmmo(string id, string ammoID, int amount, string message,
			ISprite sprite) : base(id)
		{
			Sprite			= sprite;
			Message			= message;
			HoldType		= RewardHoldTypes.Raise;
			HasDuration		= true;
			ShowMessageOnPickup			= false;
			IsCollectibleWithWeapons	= true;

			this.ammoID			= ammoID;
			this.amount			= amount;
			fullMessage			= "";
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the reward is being initialized.</summary>
		protected override void OnInitialize() {
			ObtainMessage		= Ammo.ObtainMessage;
			CantCollectMessage	= Ammo.CantCollectMessage;
		}

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			Ammo.Amount += amount;
			AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the reward is a valid for item drops.</summary>
		public override bool IsAvailable {
			get { return Inventory.IsAmmoAvailable(ammoID); }
		}

		/// <summary>Gets if the reward can be collected.</summary>
		public override bool CanCollect {
			get { return Inventory.IsAmmoContainerAvailable(ammoID); }
		}

		/// <summary>Gets the appropriate message to display when collecting the
		/// reward.</summary>
		public override string AppropriateMessage {
			get {
				string text = Message;
				if (!CanCollect && !string.IsNullOrWhiteSpace(CantCollectMessage))
					text = CantCollectMessage;
				else if (Ammo.IsFull && !string.IsNullOrWhiteSpace(FullMessage))
					text = FullMessage;
				return text;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the amount of ammo given with this reward.</summary>
		public int Amount {
			get { return amount; }
			set { amount = value; }
		}

		/// <summary>Gets the ammo associated with this reward.</summary>
		public Ammo Ammo {
			get { return Inventory.GetAmmo(ammoID);  }
		}

		/// <summary>Gets the message displayed when the ammo is already at capacity.</summary>
		public string FullMessage {
			get { return fullMessage; }
			set { fullMessage = value; }
		}
	}
}
