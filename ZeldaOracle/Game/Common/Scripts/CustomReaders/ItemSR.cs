using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Scripts.CustomReaders {


	public class ItemSR : ScriptReader {

		private enum Modes {
			Root,
			Item,
			Ammo,
		}

		private Item item;
		private ItemEquipment equipment;
		private ItemWeapon weapon;
		private ItemSecondary secondary;
		private ItemEssence essence;

		private Ammo ammo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSR() {

			//=====================================================================================
			// Type Definitions
			//=====================================================================================
			AddType("Sprite",
				"string spriteName",
				"(string animationName, int substrip)",
				"(string spriteName, string definition)"
			);
			//=====================================================================================
			// ITEM BEGIN/END 
			//=====================================================================================
			AddCommand("ITEM", (int) Modes.Root,
				"string id, string type = \"\"",
			delegate (CommandParam parameters) {
				string itemID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				if (string.IsNullOrWhiteSpace(typeName))
					typeName = typeof(Item).Name;
				Type type = GameUtil.FindTypeWithBase<Item>(typeName, false);

				item = ReflectionHelper.Construct<Item, string>(type, itemID);

				if (TypeHelper.TypeHasBase<ItemEquipment>(type))
					equipment = (ItemEquipment) item;
				if (TypeHelper.TypeHasBase<ItemWeapon>(type))
					weapon = (ItemWeapon) item;
				if (TypeHelper.TypeHasBase<ItemSecondary>(type))
					secondary = (ItemSecondary) item;
				if (TypeHelper.TypeHasBase<ItemEssence>(type))
					essence = (ItemEssence) item;

				AddResource<Item>(itemID, item);
				Mode = Modes.Item;
			});
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Root,
				"string id, string type = \"\"",
			delegate (CommandParam parameters) {
				string ammoID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				if (string.IsNullOrWhiteSpace(typeName))
					typeName = typeof(Ammo).Name;
				Type type = GameUtil.FindTypeWithBase<Ammo>(typeName, false);

				ammo = ReflectionHelper.Construct<Ammo, string>(type, ammoID);

				AddResource<Ammo>(ammoID, ammo);
				Mode = Modes.Ammo;
			});
			//=====================================================================================
			AddCommand("END", new int[] { (int) Modes.Item, (int) Modes.Ammo },
				"",
			delegate (CommandParam parameters) {
				item = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// ITEM BUILDING
			//=====================================================================================
			AddCommand("MAXLEVEL", (int) Modes.Item,
				"int maxLevel",
			delegate (CommandParam parameters) {
				item.MaxLevel = parameters.GetInt(0) - 1;
			});
			//=====================================================================================
			AddCommand("LEVELUPAMMO", (int) Modes.Item,
				"bool ammoOnLevelUp",
			delegate (CommandParam parameters) {
				item.IncreaseAmmoOnLevelUp = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Item,
				"string names...",
			delegate (CommandParam parameters) {
				string[] names = new string[parameters.ChildCount];
				for (int i = 0; i < names.Length; i++)
					names[i] = parameters.GetString(i);
				item.SetName(names);
			});
			//=====================================================================================
			AddCommand("DESCRIPTION", (int) Modes.Item,
				"string descriptions...",
			delegate (CommandParam parameters) {
				string[] descriptions = new string[parameters.ChildCount];
				for (int i = 0; i < descriptions.Length; i++)
					descriptions[i] = parameters.GetString(i);
				item.SetDescription(descriptions);
			});
			//=====================================================================================
			AddCommand("MESSAGE", (int) Modes.Item,
				"string messages...",
			delegate (CommandParam parameters) {
				string[] messages = new string[parameters.ChildCount];
				for (int i = 0; i < messages.Length; i++)
					messages[i] = parameters.GetString(i);
				item.SetMessage(messages);
			});
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Item,
				"Sprite sprites...",
			delegate (CommandParam parameters) {
				ISprite[] sprites = new ISprite[parameters.ChildCount];
				for (int i = 0; i < sprites.Length; i++)
					sprites[i] = GetSpriteFromParams(parameters, i);
				item.SetSprite(sprites);
			});
			//=====================================================================================
			AddCommand("PRICE", (int) Modes.Item,
				"int prices...",
			delegate (CommandParam parameters) {
				int[] prices = new int[parameters.ChildCount];
				for (int i = 0; i < prices.Length; i++)
					prices[i] = parameters.GetInt(i);
				item.SetDefaultPrice(prices);
			});
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Item,
				"string ammos...",
			delegate (CommandParam parameters) {
				Ammo[] ammos = new Ammo[parameters.ChildCount];
				for (int i = 0; i < ammos.Length; i++)
					ammos[i] = GetResource<Ammo>(parameters.GetString(i));
				item.SetAmmo(ammos);
			});
			//=====================================================================================
			AddCommand("MAXAMMO", (int) Modes.Item,
				"int maxAmmos...",
			delegate (CommandParam parameters) {
				int[] maxAmmos = new int[parameters.ChildCount];
				for (int i = 0; i < maxAmmos.Length; i++)
					maxAmmos[i] = parameters.GetInt(i);
				item.SetMaxAmmo(maxAmmos);
			});
			//=====================================================================================
			AddCommand("HOLDTYPE", (int) Modes.Item,
				"string rewardHoldType",
			delegate (CommandParam parameters) {
				item.HoldType = parameters.GetEnum<RewardHoldTypes>(0, false);
			});
			//=====================================================================================
			// EQUIPMENT BUILDING
			//=====================================================================================
			AddCommand("EQUIPSPRITE", (int) Modes.Item,
				"Sprite sprites...",
			delegate (CommandParam parameters) {
				if (equipment == null)
					ThrowCommandParseError("Can only call EQUIPSPRITE when constructing an " +
						"ItemEquipment!");
				ISprite[] sprites = new ISprite[parameters.ChildCount];
				for (int i = 0; i < sprites.Length; i++)
					sprites[i] = GetSpriteFromParams(parameters, i);
				equipment.SetSpriteEquipped(sprites);
			});
			//=====================================================================================
			// SECONDARY BUILDING
			//=====================================================================================
			AddCommand("SECONDSLOT", (int) Modes.Item,
				"Point slot",
			delegate (CommandParam parameters) {
				if (secondary == null)
					ThrowCommandParseError("Can only call SECONDSLOT when constructing an " +
						"ItemSecondary!");
				secondary.SecondarySlot = parameters.GetPoint(0);
			});
			//=====================================================================================
			// ESSENCE BUILDING
			//=====================================================================================
			AddCommand("ESSENCESLOT", (int) Modes.Item,
				"int slot",
			delegate (CommandParam parameters) {
				if (essence == null)
					ThrowCommandParseError("Can only call ESSENCESLOT when constructing an " +
						"ItemEssence!");
				essence.EssenceSlot = parameters.GetInt(0);
			});
			//=====================================================================================
			// AMMO BUILDING
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Ammo,
				"string name",
			delegate (CommandParam parameters) {
				ammo.Name = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("DESCRIPTION", (int) Modes.Ammo,
				"string description",
			delegate (CommandParam parameters) {
				ammo.Description = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("OBTAIN", (int) Modes.Ammo,
				"string obtainMessage",
			delegate (CommandParam parameters) {
				ammo.ObtainMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("CANTCOLLECT", (int) Modes.Ammo,
				"string cantCollectMessage",
			delegate (CommandParam parameters) {
				ammo.CantCollectMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Ammo,
				"Sprite sprite",
			delegate (CommandParam parameters) {
				ammo.Sprite = GetSpriteFromParams(parameters, 0);
			});
			//=====================================================================================
			AddCommand("AMOUNTBASED", (int) Modes.Ammo,
				"bool isAmountBased",
			delegate (CommandParam parameters) {
				ammo.IsAmountBased = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("MAXAMOUNT", (int) Modes.Ammo,
				"int maxAmount",
			delegate (CommandParam parameters) {
				ammo.MaxAmount = parameters.GetInt(0);
			});
			//=====================================================================================

		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(string name) {
			ISprite sprite = GetResource<ISprite>(name);
			if (sprite == null) {
				ThrowCommandParseError("Sprite with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(ISpriteSource source, Point2I index) {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			ISprite sprite = source.GetSprite(index);
			if (sprite == null) {
				ThrowCommandParseError("Sprite at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(string name) where T : class, ISprite {
			T sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(ISpriteSource source, Point2I index) where T : class, ISprite {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			T sprite = source.GetSprite(index) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(string name, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(name), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(ISpriteSource source, Point2I index, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(source, index), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(DefinitionSprite sprite, string definition) {
			ISprite defSprite = sprite.Get(definition);
			if (defSprite == null)
				ThrowCommandParseError("Defined sprite with definition '" + definition + "' does not exist!");
			return defSprite;
		}

		/// <summary>Gets the sprite from one of the many parameter overloads.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex = 0) {
			ISpriteSource source;
			Point2I index;
			string definition;
			return GetSpriteFromParams(param, startIndex, out source, out index, out definition);
		}

		/// <summary>Gets the sprite from one of the many parameter overloads and returns the source.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex, out ISpriteSource source, out Point2I index, out string definition) {
			// 1: string spriteName
			// 3: (string animationName, int substrip)
			// 4: (string spriteName, string definition)
			source = null;
			index = Point2I.Zero;
			definition = null;

			var param0 = param.GetParam(startIndex);
			if (param0.Type == CommandParamType.String) {
				// Overload 1:
				return GetResource<ISprite>(param.GetString(startIndex));
			}
			else {
				if (param0.GetParam(1).IsValidType(CommandParamType.Integer)) {
					// Overload 3:
					return GetSprite<Animation>(param0.GetString(0)).GetSubstrip(param0.GetInt(1));
				}
				else {
					// Overload 4:
					return GetDefinedSprite(param0.GetString(0), param0.GetString(1));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			item     = null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new ItemSR();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the source as a sprite sheet.</summary>
		/*private SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}*/

		/// <summary>Gets the source as a sprite set.</summary>
		/*private SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}*/

		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
