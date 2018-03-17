using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZeldaOracle.Common.Content {
	/// <summary>A class for storing temporary resources that will not be needed
	/// after this class is gone.</summary>
	public class TemporaryResources {

		/// <summary>A map of the resource dictionaries by resource type.</summary>
		private Dictionary<Type, IDictionary> resourceDictionaries;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the temporary resources.</summary>
		public TemporaryResources() {
			resourceDictionaries = new Dictionary<Type, IDictionary>();
		}


		//-----------------------------------------------------------------------------
		// Generic Resource Methods
		//-----------------------------------------------------------------------------

		/// <summary>Does the a resource with the given name and type exist?</summary>
		public bool ContainsResource<T>(T resource) {
			if (!ContainsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			return dictionary.ContainsValue(resource);
		}

		/// <summary>Does the a resource with the given name and type exist?</summary>
		public bool ContainsResource<T>(string name) {
			if (!ContainsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			return dictionary.ContainsKey(name);
		}

		/// <summary>Get the resource with the given name and type.</summary>
		public T GetResource<T>(string name) {
			if (!ContainsResourceType<T>())
				return default(T); // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			T resource;
			dictionary.TryGetValue(name, out resource);
			return resource;
		}

		/// <summary>Get the resource with the given name and type.</summary>
		public string GetResourceName<T>(T resource) where T : class {
			if (!ContainsResourceType<T>())
				return ""; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			return dictionary.FirstOrDefault(x => x.Value == resource).Key;
		}

		/// <summary>Get the dictionary used to store the given type of resources.</summary>
		public Dictionary<string, T> GetResourceDictionary<T>() {
			// Check if this type of resource doesn't exist!
			if (!ContainsResourceType<T>())
				resourceDictionaries[typeof(T)] = new Dictionary<string, T>();
			return (Dictionary<string, T>) resourceDictionaries[typeof(T)];
		}

		/// <summary>Is the given type of resource handled by this class?</summary>
		public bool ContainsResourceType<T>() {
			return resourceDictionaries.ContainsKey(typeof(T));
		}

		/// <summary>Add the given resource under the given name.</summary>
		public void AddResource<T>(string assetName, T resource) {
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			dictionary.Add(assetName, resource);
		}

		/// <summary>Add the given resource under the given name.</summary>
		public void SetResource<T>(string assetName, T resource) {
			Dictionary<string, T> dictionary = GetResourceDictionary<T>();
			dictionary[assetName] = resource;
		}
	}
}
