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
using ZeldaOracle.Game.ResourceData;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class ItemSR : ScriptReader {

		private enum Modes {
			Root,
			Item,
			Ammo,
		}

		private BaseResourceData baseData;
		private ItemData itemData;
		private AmmoData ammoData;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSR() {
			
			//=====================================================================================
			// ITEM BEGIN/END 
			//=====================================================================================
			AddCommand("ITEM", (int) Modes.Root,
				"string id, string type = \"Item\"",
			delegate (CommandParam parameters) {
				string itemID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				Type type = GameUtil.FindTypeWithBase<Item>(typeName, false);
				
				itemData = new ItemData();
				itemData.ResourceName = itemID;
				itemData.Type = type;

				baseData = itemData;
				
				AddResource<ItemData>(itemID, itemData);
				Mode = Modes.Item;
			});
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Root,
				"string id, string type = \"Ammo\"",
			delegate (CommandParam parameters) {
				string ammoID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				Type type = GameUtil.FindTypeWithBase<Ammo>(typeName, false);
				
				ammoData = new AmmoData();
				ammoData.ResourceName = ammoID;
				ammoData.Type = type;

				baseData = ammoData;
				
				AddResource<AmmoData>(ammoID, ammoData);
				Mode = Modes.Ammo;
			});
			//=====================================================================================
			AddCommand("CLONE", new int[] { (int) Modes.Item, (int) Modes.Ammo },
				"string id",
			delegate (CommandParam parameters) {
				string id = parameters.GetString(0);

				BaseResourceData cloneData = null;
				if (baseData is ItemData) {
					cloneData = GetResource<ItemData>(id);
					if (cloneData.Type != baseData.Type)
						ThrowCommandParseError("Cannot clone items with different types!");
				}
				else if (baseData is AmmoData) {
					cloneData = GetResource<AmmoData>(id);
					if (cloneData.Type != baseData.Type)
						ThrowCommandParseError("Cannot clone ammos with different types!");
				}
				baseData.Clone(cloneData);
			});
			//=====================================================================================
			AddCommand("END", new int[] { (int) Modes.Item, (int) Modes.Ammo },
				"",
			delegate (CommandParam parameters) {
				itemData = null;
				ammoData = null;
				baseData = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// GENERAL BUILDING
			//=====================================================================================
			AddCommand("PROPERTY", new int[] { (int) Modes.Item, (int) Modes.Ammo },
				"(string name, var value)",
			delegate (CommandParam parameters) {
				SetProperty(baseData, parameters);
			});
			//=====================================================================================
			// ITEM BUILDING
			//=====================================================================================
			AddCommand("MAXLEVEL", (int) Modes.Item,
				"int maxLevel",
			delegate (CommandParam parameters) {
				itemData.MaxLevel = parameters.GetInt(0) - 1;
			});
			//=====================================================================================
			AddCommand("LEVELUPAMMO", (int) Modes.Item,
				"bool ammoOnLevelUp",
			delegate (CommandParam parameters) {
				itemData.LevelUpAmmo = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Item,
				"string names...",
			delegate (CommandParam parameters) {
				string[] names = new string[parameters.ChildCount];
				for (int i = 0; i < names.Length; i++)
					names[i] = parameters.GetString(i);
				itemData.SetName(names);
			});
			//=====================================================================================
			AddCommand("DESCRIPTION", (int) Modes.Item,
				"string descriptions...",
			delegate (CommandParam parameters) {
				string[] descriptions = new string[parameters.ChildCount];
				for (int i = 0; i < descriptions.Length; i++)
					descriptions[i] = parameters.GetString(i);
				itemData.SetDescription(descriptions);
			});
			//=====================================================================================
			AddCommand("MESSAGE", (int) Modes.Item,
				"string messages...",
			delegate (CommandParam parameters) {
				string[] messages = new string[parameters.ChildCount];
				for (int i = 0; i < messages.Length; i++)
					messages[i] = parameters.GetString(i);
				itemData.SetMessage(messages);
			});
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Item,
				"string sprites...",
			delegate (CommandParam parameters) {
				ISprite[] sprites = new ISprite[parameters.ChildCount];
				for (int i = 0; i < sprites.Length; i++)
					sprites[i] = GetResource<ISprite>(parameters.GetString(i));
				itemData.SetSprite(sprites);
			});
			//=====================================================================================
			AddCommand("PRICE", (int) Modes.Item,
				"int prices...",
			delegate (CommandParam parameters) {
				int[] prices = new int[parameters.ChildCount];
				for (int i = 0; i < prices.Length; i++)
					prices[i] = parameters.GetInt(i);
				itemData.SetDefaultPrice(prices);
			});
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Item,
				"string ammos...",
			delegate (CommandParam parameters) {
				string[] ammoNames = new string[parameters.ChildCount];
				for (int i = 0; i < ammoNames.Length; i++) {
					// Make sure the resource exists
					GetResource<AmmoData>(parameters.GetString(i));
					ammoNames[i] = parameters.GetString(i);
				}
				itemData.SetAmmo(ammoNames);
			});
			//=====================================================================================
			AddCommand("MAXAMMO", (int) Modes.Item,
				"int maxAmmos...",
			delegate (CommandParam parameters) {
				int[] maxAmmos = new int[parameters.ChildCount];
				for (int i = 0; i < maxAmmos.Length; i++)
					maxAmmos[i] = parameters.GetInt(i);
				itemData.SetMaxAmmo(maxAmmos);
			});
			//=====================================================================================
			AddCommand("HOLDTYPE", (int) Modes.Item,
				"string rewardHoldType",
			delegate (CommandParam parameters) {
				itemData.HoldType = parameters.GetEnum<RewardHoldTypes>(0, false);
			});
			//=====================================================================================
			// EQUIPMENT BUILDING
			//=====================================================================================
			AddCommand("EQUIPSPRITE", (int) Modes.Item,
				"string sprites...",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<ItemEquipment>(itemData.Type))
					ThrowCommandParseError("Can only call EQUIPSPRITE when constructing an " +
						"ItemEquipment!");
				string[] spriteNames = new string[parameters.ChildCount];
				for (int i = 0; i < spriteNames.Length; i++) {
					// Make sure the resource exists
					GetResource<ISprite>(parameters.GetString(i));
					spriteNames[i] = parameters.GetString(i);
				}
				itemData.SetEquipSprite(spriteNames);
			});
			//=====================================================================================
			// SECONDARY BUILDING
			//=====================================================================================
			AddCommand("SECONDSLOT", (int) Modes.Item,
				"Point slot",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<ItemSecondary>(itemData.Type))
					ThrowCommandParseError("Can only call SECONDSLOT when constructing an " +
						"ItemSecondary!");
				itemData.Properties.Set("slot", parameters.GetPoint(0));
			});
			//=====================================================================================
			// ESSENCE BUILDING
			//=====================================================================================
			AddCommand("ESSENCESLOT", (int) Modes.Item,
				"int slot",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<ItemEssence>(itemData.Type))
					ThrowCommandParseError("Can only call ESSENCESLOT when constructing an " +
						"ItemEssence!");
				itemData.Properties.Set("slot", parameters.GetInt(0));
			});
			//=====================================================================================
			// AMMO BUILDING
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Ammo,
				"string name",
			delegate (CommandParam parameters) {
				ammoData.Name = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("DESCRIPTION", (int) Modes.Ammo,
				"string description",
			delegate (CommandParam parameters) {
				ammoData.Description = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("OBTAIN", (int) Modes.Ammo,
				"string obtainMessage",
			delegate (CommandParam parameters) {
				ammoData.ObtainMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("CANTCOLLECT", (int) Modes.Ammo,
				"string cantCollectMessage",
			delegate (CommandParam parameters) {
				ammoData.CantCollectMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("FULL", (int) Modes.Ammo,
				"string fullMessage",
			delegate (CommandParam parameters) {
				ammoData.FullMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Ammo,
				"string sprite",
			delegate (CommandParam parameters) {
				ammoData.Sprite = GetResource<ISprite>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("AMOUNTBASED", (int) Modes.Ammo,
				"bool isAmountBased",
			delegate (CommandParam parameters) {
				ammoData.IsAmountBased = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("AMOUNT", (int) Modes.Ammo,
				"int amount",
			delegate (CommandParam parameters) {
				ammoData.Amount = parameters.GetInt(0);
			});
			//=====================================================================================
			AddCommand("MAXAMOUNT", (int) Modes.Ammo,
				"int maxAmount",
			delegate (CommandParam parameters) {
				ammoData.MaxAmount = parameters.GetInt(0);
			});
			//=====================================================================================

		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			itemData = null;
			ammoData = null;
			baseData = null;
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
		
		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
