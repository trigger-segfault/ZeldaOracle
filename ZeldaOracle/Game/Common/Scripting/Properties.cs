using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripting {

	/// <summary>A collection of properties.</summary>
	public class Properties {
		/// <summary>The object that holds these properties.</summary>
		private IPropertyObject propertyObject;
		/// <summary>The properties from which these properties derive from (can be null).</summary>
		private Properties baseProperties;
		/// <summary>The property map.</summary>
		private Dictionary<string, Property> map;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct an empty properties list.</summary>
		public Properties() {
			this.map			= new Dictionary<string, Property>();
			this.propertyObject	= null;
			this.baseProperties	= null;
		}

		/// <summary>Construct an empty properties list with the given property object.</summary>
		public Properties(IPropertyObject propertyObject) {
			this.map			= new Dictionary<string, Property>();
			this.propertyObject	= propertyObject;
			this.baseProperties	= null;
		}

		/// <summary>Constructs a copy of the properties collection.</summary>
		public Properties(Properties copy) {
			this.map = new Dictionary<string, Property>();
			Clone(copy);
		}

		/// <summary>Clones the properties collection.</summary>
		public void Clone(Properties copy) {
			propertyObject = copy.propertyObject;
			baseProperties = copy.baseProperties;

			// Copy the property map.
			map.Clear();
			foreach (Property property in copy.map.Values) {
				Property p		= new Property(property);
				p.Properties	= this;
				map[p.Name]		= p;
			}
			ConnectBaseProperties();
		}


		//-----------------------------------------------------------------------------
		// Basic accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enumerable list of all the modified properties.</summary>
		public IEnumerable<Property> GetProperties() {
			foreach (Property property in map.Values) {
				yield return property;
			}
		}

		/// <summary>Get an enumerable list of all properties and base properties.</summary>
		public IEnumerable<Property> GetAllProperties() {
			for (Properties properties = this; properties != null; properties = properties.baseProperties) {
				foreach (Property property in properties.map.Values) {
					if (GetProperty(property.Name, true) == property)
						yield return property;
				}
			}
		}

		/// <summary>Get the property with the given name.</summary>
		public Property GetProperty(string name, bool acceptBaseProperties) {
			Property property;
			if (!map.TryGetValue(name, out property) && acceptBaseProperties && baseProperties != null)
				return baseProperties.GetProperty(name, true);
			return property;
		}

		/// <summary>Get the root property with the given name.</summary>
		public Property GetRootProperty(string name) {
			Property property = GetProperty(name, true);
			if (property != null)
				return property.GetRootProperty();
			return null;
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
		public bool Contains(string name, PropertyType type, bool acceptBaseProperties = true) {
			Property p = GetProperty(name, acceptBaseProperties);
			return (p != null && p.Type == type);
		}

		/// <summary>Does the given property have a value different from the
		/// base properties?</summary>
		public bool IsPropertyModified(string name) {
			return map.ContainsKey(name);
		}

		/// <summary>Returns true if there exists a property with the given name
		/// but no base property.</summary>
		public bool ContainsWithNoBase(string name) {
			Property p = GetProperty(name, false);
			return (p != null && p.BaseProperty == null);
		}


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsEquals(string name, string value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.String && p.StringValue == value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsEquals(string name, int value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Integer && p.IntValue == value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsEquals(string name, float value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Float && p.FloatValue == value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsEquals(string name, bool value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Boolean && p.BoolValue == value);
		}


		//-----------------------------------------------------------------------------
		// Contains NOT Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsNotEquals(string name, string value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.String && p.StringValue != value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsNotEquals(string name, int value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Integer && p.IntValue != value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsNotEquals(string name, float value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Float && p.FloatValue != value);
		}

		/// <summary>Return true if there exists a property with the given name that equates to the given value.</summary>
		public bool ContainsNotEquals(string name, bool value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Boolean && p.BoolValue != value);
		}


		//-----------------------------------------------------------------------------
		// Property value access
		//-----------------------------------------------------------------------------

		/// <summary>Get a resource value with a default value fallback.</summary>
		public T GetResource<T>(string name) where T : class {
			return Resources.GetResource<T>(GetString(name));
		}

		/// <summary>Get an enum property value.</summary>
		public E GetEnum<E>(string name) where E : struct {
			Property p = GetProperty(name, true);
			if (p.Type == PropertyType.Integer)
				return (E) Enum.ToObject(typeof(E), p.IntValue);
			else if (p.Type == PropertyType.String)
				return (E) Enum.Parse(typeof(E), p.StringValue);
			else
				throw new InvalidOperationException("Property type does not support enums.");
		}

		/// <summary>Get a string property value.</summary>
		public string GetString(string name) {
			return GetProperty(name, true).StringValue;
		}

		/// <summary>Get an integer property value.</summary>
		public int GetInteger(string name) {
			return GetProperty(name, true).IntValue;
		}

		/// <summary>Get a float property value.</summary>
		public float GetFloat(string name) {
			return GetProperty(name, true).FloatValue;
		}

		/// <summary>Get a boolean property value.</summary>
		public bool GetBoolean(string name) {
			return GetProperty(name, true).BoolValue;
		}

		/// <summary>Get a boolean property value.</summary>
		public Point2I GetPoint(string name) {
			return (Point2I) GetProperty(name, true).ObjectValue;
		}

		/// <summary>Get a generic property value.</summary>
		public T Get<T>(string name) {
			return (T) GetProperty(name, true).ObjectValue;
		}


		//-----------------------------------------------------------------------------
		// Property value access (with defaults)
		//-----------------------------------------------------------------------------

		/// <summary>Get a resource property value with a default value fallback.</summary>
		public T GetResource<T>(string name, T defaultValue) where T : class {
			Property p = GetProperty(name, true);
			T result = null;
			if (p != null && p.StringValue.Length > 0)
				result = Resources.GetResource<T>(p.StringValue);
			return result ?? defaultValue;
		}

		/// <summary>Get an enum property value with a default value fallback.</summary>
		public E GetEnum<E>(string name, E defaultValue) where E : struct {
			Property p = GetProperty(name, true);
			if (p != null) {
				if (p.Type == PropertyType.Integer)
					return (E) Enum.ToObject(typeof(E), p.IntValue);
				else if (p.Type == PropertyType.String)
					return (E) Enum.Parse(typeof(E), p.StringValue, true);
				else
					throw new InvalidOperationException("Property type does not support enums.");
			}
			return defaultValue;
		}

		/// <summary>Get a string property value with a default value fallback.</summary>
		public string GetString(string name, string defaultValue) {
			return Get<string>(name, defaultValue);
		}

		/// <summary>Get an integer property value with a default value fallback.</summary>
		public int GetInteger(string name, int defaultValue) {
			return Get<int>(name, defaultValue);
		}

		/// <summary>Get a float property value with a default value fallback.</summary>
		public float GetFloat(string name, float defaultValue) {
			return Get<float>(name, defaultValue);
		}

		/// <summary>Get a boolean property value with a default value fallback.</summary>
		public bool GetBoolean(string name, bool defaultValue) {
			return Get<bool>(name, defaultValue);
		}

		/// <summary>Get a point property value with a default value fallback.</summary>
		public Point2I GetPoint(string name, Point2I defaultValue) {
			return Get<Point2I>(name, defaultValue);
		}

		/// <summary>Get a generic property value with a default value fallback.</summary>
		public T Get<T>(string name, T defaultValue) {
			Property p = GetProperty(name, true);
			if (p != null)
				return (T) p.ObjectValue;
			return defaultValue;
		}


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			map.Clear();
		}

		/*
		public void RunActionForAll() {
			List<Property> list = map.Values.ToList();
			for (int i = 0; i < list.Count; i++) {
				list[i].RunAction(propertyObject, list[i].ObjectValue);
			}
		}
		
		/// <summary>Clear the property map.</summary>
		public void Clear() {
			map.Clear();
		}
		
		public void Remove(string name) {
			map.Remove(name);
		}
		
		/// <summary>Merge these properties with another.</summary>
		public void Merge(Properties other, bool replaceExisting) {
			foreach (Property otherProperty in other.map.Values)
				SetProperty(otherProperty.Name, otherProperty, false);
		}*/

		public bool RemoveProperty(string name, bool onlyIfNoBase) {
			Property p = GetProperty(name, false);
			if (p != null && (!onlyIfNoBase || p.BaseProperty == null)) {
				map.Remove(name);
				return true;
			}
			return false;
		}

		public bool RenameProperty(string oldName, string newName, bool onlyIfNoBase) {
			Property p = GetProperty(oldName, false);
			if (p != null && (!onlyIfNoBase || p.BaseProperty == null)) {
				p.Name = newName;
				map[newName] = p;
				map.Remove(oldName);
				if (baseProperties != null)
					p.BaseProperty = baseProperties.GetProperty(newName, true);
				else
					p.BaseProperty = null;
				return true;
			}
			return false;
		}

		/// <summary>Merge these properties with another.</summary>
		public void SetAll(Properties other) {
			foreach (Property otherProperty in other.map.Values)
				SetProperty(otherProperty.Name, otherProperty.ObjectValue, false);
		}


		//-----------------------------------------------------------------------------
		// Property Setters
		//-----------------------------------------------------------------------------

		public Property SetAsResource<T>(string name, T resource) where T : class {
			string resourceName = "";
			if (resource != null)
				resourceName = Resources.GetResourceName<T>(resource);
			return Set(name, resourceName);
		}

		public Property Set(string name, Property property) {
			return SetProperty(name, property.ObjectValue, false);
		}

		public Property Set(string name, string value) {
			return SetProperty(name, value, false);
		}
		
		public Property Set(string name, int value) {
			return SetProperty(name, value, false);
		}
		
		public Property Set(string name, float value) {
			return SetProperty(name, value, false);
		}
		
		public Property Set(string name, bool value) {
			return SetProperty(name, value, false);
		}
		
		public Property Set(string name, Point2I value) {
			return SetProperty(name, value, false);
		}
		
		public Property SetGeneric(string name, object value) {
			return SetProperty(name, value, false);
		}

		public Property SetBase(string name, Property property) {
			return SetProperty(name, property.ObjectValue, true);
		}

		public Property SetBase(string name, string value) {
			return SetProperty(name, value, true);
		}

		public Property SetBase(string name, int value) {
			return SetProperty(name, value, true);
		}

		public Property SetBase(string name, float value) {
			return SetProperty(name, value, true);
		}

		public Property SetBase(string name, bool value) {
			return SetProperty(name, value, true);
		}
		
		public Property SetBase(string name, Point2I value) {
			return SetProperty(name, value, true);
		}

		public void SetDocumentation(string name, string readableName, string editorType,
			string editorSubType, string category, string description, bool isEditable = true, bool isHidden = false)
		{
			SetDocumentation(name, new PropertyDocumentation(readableName,
				editorType, editorSubType, category, description, isEditable, isHidden));
		}

		public void SetDocumentation(string name, PropertyDocumentation documentation) {
			Property p = GetProperty(name, false);
			if (p != null)
				p.Documentation = documentation;
		}

		// Set the given property ONLY it doesn't already exist.
		// This can only create a new property.
		// Returns true if the property was created.
		//public bool Define(string name, string value);

		// Set the value of the given property ONLY if it already exists.
		// This can only modifiy existing property.
		// Returns true if the property does exist.
		//public bool Modify(string name, string value);


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Sets a propertiy's value, creating it if it doesn't already exist.
		/// This can modify an existing property or create a new property.</summary>
		private Property SetProperty(string name, object value, bool setBase) {
			if (setBase && baseProperties != null) {
				// Set the property from the base properties and remove the
				// now-redundant property from this properties' map.
				baseProperties.SetProperty(name, value, false);
				map.Remove(name);
				return null;
			}
			else {
				// Check if the value we are setting is redundant.
				bool redundantValue = false;
				Property baseProperty = null;
				if (baseProperties != null) {
					baseProperty = baseProperties.GetProperty(name, true);
					if (baseProperty != null && baseProperty.ObjectValue.Equals(value))
						redundantValue = true;
				}

				// Set the property value.
				if (map.ContainsKey(name)) {
					if (redundantValue) {
						// Remove the redundant property.
						map.Remove(name);
						return null;
					}
					else {
						// Set an existing property's value.
						Property p = map[name];
						p.ObjectValue = value;
						return p;
					}
				}
				else if (!redundantValue) {
					// Create a new property.
					Property p = Property.Create(name, value);
					p.Properties = this;
					p.BaseProperty = baseProperty;
					map[name] = p;
					return p;
				}
			}
			return null;
		}

		/// <summary>Link up our properties with their corresponding base properties.</summary>
		private void ConnectBaseProperties() {
			if (baseProperties != null) {
				// Link properties with base properties.
				Property[] properties = map.Values.ToArray();
				foreach (Property property in properties) {
					Property baseProperty = baseProperties.GetProperty(property.Name, true);
					property.BaseProperty = baseProperty;

					// Remove a redundant property (one that is equal to its base value).
					if (baseProperty != null && property.EqualsValue(baseProperty))
						map.Remove(property.Name);
				}
			}
			else {
				// Nullify all base properties.
				foreach (Property property in map.Values)
					property.BaseProperty = null;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Debug Methods
		//-----------------------------------------------------------------------------

		public void Print() {
			foreach (Property property in map.Values) {
				PrintProperty(property);
			}
		}

		public void PrintProperty(Property property) {
			Console.Write(property.Name);
			Console.Write(" = ");
			Console.Write(property.StringValue);
			Console.WriteLine();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the object that contains these properties.</summary>
		public IPropertyObject PropertyObject {
			get { return propertyObject; }
			set { propertyObject = value; }
		}

		/// <summary>Gets the number of properties.</summary>
		public int Count {
			get { return map.Count; }
		}
		/*
		/// <summary>Get the underlying map of properties.</summary>
		public Dictionary<string, Property> PropertyMap {
			get { return map; }
		}*/
		/*
		/// <summary>Get the property with the given name.</summary>
		public Property this[string propertyName] {
			get { return map[propertyName]; }
		}*/

		/// <summary>Get the base properties.</summary>
		public Properties BaseProperties {
			get { return baseProperties; }
			set { baseProperties = value; ConnectBaseProperties(); }
		}

		/// <summary>Returns true if the property collection has defined properties
		/// as well as a base collection.</summary>
		public bool HasModifiedProperties {
			get { return baseProperties != null && map.Any(); }
		}
	}
}
