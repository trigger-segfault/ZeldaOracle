using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using DataOutType =
	System.Collections.Generic.KeyValuePair<System.Type, System.Type>;
using InitializerDictionary =
	System.Collections.Generic.Dictionary<System.Type, System.Reflection.MethodInfo>;
using DataOutDictionary =
	System.Collections.Generic.Dictionary<
	System.Collections.Generic.KeyValuePair<System.Type, System.Type>,
	System.Collections.Generic.Dictionary<System.Type, System.Reflection.MethodInfo>>;

namespace ZeldaOracle.Game.ResourceData {
	/// <summary>The function called to initialize an item type's properties.</summary>
	public delegate void ItemDataInitializer(ItemData data);

	/// <summary>The function called to initialize an reward type's properties.</summary>
	public delegate void RewardDataInitializer(ItemData data);

	/// <summary>The function called to initialize a tile type's properties.</summary>
	public delegate void TileDataInitializer(TileData data);

	/// <summary>The function called to initialize a action type's properties.</summary>
	public delegate void ActionDataInitializer(ActionTileData data);

	/// <summary>A static class used to initialize a resource data type's properties,
	/// events, and other settings using static reflection.</summary>
	public static class ResourceDataInitializing {

		/// <summary>The collection of data types and their collection of initializers.</summary>
		private static Dictionary<DataOutType, InitializerDictionary> dataTypes;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the type initializer dictionaries.</summary>
		static ResourceDataInitializing() {
			dataTypes = new Dictionary<KeyValuePair<Type, Type>, Dictionary<Type, MethodInfo>>();
			RegisterDataType<TileData, Tile>();
			RegisterDataType<TileData, Entity>();
			RegisterDataType<ActionTileData, ActionTile>();
			RegisterDataType<ActionTileData, Entity>();
			RegisterDataType<ItemData, Item>();
			RegisterDataType<AmmoData, Ammo>();
			RegisterDataType<RewardData, Reward>();
		}

		/// <summary>Registers a resource data type that can be used for
		/// initialization.</summary>
		public static void RegisterDataType<DataType, OutType>()
			where DataType : BaseResourceData where OutType : class
		{
			dataTypes.Add(
				new DataOutType(typeof(DataType), typeof(OutType)),
				new InitializerDictionary());
		}


		//-----------------------------------------------------------------------------
		// Initializing
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the data for a resource's types.</summary>
		public static void InitializeData(
			BaseResourceData data, Type newType, Type previousType, Type baseType)
		{
			// No subtype to initialize
			if (newType == null)
				return;

			DataOutType key = new DataOutType(data.GetType(), baseType);

			// Get the correct inheritance types
			Type[] types = null;
			if (previousType != null)
				types = TypeHelper.GetInheritance(previousType, newType, true);
			else
				types = TypeHelper.GetInheritance(baseType, newType, true);

			// Initialize each base type's settings
			// up to and including the new type.
			foreach (Type type in types) {
				if (type == baseType || type == previousType)
					continue;
				InitializeType(key, type, data);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/*private static InitializerDictionary GetInitializers(DataOutType key) {
			InitializerDictionary initializers;
			if (!dataTypes.TryGetValue(key, out initializers)) {
				initializers = new InitializerDictionary();
				dataTypes.Add(key, initializers);
			}
			return initializers;
		}*/

		/// <summary>Initializes the data for a resource's single type.</summary>
		private static void InitializeType(DataOutType key, Type finalType,
			BaseResourceData data)
		{
			InitializerDictionary initializers;
			if (!dataTypes.TryGetValue(key, out initializers)) {
				initializers = new InitializerDictionary();
				dataTypes.Add(key, initializers);
			}
			MethodInfo methodInfo;
			if (!initializers.TryGetValue(key.Key, out methodInfo)) {
				methodInfo = finalType.GetMethod("InitializeData",
						BindingFlags.Static | BindingFlags.Public,
						key.Key);
				initializers.Add(key.Key, methodInfo);
			}
			methodInfo?.Invoke(null, new object[] { data });
		}
	}
}
