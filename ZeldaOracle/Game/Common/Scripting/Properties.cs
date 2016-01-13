using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripting {

	public class Properties {

		// The object that holds these properties.
		private IPropertyObject propertyObject;
		// The properties from which these properties derive from (can be null).
		private Properties baseProperties;
		// The property map.
		private Dictionary<string, Property> map;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		// Construct an empty properties list.
		public Properties() : this(null) {
		}
		
		// Construct an empty properties list with the given property object.
		public Properties(IPropertyObject propertyObject) {
			this.map			= new Dictionary<string, Property>();
			this.baseProperties	= null;
			this.propertyObject	= propertyObject;
		}


		//-----------------------------------------------------------------------------
		// Basic accessors
		//-----------------------------------------------------------------------------
		
		// Get an enumerable list of all the modified properties.
		public IEnumerable<Property> GetProperties() {
			foreach (Property property in map.Values) {
				yield return property;
			}
		}
		
		// Get an enumerable list of all properties and base properties.
		public IEnumerable<Property> GetAllProperties() {
			for (Properties properties = this; properties != null; properties = properties.baseProperties) {
				foreach (Property property in properties.map.Values) {
					if (GetProperty(property.Name, true) == property)
						yield return property;
				}
			}
		}
		
		// Get the property with the given name.
		public Property GetProperty(string name, bool acceptBaseProperties) {
			KeyValuePair<string, Property> property = map.FirstOrDefault(p => p.Key == name);
			if (property.Value == null && acceptBaseProperties && baseProperties != null)
				return baseProperties.GetProperty(name, true);
			return property.Value;
		}
		
		// Get the root property with the given name.
		public Property GetRootProperty(string name) {
			Property property = GetProperty(name, true);
			if (property != null)
				return property.GetRootProperty();
			return null;
		}
		

		//-----------------------------------------------------------------------------
		// Property Existance
		//-----------------------------------------------------------------------------

		// Returns true if there exists a property with the given name.
		public bool Exists(string name) {
			return (GetProperty(name, true) != null);
		}

		// Returns true if there exists a property with the given name and type.
		public bool Exists(string name, PropertyType type) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == type);
		}

		// Does the given property have a value different from the base properties?
		public bool IsPropertyModified(string name) {
			return map.ContainsKey(name);
		}

		
		//-----------------------------------------------------------------------------
		// Exist Equals
		//-----------------------------------------------------------------------------
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, string value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.String && p.StringValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, int value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Integer && p.IntValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, float value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Float && p.FloatValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, bool value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Boolean && p.BoolValue == value);
		}
		
		
		//-----------------------------------------------------------------------------
		// Exist NOT Equals
		//-----------------------------------------------------------------------------
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, string value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.String && p.StringValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, int value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Integer && p.IntValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, float value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Float && p.FloatValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, bool value) {
			Property p = GetProperty(name, true);
			return (p != null && p.Type == PropertyType.Boolean && p.BoolValue != value);
		}


		//-----------------------------------------------------------------------------
		// Property value access
		//-----------------------------------------------------------------------------

		// Get a resource value with a default value fallback.
		public T GetResource<T>(string name) where T : class {
			return Resources.GetResource<T>(GetString(name));
		}
		
		// Get an enum property value.
		public E GetEnum<E>(string name) where E : struct {
			return (E) Enum.ToObject(typeof(E), GetProperty(name, true).IntValue);
		}
		
		// Get a string property value.
		public string GetString(string name) {
			return GetProperty(name, true).StringValue;
		}
		
		// Get an integer property value.
		public int GetInteger(string name) {
			return GetProperty(name, true).IntValue;
		}
		
		// Get a float property value.
		public float GetFloat(string name) {
			return GetProperty(name, true).FloatValue;
		}
		
		// Get a boolean property value.
		public bool GetBoolean(string name) {
			return GetProperty(name, true).BoolValue;
		}
		
		// Get a boolean property value.
		public Point2I GetPoint(string name) {
			return (Point2I) GetProperty(name, true).ObjectValue;
		}
		
		// Get a generic property value.
		public T Get<T>(string name) {
			return (T) GetProperty(name, true).ObjectValue;
		}


		//-----------------------------------------------------------------------------
		// Property value access (with defaults)
		//-----------------------------------------------------------------------------
		
		// Get a resource property value with a default value fallback.
		public T GetResource<T>(string name, T defaultValue) where T : class {
			Property p = GetProperty(name, true);
			if (p != null && p.StringValue.Length > 0)
				return Resources.GetResource<T>(p.StringValue);
			return defaultValue;
		}

		// Get a string property value with a default value fallback.
		public string GetString(string name, string defaultValue) {
			return Get<string>(name, defaultValue);
		}
		
		// Get an integer property value with a default value fallback.
		public int GetInteger(string name, int defaultValue) {
			return Get<int>(name, defaultValue);
		}
		
		// Get an enum property value with a default value fallback.
		public E GetEnum<E>(string name, E defaultValue) where E : struct {
			Property p = GetProperty(name, true);
			if (p != null)
				return (E) Enum.ToObject(typeof(E), p.IntValue);
			return defaultValue;
		}
		
		// Get a float property value with a default value fallback.
		public float GetFloat(string name, float defaultValue) {
			return Get<float>(name, defaultValue);
		}
		
		// Get a boolean property value with a default value fallback.
		public bool GetBoolean(string name, bool defaultValue) {
			return Get<bool>(name, defaultValue);
		}
		
		// Get a point property value with a default value fallback.
		public Point2I GetPoint(string name, Point2I defaultValue) {
			return Get<Point2I>(name, defaultValue);
		}
		
		// Get a generic property value with a default value fallback.
		public T Get<T>(string name, T defaultValue) {
			Property p = GetProperty(name, true);
			if (p != null)
				return (T) p.ObjectValue;
			return defaultValue;
		}


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------
		/*
		public void RunActionForAll() {
			List<Property> list = map.Values.ToList();
			for (int i = 0; i < list.Count; i++) {
				list[i].RunAction(propertyObject, list[i].ObjectValue);
			}
		}
		
		// Clear the property map.
		public void Clear() {
			map.Clear();
		}
		
		public void Remove(string name) {
			map.Remove(name);
		}
		
		// Merge these properties with another.
		public void Merge(Properties other, bool replaceExisting) {
			foreach (Property otherProperty in other.map.Values)
				SetProperty(otherProperty.Name, otherProperty, false);
		}*/

		// Merge these properties with another.
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
		
		// Sets a propertiy's value, creating it if it doesn't already exist.
		// This can modify an existing property or create a new property.
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

		// Link up our properties with their corresponding base properties.
		private void ConnectBaseProperties() {
			Property[] properties = map.Values.ToArray();

			// Link properties with base properties.
			foreach (Property property in properties) {
				Property baseProperty = baseProperties.GetProperty(property.Name, true);
				property.BaseProperty = baseProperty;

				// Remove a redundant property (one that is equal to its base value).
				if (baseProperty != null && property.EqualsValue(baseProperty))
					map.Remove(property.Name);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The object that has these properties.
		public IPropertyObject PropertyObject {
			get { return propertyObject; }
			set { propertyObject = value; }
		}

		// Get the number of properties.
		public int Count {
			get { return map.Count; }
		}
		/*
		// Get the underlying map of properties.
		public Dictionary<string, Property> PropertyMap {
			get { return map; }
		}*/
		/*
		// Get the property with the given name.
		public Property this[string propertyName] {
			get { return map[propertyName]; }
		}*/
		
		// Get the base properties.
		public Properties BaseProperties {
			get { return baseProperties; }
			set { baseProperties = value; ConnectBaseProperties(); }
		}
	}
}
