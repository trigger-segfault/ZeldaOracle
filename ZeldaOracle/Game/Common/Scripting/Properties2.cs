using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripting {
	[Serializable]
	public class Properties2 : VarBaseCollection<Property2> {

		private Dictionary<string, Property2> classMap;
		private Properties2 baseProperties;
		private IPropertyObject2 propertyObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		
		/// <summary>Construct an empty properties list.</summary>
		public Properties2() {
			propertyObject		= null;
			baseProperties		= null;
		}

		/// <summary>Construct an empty properties list with the given property object.</summary>
		public Properties2(IPropertyObject2 propertyObject) {
			this.propertyObject	= propertyObject;
			baseProperties		= null;
		}

		/// <summary>Constructs a copy of the properties collection.</summary>
		public Properties2(Properties2 copy) {
			Clone(copy);
		}

		/// <summary>Constructs a copy of the properties collection.</summary>
		public Properties2(Properties2 copy, IPropertyObject2 propertyObject) {
			Clone(copy);
			this.propertyObject	= propertyObject;
		}

		/// <summary>Clones the properties collection.</summary>
		public void Clone(Properties2 copy) {
			propertyObject		= copy.propertyObject;
			baseProperties		= copy.baseProperties;

			// Copy the property map
			map.Clear();
			foreach (Property2 property in copy.map.Values) {
				Property2 p			= new Property2(property);
				p.Properties		= this;
				map[p.Name]			= p;
			}
			foreach (Property2 property in copy.classMap.Values) {
				Property2 p			= new Property2(property);
				p.Properties		= this;
				classMap[p.Name]	= p;
			}
			ConnectBaseProperties();
		}

		//-----------------------------------------------------------------------------
		// Basic Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enumerable list of all the modified properties.</summary>
		public IEnumerable<Property2> GetProperties() {
			foreach (Property2 property in map.Values) {
				yield return property;
			}
			foreach (Property2 property in classMap.Values) {
				if (property.HasBase &&
					!property.EqualsValue(property.BaseProperty))
				{
					yield return property;
				}
			}
		}

		/// <summary>Get an enumerable list of all properties and base properties.</summary>
		public IEnumerable<Property2> GetAllProperties() {
			for (Properties2 properties = this; properties != null;
				properties = properties.baseProperties)
			{
				foreach (Property2 property in properties.map.Values) {
					if (GetProperty(property.Name, true) == property)
						yield return property;
				}
				foreach (Property2 property in properties.classMap.Values) {
					if (GetPropertyClass(property.Name, true) == property)
						yield return property;
				}
			}
		}

		/// <summary>Get the property with the given name.</summary>
		public Property2 GetProperty(string name, bool acceptBaseProperties) {
			Property2 p;
			if (!map.TryGetValue(name, out p) && !classMap.TryGetValue(name, out p)) {
				if (baseProperties != null) {
					Property2 baseP = baseProperties.GetVar(name);
					if (baseP?.IsClass ?? false) {
						p = new Property2(name, baseP.CloneValue());
						p.Properties = this;
						p.BaseProperty = baseP;
						classMap[name] = p;
					}
					else if (acceptBaseProperties) {
						return baseP;
					}
				}
			}
			return p;
		}

		/// <summary>Get the class property with the given name without creating one.</summary>
		public Property2 GetPropertyClass(string name,
			bool acceptBaseProperties)
		{
			Property2 p;
			if (!classMap.TryGetValue(name, out p) && acceptBaseProperties) {
				return baseProperties?.GetPropertyClass(name, true);
			}
			return p;
		}

		/// <summary>Get the root class property with the given name without creating
		/// one.</summary>
		public Property2 GetRootPropertyClass(string name) {
			if (baseProperties != null)
				return baseProperties.GetRootPropertyClass(name);
			else
				return GetPropertyClass(name, false);
		}


		//-----------------------------------------------------------------------------
		// Property Existance
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if there exists a property with the given name.</summary>
		public bool Contains(string name, bool acceptBaseProperties = true) {
			return (GetProperty(name, acceptBaseProperties) != null);
		}

		/// <summary>Returns true if there exists a property with the given name
		/// and type.</summary>
		public bool Contains<T>(string name, bool acceptBaseProperties = true) {
			Property2 p = GetProperty(name, acceptBaseProperties);
			return (p != null && p.FullType == typeof(T));
		}

		/// <summary>Does the given property have a value different from the
		/// base properties?</summary>
		public bool IsPropertyModified(string name) {
			if (baseProperties == null)
				return false;
			if (map.ContainsKey(name))
				return true;
			Property2 p;
			if (classMap.TryGetValue(name, out p)) {
				Property2 baseP = baseProperties.GetProperty(name, true);
				return (!baseP?.EqualsValue(p) ?? false);
			}
			return false;
		}

		/// <summary>Returns true if there exists a property with the given name
		/// but no base property.</summary>
		public bool ContainsWithNoBase(string name) {
			Property2 p = GetProperty(name, false);
			return (!p?.HasBase ?? false);
		}


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a property with the given name that
		/// equates to the given value.</summary>
		public bool ContainsEquals(string name, object value) {
			Property2 p = GetProperty(name, true);
			return (p?.EqualsValue(value) ?? false);
		}

		/// <summary>Return true if there exists a property with the given name that
		/// does not equate to the given value.</summary>
		public bool ContainsNotEquals(string name, object value) {
			Property2 p = GetProperty(name, true);
			return (!p?.EqualsValue(value) ?? false);
		}


		//-----------------------------------------------------------------------------
		// Property Value Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get a resource value with a default value fallback.</summary>
		public R GetResource<R>(string name) where R : class {
			return Resources.Get<R>(Get<string>(name));
		}


		//-----------------------------------------------------------------------------
		// Property Value Accessors (with defaults)
		//-----------------------------------------------------------------------------

		/// <summary>Get a resource property value with a default value fallback.</summary>
		public R GetResource<R>(string name, R defaultValue) where R : class {
			R result = null;
			string resourceName = Get<string>(name, null);
			if (resourceName != null)
				result = Resources.Get<R>(resourceName);
			return result ?? defaultValue;
		}



		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		/// <summery>Clears all of the properties but not base properties.</summery>
		public void Clear() {
			map.Clear();
			classMap.Clear();
		}
		
		/// <summary>Sets the property and its value and checks for redundancy.</summary>
		public Property2 SetProperty(string name, object value) {
			// Check if the value we are setting is redundant.
			bool redundantValue = false;
			Property2 baseProperty = null;
			bool isClass = value.GetType().IsClass;

			if (!isClass && baseProperties != null) {
				baseProperty = baseProperties.GetVar(name);
				if (baseProperty?.EqualsValue(value) ?? false)
					redundantValue = true;
			}

			Property2 p = GetProperty(name, false);

			// Set the property value
			if (p != null) {
				if (redundantValue) {
					// Remove the redundant property
					map.Remove(name);
					return null;
				}
				else {
					// Set an existing property's value
					p.ObjectValue = value;
					return p;
				}
			}
			else if (!redundantValue) {
				// Create a new property
				p = new Property2(name, value);
				p.Properties = this;
				p.BaseProperty = baseProperty;
				if (isClass)
					classMap[name] = p;
				else
					map[name] = p;
				return p;
			}
			return null;
		}

		/// <summary>Removes the property but not from the base properties.</summary>
		public bool RemoveProperty(string name, bool onlyIfNoBase) {
			Property2 p = GetProperty(name, false);
			if (p != null && (!onlyIfNoBase || !p.HasBase)) {
				if (p.IsClass)
					classMap.Remove(name);
				else
					map.Remove(name);
				return true;
			}
			return false;
		}

		/// <summary>Removes the property but not in the base properties.</summary>
		public bool RenameProperty(string oldName, string newName, bool onlyIfNoBase) {
			Property2 p = GetProperty(oldName, false);
			if (p != null && (!onlyIfNoBase || !p.HasBase)) {
				p.Name = newName;
				if (p.IsClass) {
					classMap.Remove(oldName);
					classMap[newName] = p;
				}
				else {
					map.Remove(oldName);
					map[newName] = p;
				}
				if (baseProperties != null)
					p.BaseProperty = baseProperties.GetProperty(newName, true);
				else
					p.BaseProperty = null;
				return true;
			}
			return false;
		}

		/// <summary>Merge these properties with another.</summary>
		public void SetAll(Properties2 others) {
			foreach (Property2 other in others.map.Values) {
				SetProperty(other.Name, other.ObjectValue);
			}
			foreach (Property2 other in others.classMap.Values) {
				SetProperty(other.Name, other.ObjectValue);
			}
		}

		//-----------------------------------------------------------------------------
		// Property Value Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the property's value as an resource name.</summary>
		public Property2 SetAsResource<T>(string name, T resource) where T : class {
			string resourceName = "";
			if (resource != null)
				resourceName = Resources.GetName<T>(resource);
			return SetProperty(name, resourceName);
		}


		public Property2 SetGeneric(string name, object value) {
			return SetProperty(name, value);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Link up our properties with their corresponding base properties.</summary>
		private void ConnectBaseProperties() {
			if (baseProperties != null) {
				// Link properties with base properties.
				Property2[] properties = map.Values.ToArray();
				foreach (Property2 property in properties) {
					Property2 baseProperty = baseProperties.GetProperty(property.Name, true);
					property.BaseProperty = baseProperty;

					// Remove a redundant property (one that is equal to its base value).
					if (baseProperty != null && property.EqualsValue(baseProperty))
						map.Remove(property.Name);
				}
				properties = classMap.Values.ToArray();
				foreach (Property2 property in properties) {
					Property2 baseProperty = baseProperties.GetProperty(property.Name, true);
					property.BaseProperty = baseProperty;

					// Remove a redundant property (one that is equal to its base value).
					if (baseProperty != null && property.EqualsValue(baseProperty))
						classMap.Remove(property.Name);
				}
			}
			else {
				// Nullify all base properties.
				foreach (Property2 property in map.Values)
					property.BaseProperty = null;
				// Nullify all base properties.
				foreach (Property2 property in classMap.Values)
					property.BaseProperty = null;
			}
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override Property2 GetVar(string name) {
			return GetProperty(name, true);
		}

		protected override Property2 SetVar(string name, object value) {
			return SetProperty(name, value);
		}

		protected override Property2 SetVarAt(string name, int index, object value) {
			// Check if the value we are setting is redundant.
			bool redundantValue = false;
			Property2 baseProperty = null;
			bool isClass = value.GetType().IsClass;

			if (!isClass && baseProperties != null) {
				baseProperty = baseProperties.GetVar(name);
				if (baseProperty?.EqualsValue(value) ?? false)
					redundantValue = true;
			}

			Property2 p = GetProperty(name, false);

			// Set the property value
			if (p != null) {
				if (redundantValue) {
					// Remove the redundant property
					map.Remove(name);
					return null;
				}
				else {
					// Set an existing property's value
					p[index] = value;
					return p;
				}
			}
			else if (!redundantValue) {
				// Create a new property
				p = new Property2(name, value);
				p.Properties = this;
				p.BaseProperty = baseProperty;
				if (isClass)
					classMap[name] = p;
				else
					map[name] = p;
				return p;
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the object that contains these properties.</summary>
		public IPropertyObject2 PropertyObject {
			get { return propertyObject; }
			set { propertyObject = value; }
		}

		/// <summary>Gets the number of defined properties.</summary>
		public int Count {
			get { return map.Count + classMap.Count; }
		}

		/// <summary>Get the base properties.</summary>
		public Properties2 BaseProperties {
			get { return baseProperties; }
			set { baseProperties = value; ConnectBaseProperties(); }
		}

		/// <summary>Gets the root base properties. Can be this.</summary>
		public Properties2 RootProperties {
			get {
				if (baseProperties != null)
					return baseProperties.RootProperties;
				return this;
			}
		}

		/// <summary>Returns true if the property collection has defined properties
		/// as well as a base collection.</summary>
		public bool HasModifiedProperties {
			get {
				if (baseProperties != null) {
					if (map.Any())
						return true;
					foreach (Property2 p in classMap.Values) {
						if (p.HasBase && !p.EqualsValue(p.BaseProperty))
							return true;
					}
				}
				return false;
			}
		}
	}
}
