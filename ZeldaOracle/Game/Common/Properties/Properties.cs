using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Properties {

	public class Properties {

		// The property map.
		private Dictionary<string, Property> map;
		private Properties baseProperties;
		private IPropertyObject propertyObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Construct an empty properties list.
		public Properties() {
			this.map			= new Dictionary<string, Property>();
			this.baseProperties	= null;
		}


		//-----------------------------------------------------------------------------
		// Basic accessors
		//-----------------------------------------------------------------------------
		
		// Get the property with the given name.
		public Property GetProperty(string name, bool acceptBaseProperties = false) {
			if (acceptBaseProperties && baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetProperty(name, true);
			return map[name];
		}
		

		//-----------------------------------------------------------------------------
		// Property existance
		//-----------------------------------------------------------------------------

		// Returns true if there exists a property with the given name.
		public bool Exists(string name) {
			if (baseProperties != null && baseProperties.Exists(name))
				return true;
			return map.ContainsKey(name);
		}

		// Returns true if there exists a property with the given name and type.
		public bool Exists(string name, PropertyType type) {
			if (baseProperties != null && baseProperties.Exists(name, type))
				return true;
			if (!map.ContainsKey(name))
				return false;
			return (map[name].Type == type);
		}

		// Does the given property have a value different from the base properties?
		public bool IsPropertyModified(string name) {
			if (baseProperties == null || !map.ContainsKey(name) || !baseProperties.map.ContainsKey(name))
				return false;
			return !baseProperties.map[name].EqualsValue(map[name]);
		}

		
		//-----------------------------------------------------------------------------
		// Exist equals
		//-----------------------------------------------------------------------------
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, string value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.String && p.StringValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, int value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Integer && p.IntValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, float value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Float && p.FloatValue == value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsEquals(string name, bool value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Boolean && p.BoolValue == value);
		}
		
		
		//-----------------------------------------------------------------------------
		// Exist NOT equals
		//-----------------------------------------------------------------------------
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, string value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsNotEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.String && p.StringValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, int value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsNotEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Integer && p.IntValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, float value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsNotEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Float && p.FloatValue != value);
		}
		
		// Return true if there exists a property with the given name that equates to the given value.
		public bool ExistsNotEquals(string name, bool value) {
			if (!map.ContainsKey(name)) {
				if (baseProperties != null)
					return baseProperties.ExistsNotEquals(name, value);
				return false;
			}
			Property p = GetProperty(name);
			return (p.Type == PropertyType.Boolean && p.BoolValue != value);
		}


		//-----------------------------------------------------------------------------
		// Property value access
		//-----------------------------------------------------------------------------
		
		// Get a boolean value with a default value fallback.
		public Property GetList(string name) {
			return GetProperty(name);
		}
		
		// Get a boolean value with a default value fallback.
		public string GetString(string name) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetString(name);
			return GetProperty(name).StringValue;
		}
		
		// Get a boolean value with a default value fallback.
		public int GetInteger(string name) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetInteger(name);
			return GetProperty(name).IntValue;
		}
		
		// Get a boolean value with a default value fallback.
		public float GetFloat(string name) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetFloat(name);
			return GetProperty(name).FloatValue;
		}
		
		// Get a boolean value with a default value fallback.
		public bool GetBoolean(string name) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetBoolean(name);
			return GetProperty(name).BoolValue;
		}


		//-----------------------------------------------------------------------------
		// Property value acces (with defaults)
		//-----------------------------------------------------------------------------

		// Get a boolean value with a default value fallback.
		public string GetString(string name, string defaultValue) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetString(name, defaultValue);
			if (Exists(name, PropertyType.String))
				return GetProperty(name).StringValue;
			return defaultValue;
		}
		
		// Get a boolean value with a default value fallback.
		public int GetInteger(string name, int defaultValue) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetInteger(name, defaultValue);
			if (Exists(name, PropertyType.Integer))
				return GetProperty(name).IntValue;
			return defaultValue;
		}
		
		// Get a boolean value with a default value fallback.
		public float GetFloat(string name, float defaultValue) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetFloat(name, defaultValue);
			if (Exists(name, PropertyType.Float))
				return GetProperty(name).FloatValue;
			return defaultValue;
		}
		
		// Get a boolean value with a default value fallback.
		public bool GetBoolean(string name, bool defaultValue) {
			if (baseProperties != null && !map.ContainsKey(name))
				return baseProperties.GetBoolean(name, defaultValue);
			if (Exists(name, PropertyType.Boolean))
				return GetProperty(name).BoolValue;
			return defaultValue;
		}


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------
		
		// Clear the property map.
		public void Clear() {
			map.Clear();
		}

		public void Remove(string name) {
			map.Remove(name);
		}

		// Add a new property to the map.
		public void Add(Property property) {
			map[property.Name] = property;
			map[property.Name].Properties = this;
			// TODO: Check if property is not equal to default.
		}

		// Merge these properties with another.
		public void Merge(Properties other, bool replaceExisting) {
			string[] keys = other.map.Keys.ToArray();

			for (int i = 0; i < keys.Length; ++i) {
				string key = keys[i];
				if (replaceExisting || !map.ContainsKey(key)) {
					map[key] = new Property(other.map[key]);
					map[key].Properties = this;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Property set
		//-----------------------------------------------------------------------------
		
		// Sets a propertiy's value, creating it if it doesn't already exist.
		// This can modify an existing property or create a new property.

		private Property SetProperty(string name, Property property, bool setBase) {
			if (setBase) {
				if (map.ContainsKey(name)) {
					map[name].SetValues(property);
				}
				else {
					map[name] = property;
					map[name].Properties = this;
				}
				baseProperties.SetProperty(name, property, false);
			}
			else {
				if (map.ContainsKey(name)) {
					map[name].SetValues(property);
				}
				else if (baseProperties != null && baseProperties.map.ContainsKey(name)) {
					map[name] = new Property(baseProperties.map[name]);
					map[name].Properties = this;
					map[name].SetValues(property);
				}
				else {
					map[name] = property;
					map[name].Properties = this;
				}
			}
			return property;
		}

		public Property Set(string name, Property property) {
			return SetProperty(name, property, false);
		}

		public Property Set(string name, string value) {
			return SetProperty(name, Property.CreateString(name, value), false);
		}
		
		public Property Set(string name, int value) {
			return SetProperty(name, Property.CreateInt(name, value), false);
		}
		
		public Property Set(string name, float value) {
			return SetProperty(name, Property.CreateFloat(name, value), false);
		}
		
		public Property Set(string name, bool value) {
			return SetProperty(name, Property.CreateBool(name, value), false);
		}


		public Property SetBase(string name, Property property) {
			return SetProperty(name, property, true);
		}

		public Property SetBase(string name, string value) {
			return SetProperty(name, Property.CreateString(name, value), true);
		}

		public Property SetBase(string name, int value) {
			return SetProperty(name, Property.CreateInt(name, value), true);
		}

		public Property SetBase(string name, float value) {
			return SetProperty(name, Property.CreateFloat(name, value), true);
		}

		public Property SetBase(string name, bool value) {
			return SetProperty(name, Property.CreateBool(name, value), true);
		}
		

		//-----------------------------------------------------------------------------
		// Property define
		//-----------------------------------------------------------------------------
		/*
		// Set the given property ONLY it doesn't already exist.
		// This can only create a new property.
		// Returns true if the property was created.

		public bool Define(string name, string value) {
			if (!Exists(name)) {
				map.Add(name, Property.CreateString(name, value));
				return true;
			}
			return false;
		}

		public bool Define(string name, int value) {
			if (!Exists(name)) {
				map.Add(name, Property.CreateInt(name, value));
				return true;
			}
			return false;
		}

		public bool Define(string name, float value) {
			if (!Exists(name)) {
				map.Add(name, Property.CreateFloat(name, value));
				return true;
			}
			return false;
		}

		public bool Define(string name, bool value) {
			if (!Exists(name)) {
				map.Add(name, Property.CreateBool(name, value));
				return true;
			}
			return false;
		}
		

		//-----------------------------------------------------------------------------
		// Property modify
		//-----------------------------------------------------------------------------

		// Set the value of the given property ONLY if it already exists.
		// This can only modifiy existing property.
		// Returns true if the property does exist.

		public bool Modify(string name, string value) {
			if (Exists(name)) {
				map[name] = Property.CreateString(name, value);
				return true;
			}
			return false;
		}

		public bool Modify(string name, int value) {
			if (Exists(name)) {
				map[name] = Property.CreateInt(name, value);
				return true;
			}
			return false;
		}

		public bool Modify(string name, float value) {
			if (Exists(name)) {
				map[name] = Property.CreateFloat(name, value);
				return true;
			}
			return false;
		}

		public bool Modify(string name, bool value) {
			if (Exists(name)) {
				map[name] = Property.CreateBool(name, value);
				return true;
			}
			return false;
		}
		*/

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		// Create a Properties instance from the given list of properties.
		public static Properties Create(params Property[] propertyList) {
			Properties p = new Properties();
			for (int i = 0; i < propertyList.Length; i++)
				p.Add(propertyList[i]);
			return p;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Get the number of properties.
		public int Count {
			get { return map.Count; }
		}

		// Get the map of properties.
		public Dictionary<string, Property> PropertyMap {
			get { return map; }
		}
		
		// Get the property with the given name.
		public Property this[string propertyName] {
			get { return map[propertyName]; }
		}
		
		// Get the base properties.
		public Properties BaseProperties {
			get { return baseProperties; }
			set { baseProperties = value; }
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
			set { propertyObject = value; }
		}

	}
}
