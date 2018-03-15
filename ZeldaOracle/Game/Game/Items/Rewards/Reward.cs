using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>The base class for all player rewards.</summary>
	public abstract class Reward : ZeldaAPI.Reward {

		private RewardManager	rewardManager;
		private string			id;

		private Properties		properties;

		private ISprite			sprite;
		private string			message;
		private string			obtainMessage;
		private string			cantCollectMessage;
		private bool			holdInChest;
		private RewardHoldTypes	holdType;
		private bool			hasDuration;
		private bool			showMessageOnPickup;
		private bool			interactWithWeapons;
		private Sound			bounceSound;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base reward.</summary>
		public Reward(string id) {
			this.id				= id;
			rewardManager		= null;

			sprite				= null;
			message				= "";
			obtainMessage		= "";
			cantCollectMessage	= "";
			holdInChest			= true;
			holdType			= RewardHoldTypes.TwoHands;
			hasDuration			= false;
			showMessageOnPickup	= false;
			interactWithWeapons	= false;
			bounceSound			= null;
		}

		/// <summary>Clones the data for the specified reward.</summary>
		public virtual void Clone(Reward reward) {
			sprite				= reward.sprite;
			message				= reward.message;
			obtainMessage		= reward.obtainMessage;
			cantCollectMessage	= reward.cantCollectMessage;
			holdInChest			= reward.holdInChest;
			holdType			= reward.holdType;
			hasDuration			= reward.hasDuration;
			showMessageOnPickup	= reward.showMessageOnPickup;
			interactWithWeapons	= reward.interactWithWeapons;
			bounceSound			= reward.bounceSound;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the reward.</summary>
		public void Initialize(RewardManager rewardManager) {
			this.rewardManager = rewardManager;
			OnInitialize();
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the reward is being initialized.</summary>
		protected virtual void OnInitialize() { }

		/// <summary>Called when the player collects the reward.</summary>
		public virtual void OnCollect() { }

		/// <summary>When overridden, collecting with no message will have different
		/// functionality.</summary>
		public virtual void OnCollectNoMessage() {
			OnCollect();
		}

		/// <summary>Displays the reward message.</summary>
		public virtual void OnDisplayMessage() {
			GameControl.DisplayMessage(AppropriateMessage);
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets if the reward is a valid for item drops.</summary>
		public virtual bool IsAvailable {
			get { return true; }
		}

		/// <summary>Gets if the reward can be collected.</summary>
		public virtual bool CanCollect {
			get { return true; }
		}

		/// <summary>Gets the appropriate message to display when collecting the
		/// reward.</summary>
		public virtual string AppropriateMessage {
			get {
				string text = message;
				if (!CanCollect && !string.IsNullOrWhiteSpace(CantCollectMessage))
					text = CantCollectMessage;
				return text;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override string ToString() {
			return id;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the reward manager for the reward.</summary>
		public RewardManager RewardManager {
			get { return rewardManager; }
			set { rewardManager = value; }
		}

		/// <summary>Get the inventory for the game.</summary>
		public Inventory Inventory {
			get { return rewardManager.Inventory; }
		}

		/// <summary>Gets the current game control.</summary>
		public GameControl GameControl {
			get { return rewardManager.GameControl; }
		}

		/// <summary>Gets the current room control.</summary>
		public RoomControl RoomControl {
			get { return rewardManager.GameControl.RoomControl; }
		}

		/// <summary>Gets the current player.</summary>
		public Player Player {
			get { return rewardManager.GameControl.Player; }
		}

		/// <summary>Gets the ID for the reward.</summary>
		public string ID {
			get { return id; }
		}

		/// <summary>Gets the sprite for the reward.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set {
				sprite = value;
				Rectangle2I bounds = value.Bounds;
				if (bounds.Width != 16 && bounds.X == 0) {
					sprite = new OffsetSprite(value,
						new Point2I((16 - bounds.Width) / 2, 0));
				}
			}
		}

		/// <summary>Gets the message for the reward.</summary>
		public string Message {
			get { return message; }
			set { message = value; }
		}

		/// <summary>Gets the message displayed when the reward is first obtained.
		/// If this value is not set then the regular message will be shown.</summary>
		public string ObtainMessage {
			get { return obtainMessage; }
			set { obtainMessage = value; }
		}

		/// <summary>Gets the message displayed when the reward cannot be collected.</summary>
		public string CantCollectMessage {
			get { return cantCollectMessage; }
			set { cantCollectMessage = value; }
		}

		/// <summary>Gets if the reward does not rise out of chests and instead, is
		/// picked up by the player.</summary>
		public bool HoldInChest {
			get { return holdInChest; }
			set { holdInChest = value; }
		}

		/// <summary>Gets the method for holding this reward when in the reward state.</summary>
		public RewardHoldTypes HoldType {
			get { return holdType; }
			set { holdType = value; }
		}

		/// <summary>Gets if the reward entity has a duration.</summary>
		public bool HasDuration {
			get { return hasDuration; }
			set { hasDuration = value; }
		}

		/// <summary>Gets if the reward message is shown when the item is picked up
		/// from the ground.</summary>
		public bool ShowMessageOnPickup {
			get { return showMessageOnPickup; }
			set { showMessageOnPickup = value; }
		}

		/// <summary>Gets if this reward can be collected with weapons like the
		/// sword, boomerange, and switch hook.</summary>
		public bool InteractWithWeapons {
			get { return interactWithWeapons; }
			set { interactWithWeapons = value; }
		}

		/// <summary>Gets the bounce sound for the reward's entity.</summary>
		public Sound BounceSound {
			get { return bounceSound; }
			set { bounceSound = value; }
		}
	}
}
