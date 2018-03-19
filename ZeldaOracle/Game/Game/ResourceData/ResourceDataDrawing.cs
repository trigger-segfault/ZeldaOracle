using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics;
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
	/// <summary>A static class used to draw a resource data type in the editor
	/// by using static reflection.</summary>
	public static class ResourceDataDrawing {

		/// <summary>The collection of data types and their collection of drawers.</summary>
		private static Dictionary<Type, OutDictionary> dataTypes;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the type drawer dictionaries.</summary>
		static ResourceDataDrawing() {
			dataTypes = new Dictionary<Type, OutDictionary>();
			RegisterDataType<TileData, Tile, TileDataDrawer>("TileData");
			RegisterDataType<TileData, Entity, TileDataDrawer>("TileData");
			RegisterDataType<ActionTileData, ActionTile, ActionDataDrawer>("TileData");
			RegisterDataType<ActionTileData, Entity, ActionDataDrawer>("TileData");
		}

		/// <summary>Registers a resource data type that can be used for
		/// drawing. Graphics2D is automatically inserted as the first parameter.</summary>
		public static void RegisterDataType<TData, TOut, TDelegate>(
			string functionPostfix)
			where TData : BaseResourceData where TOut : class
		{
			OutDictionary dictionary;
			if (!dataTypes.TryGetValue(typeof(TData), out dictionary)) {
				dictionary = new OutDictionary();
				dataTypes.Add(typeof(TData), dictionary);
			}
			dictionary.Add(typeof(TOut),
				new DelegateLookupInfo("Draw" + functionPostfix, typeof(TDelegate)));
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the drawing method for the base resource data.
		/// Uses types supplied by data. It's important to call the function as a
		/// delegate known at compile time, otherwise the function call will be
		/// extremely slow.</summary>
		public static TDelegate GetDrawer<TDelegate>(BaseResourceData data)
			where TDelegate : class
		{
			return GetDrawer<TDelegate>(data, data.OutputType, data.Type);
		}

		/// <summary>Gets the drawing method for the base resource data.
		/// Uses types specified in parameters. It's important to call the function
		/// as a delegate known at compile time, otherwise the function call will be
		/// extremely slow.</summary>
		public static TDelegate GetDrawer<TDelegate>(BaseResourceData data,
			Type baseType, Type finalType) where TDelegate : class
		{
			// No subtype to initialize
			if (finalType == null)
				return null;
			
			// Get the correct inheritance types
			Type[] types = TypeHelper.GetInheritance(baseType, finalType, false);

			// Initialize each base type's settings
			// up to and including the new type.
			OutDictionary dictionary = dataTypes[data.GetType()];
			DelegateLookupInfo info = dictionary[baseType];
			Delegate func;
			foreach (Type type in types) {
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
				if (func != null) // BEHOLD: The ultimate cast!
					return (TDelegate) (object) func;
			}
			return null;
		}
	}
}
