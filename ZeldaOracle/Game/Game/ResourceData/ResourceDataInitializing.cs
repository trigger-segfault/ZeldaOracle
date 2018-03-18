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
using OutDictionary =
	System.Collections.Generic.Dictionary<System.Type,
		ZeldaOracle.Game.ResourceData.DelegateLookupInfo>;

namespace ZeldaOracle.Game.ResourceData {
	/// <summary>A static class used to initialize a resource data type's properties,
	/// events, and other settings using static reflection.</summary>
	public static class ResourceDataInitializing {
		
		/// <summary>The collection of data types and their collection of initializers.</summary>
		private static Dictionary<Type, OutDictionary> dataTypes;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the type initializer dictionaries.</summary>
		static ResourceDataInitializing() {
			dataTypes = new Dictionary<Type, OutDictionary>();
			RegisterDataType<TileData, Tile>("TileData");
			RegisterDataType<TileData, Entity>("TileData");
			RegisterDataType<ActionTileData, ActionTile>("TileData");
			RegisterDataType<ActionTileData, Entity>("TileData");

			RegisterDataType<ItemData, Item>("ItemData");
			RegisterDataType<AmmoData, Ammo>("AmmoData");
			RegisterDataType<RewardData, Reward>("RewardData");
		}

		/// <summary>Registers a resource data type that can be used for
		/// initialization.</summary>
		public static void RegisterDataType<TData, TOut>(string functionPostfix)
			where TData : BaseResourceData where TOut : class
		{
			/*dataTypes.Add(
				new DataOutType(typeof(TData), typeof(TOut)),
				new DelegateLookupInfo("Initialize" + functionPostfix,
					typeof(Action<TData>)));*/
			OutDictionary dictionary;
			if (!dataTypes.TryGetValue(typeof(TData), out dictionary)) {
				dictionary = new OutDictionary();
				dataTypes.Add(typeof(TData), dictionary);
			}
			dictionary.Add(typeof(TOut),
				new DelegateLookupInfo("Initialize" + functionPostfix,
				typeof(Action<TData>)));
		}


		//-----------------------------------------------------------------------------
		// Initializing
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the data for a resource's types.</summary>
		public static void InitializeData<TData>(TData data, Type baseType,
			Type newType, Type previousType) where TData : BaseResourceData
		{
			// No subtype to initialize
			if (newType == null)
				return;
			
			// Get the correct inheritance types
			Type[] types = null;
			if (previousType != null)
				types = TypeHelper.GetInheritance(previousType, newType, true);
			else
				types = TypeHelper.GetInheritance(baseType, newType, true);

			// Initialize each base type's settings
			// up to and including the new type.
			OutDictionary dictionary = dataTypes[typeof(TData)];
			DelegateLookupInfo info = dictionary[baseType];
			Delegate func;
			foreach (Type type in types) {
				if (type == baseType || type == previousType)
					continue;
				if (!info.Delegates.TryGetValue(type, out func)) {
					MethodInfo methodInfo = type.GetMethod(info.FunctionName,
							BindingFlags.Static | BindingFlags.Public,
							info.Parameters);
					if (methodInfo != null) {
						func = ReflectionHelper.GetFunction(info.DelegateType,
							methodInfo);
					}
					info.Delegates.Add(type, func);
				}
				// It's important to call the function as a delegate known at
				// compile time, otherwise the function call will be extremely slow.
				((Action<TData>) func)?.Invoke(data);
			}
		}
	}
}
