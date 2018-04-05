using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.ResourceData;

namespace ZeldaOracle.Game.Items {
	/// <summary>A data structure for creating ammo in-game.</summary>
	public class AmmoData : BaseResourceData {

		/// <summary>The sprite to use for the ammo in the selection window
		/// and next to the weapon.</summary>
		private ISprite sprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty ammo data.</summary>
		public AmmoData() {
			sprite = new EmptySprite();
		}

		/// <summary>Constructs a copy of the ammo data.</summary>
		public AmmoData(AmmoData copy) : base(copy) {
			sprite = copy.sprite;
		}

		/// <summary>Clones the specified ammo data.</summary>
		public override void Clone(BaseResourceData baseCopy) {
			base.Clone(baseCopy);

			AmmoData copy = (AmmoData) baseCopy;
			sprite = copy.sprite;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected override void InitializeData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, OutputType, Type, previousType);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public override Type OutputType {
			get { return typeof(Ammo); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the sprite for the ammo data.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets the name for the ammo data.</summary>
		public string Name {
			get { return Properties.Get("name", ""); }
			set { Properties.Set("name", value); }
		}

		/// <summary>Gets or sets the description for the ammo data.</summary>
		public string Description {
			get { return Properties.Get("description", ""); }
			set { Properties.Set("description", value); }
		}

		/// <summary>Gets the message when the player picks up this ammo type
		/// for the first time.</summary>
		public string ObtainMessage {
			get { return Properties.Get("obtain_message", ""); }
			set { Properties.Set("obtain_message", value); }
		}

		/// <summary>Gets the message when the player tries to pick up the ammo but
		/// has no container for it.</summary>
		public string CantCollectMessage {
			get { return Properties.Get("cant_collect_message", ""); }
			set { Properties.Set("cant_collect_message", value); }
		}
		
		/// <summary>Gets the message when the player tries to pick up the ammo but
		/// the container is full.</summary>
		public string FullMessage {
			get { return Properties.Get("full_message", ""); }
			set { Properties.Set("full_message", value); }
		}

		/// <summary>Gets or sets if the ammo type is amount-based.</summary>
		public bool IsAmountBased {
			get { return Properties.Get("amount_based", true); }
			set { Properties.Set("amount_based", value); }
		}

		/// <summary>Gets or sets the default amount for the ammo data.</summary>
		public int Amount {
			get { return Properties.Get("amount", 0); }
			set {
				value = GMath.Max(value, 0);
				Properties.Set("amount", value);
			}
		}

		/// <summary>Gets or sets the default max amount for the ammo data.</summary>
		public int MaxAmount {
			get { return Properties.Get("max_amount", 0); }
			set {
				value = GMath.Max(value, 0);
				Properties.Set("max_amount", value);
			}
		}
	}
}
