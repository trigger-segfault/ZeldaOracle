using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.ResourceData;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardData : BaseResourceData {

		private ISprite		sprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the reward data.</summary>
		public RewardData() {
			sprite		= new EmptySprite();
		}

		/// <summary>Constructs a copy of the reward data.</summary>
		public RewardData(RewardData copy) : base(copy) {
			sprite		= copy.sprite;
		}

		/// <summary>Clones the specified reward data.</summary>
		public override void Clone(BaseResourceData baseCopy) {
			base.Clone(baseCopy);

			RewardData copy = (RewardData) baseCopy;
			sprite = copy.sprite;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		public override void InitializeData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, OutputType, Type, previousType);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public override Type OutputType {
			get { return typeof(Reward); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the sprite for the reward data.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets the normal message when the player picks up this
		/// reward type.</summary>
		public string Message {
			get { return Properties.Get("message", ""); }
			set { Properties.Set("message", value); }
		}

		/// <summary>Gets or sets the message when the player picks up this reward type
		/// for the first time.</summary>
		public string ObtainMessage {
			get { return Properties.Get("obtain_message", ""); }
			set { Properties.Set("obtain_message", value); }
		}

		/// <summary>Gets or sets the message when the player tries to pick up the
		/// reward but has no container for it.</summary>
		public string CantCollectMessage {
			get { return Properties.Get("cant_collect_message", ""); }
			set { Properties.Set("cant_collect_message", value); }
		}

		/// <summary>Gets the message when the player tries to pick up the reward but
		/// the container is full.</summary>
		public string FullMessage {
			get { return Properties.Get("full_message", ""); }
			set { Properties.Set("full_message", value); }
		}

		/// <summary>Gets or sets if the reward does not rise out of chests and
		/// instead, is picked up by the player.</summary>
		public bool HoldInChest {
			get { return Properties.Get("hold_in_chest", true); }
			set { Properties.Set("hold_in_chest", value); }
		}

		/// <summary>Gets or sets the reward hold type for the reward data.</summary>
		public RewardHoldTypes HoldType {
			get { return properties.GetEnum("hold_type", RewardHoldTypes.TwoHands); }
			set { properties.SetEnumStr("hold_type", value); }
		}

		/// <summary>Gets or sets if the reward entity has a duration.</summary>
		public bool HasDuration {
			get { return Properties.Get("has_duration", false); }
			set { Properties.Set("has_duration", value); }
		}

		/// <summary>Gets or sets if the reward message is shown when the item is
		/// picked up from the ground.</summary>
		public bool ShowPickupMessage {
			get { return Properties.Get("show_pickup_message", false); }
			set { Properties.Set("show_pickup_message", value); }
		}

		/// <summary>Gets or sets if this reward can be collected with weapons like the
		/// sword, boomerange, and switch hook.</summary>
		public bool WeaponInteract {
			get { return Properties.Get("weapon_interact", false); }
			set { Properties.Set("weapon_interact", value); }
		}
		
		/// <summary>Gets or sets the amount of the reward given with this reward.</summary>
		public int Amount {
			get { return Properties.Get("amount", 0); }
			set { Properties.Set("amount", value); }
		}

		/// <summary>Gets or sets the ammo data tied to the reward.</summary>
		public AmmoData AmmoData {
			get { return Properties.GetResource<AmmoData>("ammo", null); }
			set {
				Properties.Set("ammo", value.ResourceName);
				ObtainMessage = value.ObtainMessage;
				CantCollectMessage = value.CantCollectMessage;
				FullMessage = value.FullMessage;
			}
		}

		/// <summary>Gets or sets the bounce sound tied to the reward.</summary>
		public Sound BounceSound {
			get { return Properties.GetResource<Sound>("bounce_sound", null); }
			set { Properties.Set("bounce_sound", value.Name); }
		}
	}
}
