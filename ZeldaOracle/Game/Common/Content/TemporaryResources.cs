using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Common.Content {
	public class TemporaryResources {

		// A map of the resource dictionaries by resource type.
		private Dictionary<Type, object> resourceDictionaries;

		// GRAPHICS:
		// The collection of loaded images.
		private Dictionary<string, Image> images;
		// The collection of loaded sprite sheets.
		private Dictionary<string, SpriteSheet> spriteSheets;
		// The collection of loaded sprites.
		private Dictionary<string, Sprite> sprites;
		// The collection of loaded animations.
		private Dictionary<string, Animation> animations;

		// OTHER:
		// The collection of loaded collision models.
		private Dictionary<string, CollisionModel> collisionModels;
		// The collection of loaded tile data strucures.
		private Dictionary<string, TileData> tileData;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TemporaryResources() {
			this.images				= new Dictionary<string, Image>();
			this.spriteSheets		= new Dictionary<string, SpriteSheet>();
			this.sprites			= new Dictionary<string, Sprite>();
			this.animations			= new Dictionary<string, Animation>();

			this.collisionModels	= new Dictionary<string, CollisionModel>();
			this.tileData			= new Dictionary<string, TileData>();

			// Setup the resource dictionary lookup map.
			this.resourceDictionaries = new Dictionary<Type, object>();
			this.resourceDictionaries[typeof(Image)]			= images;
			this.resourceDictionaries[typeof(SpriteSheet)]		= spriteSheets;
			this.resourceDictionaries[typeof(Sprite)]			= sprites;
			this.resourceDictionaries[typeof(Animation)]		= animations;
			this.resourceDictionaries[typeof(CollisionModel)]	= collisionModels;
			this.resourceDictionaries[typeof(TileData)]			= tileData;
		}


		//-----------------------------------------------------------------------------
		// Generic Resource Methods
		//-----------------------------------------------------------------------------

		// Does the a resource with the given name and type exist?
		public bool ExistsResource<T>(T resource) {
			if (!ExistsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>)resourceDictionaries[typeof(T)];
			return dictionary.ContainsValue(resource) || Resources.ExistsResource<T>(resource);
		}

		// Does the a resource with the given name and type exist?
		public bool ExistsResource<T>(string name) {
			if (!ExistsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>)resourceDictionaries[typeof(T)];
			return dictionary.ContainsKey(name) || Resources.ExistsResource<T>(name);
		}

		// Get the resource with the given name and type.
		public T GetResource<T>(string name) {
			if (!ExistsResourceType<T>())
				return default(T); // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>)resourceDictionaries[typeof(T)];
			if (!dictionary.ContainsKey(name))
				return Resources.GetResource<T>(name);
			return dictionary[name];
		}

		// Get the resource with the given name and type.
		public string GetResourceName<T>(T resource) where T : class {
			if (!ExistsResourceType<T>())
				return ""; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>)resourceDictionaries[typeof(T)];
			if (!dictionary.ContainsValue(resource))
				return Resources.GetResourceName<T>(resource);
			return dictionary.FirstOrDefault(x => x.Value == resource).Key;
		}

		// Get the dictionary used to store the given type of resources.
		public Dictionary<string, T> GetResourceDictionary<T>() {
			if (!ExistsResourceType<T>())
				return null; // This type of resource doesn't exist!
			return (Dictionary<string, T>)resourceDictionaries[typeof(T)];
		}

		// Is the given type of resource handled by this class?
		public bool ExistsResourceType<T>() {
			return resourceDictionaries.ContainsKey(typeof(T));
		}

		// Add the given resource under the given name.
		public void AddResource<T>(string assetName, T resource) {
			if (!ExistsResourceType<T>())
				return; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>)resourceDictionaries[typeof(T)];
			dictionary.Add(assetName, resource);
		}


		//-----------------------------------------------------------------------------
		// Resource Accessors
		//-----------------------------------------------------------------------------

		// Gets a sprite or animation depending on which one exists.
		public SpriteAnimation GetSpriteAnimation(string name) {
			if (sprites.ContainsKey(name))
				return sprites[name];
			else if (animations.ContainsKey(name))
				return animations[name];
			return Resources.GetSpriteAnimation(name);
		}
	}
}
