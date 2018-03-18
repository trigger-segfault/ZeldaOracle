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
		private Ammo ammo;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ammo reward.</summary>
		public RewardAmmo() {
			Sprite			= new EmptySprite();
			Message			= "";
			HoldInChest		= false;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= true;
			ShowPickupMessage	= false;
			InteractWithWeapons	= true;

			ammo			= null;
			Amount			= 0;
			FullMessage		= "";
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the reward is being initialized.</summary>
		protected override void OnInitialize() {
			base.OnInitialize();
			AmmoData ammoData = RewardData.AmmoData;
			if (ammoData == null)
				throw new Exception("Could not find ammo!");
			ammo = Inventory.GetAmmo(ammoData.ResourceName);
			if (ammo == null)
				throw new Exception("Could not find ammo!");
		}

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			ammo.Amount += Amount;
			AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the reward is a valid for item drops.</summary>
		public override bool IsAvailable {
			get { return ammo.IsAvailable; }
		}

		/// <summary>Gets if the reward can be collected.</summary>
		public override bool CanCollect {
			get { return ammo.IsContainerAvailable; }
		}

		/// <summary>Gets if the reward is already at capacity.</summary>
		public override bool IsFull {
			get { return ammo.IsFull; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the ammo associated with this reward.</summary>
		public Ammo Ammo {
			get { return ammo; }
			set { ammo = value; }
		}
	}
}
