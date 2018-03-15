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
	public class RewardAmmo : RewardAmount {

		/// <summary>The ID of the ammo to give.</summary>
		private string ammoID;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ammo reward.</summary>
		public RewardAmmo(string id) : this(id, true) {
		}
		
		/// <summary>Constructs the ammo reward.</summary>
		protected RewardAmmo(string id, bool addPrefix)
			: base((addPrefix ? "ammo_" : "") + id)
		{
			Sprite			= new EmptySprite();
			Message			= "";
			HoldInChest		= false;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= true;
			ShowMessageOnPickup			= false;
			InteractWithWeapons	= true;

			ammoID			= "";
			Amount			= 0;
			FullMessage		= "";
		}

		/// <summary>Constructs the ammo reward.</summary>
		public RewardAmmo(string id, string ammoID, int amount, string message,
			ISprite sprite) : base(id)
		{
			Sprite			= sprite;
			Message			= message;
			HoldInChest		= false;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= true;
			ShowMessageOnPickup			= false;
			InteractWithWeapons	= true;

			this.ammoID		= ammoID;
			Amount			= amount;
			FullMessage		= "";
		}

		/// <summary>Clones the data for the specified reward.</summary>
		public override void Clone(Reward reward) {
			base.Clone(reward);

			if (reward is RewardAmmo) {
				var rewardAmmo = (RewardAmmo) reward;

				ammoID		= rewardAmmo.ammoID;
			}
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
			Ammo.Amount += Amount;
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

		/// <summary>Gets if the reward is already at capacity.</summary>
		public override bool IsFull {
			get { return Ammo.IsFull; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the ammo associated with this reward.</summary>
		public Ammo Ammo {
			get { return Inventory.GetAmmo(ammoID); }
			set { ammoID = value.ID; }
		}
	}
}
