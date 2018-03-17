using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>The function called to initialize a tile type's properties.</summary>
	public delegate void TileDataInitializer(TileData data);

	/// <summary>The function called to initialize an action type's properties.</summary>
	public delegate void ActionDataInitializer(ActionTileData data);

	/// <summary>A static class used to initialize a tile type's properties
	/// and events using static reflection.</summary>
	public static class TileDataInitializing {
		/// <summary>The collection of tile initializer functions.</summary>
		private static Dictionary<Type, TileDataInitializer> tileInitializers;
		/// <summary>The collection of action initializer functions.</summary>
		private static Dictionary<Type, ActionDataInitializer> actionInitializers;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the type initializer dictionaries.</summary>
		static TileDataInitializing() {
			tileInitializers = new Dictionary<Type, TileDataInitializer>();
			actionInitializers = new Dictionary<Type, ActionDataInitializer>();
		}


		//-----------------------------------------------------------------------------
		// Initializing
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the data for a tile's types.</summary>
		public static void InitializeTile(BaseTileData data, Type previousType) {
			// No subtype to initialize
			if (data.Type == null)
				return;
			
			// Get the correct inheritance types
			Type[] types = null;
			if (previousType != null)
				types = TypeHelper.GetInheritance(previousType, data.Type, true);
			else if (data is TileData)
				types = TypeHelper.GetInheritance<Tile>(data.Type, true);
			else if (data is ActionTileData)
				types = TypeHelper.GetInheritance<ActionTile>(data.Type, true);
			else
				throw new ArgumentException("Invalidate BaseTileData!");

			// Initialize each base type's properties
			// up to and including this tile's type.
			foreach (Type type in types) {
				if (type.Equals(typeof(Tile)) || type.Equals(typeof(ActionTile)) ||
					type.Equals(previousType))
					continue;
				InitializeTileType(type, data);
			}
		}

		/// <summary>Initializes the data for a tile's entity types.</summary>
		public static void InitializeEntity(BaseTileData data, Type previousType) {
			// No subtype to initialize
			if (data.EntityType == null)
				return;

			// Get the correct inheritance types
			Type[] types = null;
			if (previousType != null)
				types = TypeHelper.GetInheritance(previousType, data.EntityType, true);
			else
				types = TypeHelper.GetInheritance<Entity>(data.EntityType, true);

			// Initialize each base type's properties
			// up to and including this tile's type.
			foreach (Type type in types) {
				if (type.Equals(typeof(Entity)) || type.Equals(previousType))
					continue;
				InitializeTileType(type, data);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the data for a tile's single type.
		/// This method functions the same for both tiles types and entity types.</summary>
		private static void InitializeTileType(Type type, BaseTileData data) {
			if (data is TileData) {
				TileData tile = (TileData) data;
				TileDataInitializer func;
				if (!tileInitializers.TryGetValue(type, out func)) {
					MethodInfo methodInfo = type.GetMethod("InitializeTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(TileData));
					if (methodInfo != null) {
						func = ReflectionHelper.GetFunction
							<TileDataInitializer>(methodInfo);
					}
					tileInitializers.Add(type, func);
				}
				if (func != null)
					func(tile);
			}
			else if (data is ActionTileData) {
				ActionTileData action = (ActionTileData) data;
				ActionDataInitializer func;
				if (!actionInitializers.TryGetValue(type, out func)) {
					MethodInfo methodInfo = type.GetMethod("InitializeTileData",
						BindingFlags.Static | BindingFlags.Public,
						typeof(ActionTileData));
					if (methodInfo != null) {
						func = ReflectionHelper.GetFunction
							<ActionDataInitializer>(methodInfo);
					}
					actionInitializers.Add(type, func);
				}
				if (func != null)
					func(action);
			}
		}
	}
}
