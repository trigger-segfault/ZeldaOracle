using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardData : BaseItemData {

		private ISprite		sprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the reward data.</summary>
		public RewardData() {
			type		= typeof(Reward);
			sprite		= new EmptySprite();
		}

		/// <summary>Constructs a copy of the reward data.</summary>
		public RewardData(RewardData copy) : base(copy) {
			sprite		= copy.sprite;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets a property list from the base name and values.</summary>
		public void SetPropertyList<T>(string name, T[] values) {
			for (int i = 0; i < values.Length; i++) {
				properties.SetGeneric(ListName(name, i), values[i]);
			}
			// Remove lingering list properties
			for (int i = values.Length; properties.Contains(ListName(name, i)); i++) {
				properties.RemoveProperty(ListName(name, i), false);
			}
		}

		/// <summary>Gets a property list from the base name.</summary>
		public T[] GetPropertyList<T>(string name) {
			int length = 0;
			for (; properties.Contains(ListName(name, length)); length++) ;
			T[] values = new T[length];
			for (int i = 0; i < length; i++) {
				values[i] = properties.Get<T>(ListName(name, i));
			}
			return values;
		}

		/// <summary>Sets the leveled names of the item.</summary>
		public void SetName(params string[] names) {
			SetPropertyList("name", names);
		}

		/// <summary>Sets the leveled descriptions of the item.</summary>
		public void SetDescription(params string[] descriptions) {
			SetPropertyList("description", descriptions);
		}

		/// <summary>Sets the leveled reward messages of the item.</summary>
		public void SetMessage(params string[] messages) {
			SetPropertyList("message", messages);
		}

		/// <summary>Sets the leveled sprites of the item.</summary>
		public void SetSprite(params ISprite[] sprites) {
			this.sprites = sprites;
		}

		/// <summary>Sets the leveled equipped sprites of the item.</summary>
		public void SetEquipSprite(params string[] sprites) {
			SetPropertyList("equip_sprite", sprites);
		}

		/// <summary>Sets the default leveled prices of the item.</summary>
		public void SetDefaultPrice(params int[] prices) {
			SetPropertyList("price", prices);
		}

		/// <summary>Sets the ammo types used by this weapon.</summary>
		public virtual void SetAmmo(params string[] ammoIDs) {
			SetPropertyList("ammo", ammoIDs);
		}

		/// <summary>Sets the max ammo allowed at each level.</summary>
		public void SetMaxAmmo(params int[] maxAmmos) {
			SetPropertyList("max_ammo", maxAmmos);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name for a list property at the specified index.</summary>
		private string ListName(string propertyName, int index) {
			return propertyName + "_" + (index + 1);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Arrays ---------------------------------------------------------------------

		/// <summary>Gets the leveled sprites for the item data.</summary>
		public ISprite[] Sprites {
			get { return sprites; }
		}

		/// <summary>Gets the leveled sprites for the item data.</summary>
		public ISprite[] EquipSprites {
			get {
				string[] spriteNames = GetPropertyList<string>("equip_sprite");
				ISprite[] sprites = new ISprite[spriteNames.Length];
				for (int i = 0; i < sprites.Length; i++) {
					sprites[i] = Resources.Get<ISprite>(spriteNames[i]);
				}
				return sprites;
			}
		}

		/// <summary>Gets the leveled names for the item data.</summary>
		public string[] Names {
			get { return GetPropertyList<string>("name"); }
		}

		/// <summary>Gets the leveled descriptions for the item data.</summary>
		public string[] Descriptions {
			get { return GetPropertyList<string>("description"); }
		}

		/// <summary>Gets the leveled messages for the item data.</summary>
		public string[] Messages {
			get { return GetPropertyList<string>("message"); }
		}

		/// <summary>Gets the leveled prices for the item data.</summary>
		public int[] Prices {
			get { return GetPropertyList<int>("price"); }
		}

		/// <summary>Gets the ammos for the item data.</summary>
		public string[] Ammos {
			get { return GetPropertyList<string>("ammo"); }
		}

		/// <summary>Gets the leveled max ammos for the item data.</summary>
		public int[] MaxAmmos {
			get { return GetPropertyList<int>("max_ammo"); }
		}
		
		// General --------------------------------------------------------------------

		/// <summary>Gets or sets the max level for the item data.</summary>
		public int MaxLevel {
			get { return properties.Get("max_level", Item.Level1); }
			set {
				value = GMath.Max(Item.Level1, value);
				properties.Set("max_level", value);
			}
		}

		/// <summary>Gets or sets the reward hold type for the item data.</summary>
		public RewardHoldTypes HoldType {
			get { return properties.GetEnum("hold_type", RewardHoldTypes.TwoHands); }
			set { properties.SetEnumStr("hold_type", value); }
		}

		/// <summary>Gets or sets if ammo is increased on level-up for the item data.</summary>
		public bool LevelUpAmmo {
			get { return properties.Get("levelup_ammo", false); }
			set { properties.Set("levelup_ammo", value); }
		}
	}
}
