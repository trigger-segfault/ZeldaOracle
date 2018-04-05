using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Scripting.Internal;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A collection of properties.</summary>
	[Serializable]
	public class Properties : VarBaseCollection<Property>, IEnumerable {

		/// <summary>The map of value properties in the collection. Properties in the
		/// value map are guaranteed to be modified from the base properties.</summary>
		private Dictionary<string, Property> valueMap;
		/// <summary>The map of class properties in the collection. Properties in the
		/// class map are not guaranteed to be modified from the base properties.</summary>
		private Dictionary<string, Property> classMap;
		/// <summary>The properties from which these properties derive from (can be null).</summary>
		[NonSerialized]
		private Properties baseProperties;
		/// <summary>The object that holds these properties.</summary>
		[NonSerialized]
		private IPropertyObject propertyObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Construct an empty properties list.</summary>
		public Properties() {
			valueMap = new Dictionary<string, Property>();
			classMap = new Dictionary<string, Property>();
			propertyObject		= null;
			baseProperties		= null;
		}

		/// <summary>Construct an empty properties list with the given property object.</summary>
		public Properties(IPropertyObject propertyObject) {
			valueMap = new Dictionary<string, Property>();
			classMap = new Dictionary<string, Property>();
			this.propertyObject	= propertyObject;
			baseProperties		= null;
		}

		/// <summary>Constructs a copy of the properties collection.</summary>
		public Properties(Properties copy) {
			valueMap = new Dictionary<string, Property>();
			classMap = new Dictionary<string, Property>();
			Clone(copy);
		}

		/// <summary>Constructs a copy of the properties collection.</summary>
		public Properties(Properties copy, IPropertyObject propertyObject) {
			valueMap = new Dictionary<string, Property>();
			classMap = new Dictionary<string, Property>();
			Clone(copy);
			this.propertyObject	= propertyObject;
		}

		/// <summary>Clones the properties collection.</summary>
		public void Clone(Properties copy) {
			propertyObject		= copy.propertyObject;
			baseProperties		= copy.baseProperties;

			// Copy the property map
			valueMap.Clear();
			classMap.Clear();
			foreach (Property property in copy.valueMap.Values) {
				Property p			= new Property(property);
				p.Properties		= this;
				valueMap[p.Name]			= p;
			}
			foreach (Property property in copy.classMap.Values) {
				Property p			= new Property(property);
				p.Properties		= this;
				classMap[p.Name]	= p;
			}
			ConnectBaseProperties();
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			foreach (Property property in valueMap.Values) {
				yield return property;
			}
			foreach (Property property in classMap.Values) {
				yield return property;
			}
		}


		//-----------------------------------------------------------------------------
		// Basic Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enumerable list of all the modified properties.</summary>
		public IEnumerable<Property> GetProperties() {
			foreach (Property property in valueMap.Values) {
				yield return property;
			}
			foreach (Property property in classMap.Values) {
				if (property.IsModified)
					yield return property;
			}
		}

		/// <summary>Get an enumerable list of all properties and base properties.</summary>
		public IEnumerable<Property> GetAllProperties() {
			for (Properties properties = this; properties != null;
				properties = properties.baseProperties)
			{
				foreach (Property property in properties.valueMap.Values) {
					if (GetPropertyValue(property.Name, true) == property)
						yield return property;
				}
				foreach (Property property in properties.classMap.Values) {
					if (GetPropertyClass(property.Name, true) == property)
						yield return property;
				}
			}
		}

		/// <summary>Get the property with the given name.</summary>
		public Property GetProperty(string name, bool acceptBaseProperties) {
			Property p;
			if (!valueMap.TryGetValue(name, out p) && !classMap.TryGetValue(name, out p)) {
				if (acceptBaseProperties && baseProperties != null) {
					Property baseProperty = baseProperties.GetVar(name);
					if (baseProperty?.IsClass ?? false) {
						p = new Property(name, baseProperty.CloneValue());
						p.Properties = this;
						p.BaseProperty = baseProperty;
						classMap[name] = p;
					}
					else if (acceptBaseProperties) {
						return baseProperty;
					}
				}
			}
			return p;
		}

		/// <summary>Get the value property with the given name.</summary>
		public Property GetPropertyValue(string name, bool acceptBaseProperties) {
			Property p;
			if (!valueMap.TryGetValue(name, out p) && acceptBaseProperties) {
				return baseProperties?.GetPropertyValue(name, true);
			}
			return p;
		}

		/// <summary>Get the class property with the given name without creating one.</summary>
		public Property GetPropertyClass(string name, bool acceptBaseProperties) {
			Property p;
			if (!classMap.TryGetValue(name, out p) && acceptBaseProperties) {
				return baseProperties?.GetPropertyClass(name, true);
			}
			return p;
		}

		/// <summary>Get the root property with the given name.</summary>
		public Property GetRootProperty(string name) {
			if (baseProperties != null)
				return baseProperties.GetRootProperty(name);
			else
				return GetProperty(name, false);
		}

		/// <summary>Get the root value property with the given name.</summary>
		public Property GetRootPropertyValue(string name) {
			if (baseProperties != null)
				return baseProperties.GetRootPropertyValue(name);
			else
				return GetPropertyValue(name, false);
		}

		/// <summary>Get the root class property with the given name without creating
		/// one.</summary>
		public Property GetRootPropertyClass(string name) {
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
			Property p = GetProperty(name, acceptBaseProperties);
			return (p != null && p.FullType == typeof(T));
		}

		/// <summary>Does the given property have a value different from the
		/// base properties?</summary>
		public bool IsPropertyModified(string name) {
			if (valueMap.ContainsKey(name))
				return true;
			Property p;
			if (classMap.TryGetValue(name, out p))
				return p.IsModified;
			return false;
		}

		/// <summary>Returns true if there exists a property with the given name
		/// but no base property.</summary>
		public bool ContainsWithNoBase(string name) {
			Property p = GetProperty(name, false);
			return (!p?.HasBase ?? false);
		}


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a property with the given name that
		/// equates to the given value.</summary>
		public bool ContainsEquals(string name, object value) {
			Property p = GetProperty(name, true);
			return (p?.EqualsValue(value) ?? false);
		}

		/// <summary>Return true if there exists a property with the given name that
		/// does not equate to the given value.</summary>
		public bool ContainsNotEquals(string name, object value) {
			Property p = GetProperty(name, true);
			return (!p?.EqualsValue(value) ?? false);
		}
		

		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		/// <summery>Clears all of the properties but not base properties.</summery>
		public void Clear() {
			valueMap.Clear();
			classMap.Clear();
		}
		
		/// <summary>Sets the property and its value and checks for redundancy.</summary>
		public Property SetProperty(string name, object value) {
			// Check if the value we are setting is redundant.
			bool redundantValue = false;
			Property baseProperty = null;
			bool isClass = value.GetType().IsClass && !(value is string);

			if (!isClass && baseProperties != null) {
				baseProperty = baseProperties.GetProperty(name, true);
				if (baseProperty?.EqualsValue(value) ?? false)
					redundantValue = true;
			}

			Property p = GetProperty(name, false);

			// Set the property value
			if (p != null) {
				if (redundantValue) {
					// Remove the redundant property
					valueMap.Remove(name);
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
				p = new Property(name, value);
				p.Properties = this;
				p.BaseProperty = baseProperty;
				if (isClass)
					classMap[name] = p;
				else
					valueMap[name] = p;
				return p;
			}
			return null;
		}

		/// <summary>Removes the property but not from the base properties.</summary>
		public bool RemoveProperty(string name, bool onlyIfNoBase) {
			Property p = GetProperty(name, false);
			if (p != null && (!onlyIfNoBase || !p.HasBase)) {
				if (p.IsClass)
					classMap.Remove(name);
				else
					valueMap.Remove(name);
				return true;
			}
			return false;
		}

		/// <summary>Removes the property but not in the base properties.</summary>
		public bool RenameProperty(string oldName, string newName, bool onlyIfNoBase) {
			Property p = GetProperty(oldName, false);
			if (p != null && (!onlyIfNoBase || !p.HasBase)) {
				p.Name = newName;
				if (p.IsClass) {
					classMap.Remove(oldName);
					classMap[newName] = p;
				}
				else {
					valueMap.Remove(oldName);
					valueMap[newName] = p;
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
		public void SetAll(Properties others) {
			foreach (Property other in others.valueMap.Values) {
				SetProperty(other.Name, other.ObjectValue);
			}
			foreach (Property other in others.classMap.Values) {
				SetProperty(other.Name, other.ObjectValue);
			}
		}
		
		/// <summary>Sets the documentation for an existing property.
		/// Does not include base properties.</summary>
		public void SetDocumentation(string name, string readableName,
			string editorType, string editorSubType, string category,
			string description, bool isReadOnly = false, bool isBrowsable = true)
		{
			SetDocumentation(name, new PropertyDocumentation(readableName,
				editorType, editorSubType, category,
				description, isReadOnly, isBrowsable));
		}

		/// <summary>Sets the documentation for an existing property.
		/// Does not include base properties.</summary>
		public void SetDocumentation(string name, string readableName,
			string editorType, Type editorSubType, string category, string description,
			bool isReadOnly = false, bool isBrowsable = true)
		{
			SetDocumentation(name, new PropertyDocumentation(readableName,
				editorType, editorSubType, category,
				description, isReadOnly, isBrowsable));
		}

		/// <summary>Sets the documentation for an existing property.
		/// Does not include base properties.</summary>
		public void SetDocumentation(string name,
			PropertyDocumentation documentation)
		{
			Property p = GetProperty(name, false);
			if (p != null)
				p.Documentation = documentation;
		}

		/// <summary>Marks a property as unbrowsable and read only in the
		/// documentation.</summary>
		public void Hide(string name) {
			Property p = GetProperty(name, false);
			if (p != null)
				p.Hide();
		}

		/// <summary>Marks a property as browsable and writable in the documentation.</summary>
		public void Unhide(string name) {
			Property p = GetProperty(name, false);
			if (p != null)
				p.Unhide();
		}

		/// <summary>Used to restore properties that were aquired from the clipboard.</summary>
		public void RestoreFromClipboard(Properties baseProperties,
			IPropertyObject propertyObject)
		{
			this.propertyObject = propertyObject;
			this.baseProperties = baseProperties;
			foreach (Property property in valueMap.Values)
				property.Properties = this;
			foreach (Property property in classMap.Values)
				property.Properties = this;
			ConnectBaseProperties();
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Link up our properties with their corresponding base properties.</summary>
		private void ConnectBaseProperties() {
			if (baseProperties != null) {
				// Link properties with base properties.
				Property[] properties = valueMap.Values.ToArray();
				foreach (Property property in properties) {
					Property baseProperty =
						baseProperties.GetPropertyValue(property.Name, true);
					property.BaseProperty = baseProperty;

					// Remove a redundant property (one that is equal to its base value).
					if (baseProperty?.EqualsValue(property) ?? false)
						valueMap.Remove(property.Name);
				}
				properties = classMap.Values.ToArray();
				foreach (Property property in properties) {
					Property baseProperty =
						baseProperties.GetPropertyClass(property.Name, true);
					property.BaseProperty = baseProperty;

					// Remove a redundant property (one that is equal to its base value).
					if (baseProperty?.EqualsValue(property) ?? false)
						classMap.Remove(property.Name);
				}
			}
			else {
				// Nullify all base properties
				foreach (Property property in valueMap.Values)
					property.BaseProperty = null;
				foreach (Property property in classMap.Values)
					property.BaseProperty = null;
			}
		}


		//-----------------------------------------------------------------------------
		// Debug Methods
		//-----------------------------------------------------------------------------

		/// <summary>Prints only the properties contained within this collection.</summary>
		public void Print() {
			foreach (Property property in GetProperties()) {
				Console.WriteLine(property.ToString());
			}
		}

		/// <summary>Prints all properties contained within this collection and its
		/// bases.</summary>
		public void PrintAll() {
			foreach (Property property in GetAllProperties()) {
				Console.WriteLine(property.ToString());
			}
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the var.</summary>
		protected override Property GetVar(string name) {
			return GetProperty(name, true);
		}

		/// <summary>Sets the var.</summary>
		protected override Property SetVar(string name, object value) {
			return SetProperty(name, value);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the object that contains these properties.</summary>
		public IPropertyObject PropertyObject {
			get { return propertyObject; }
			set { propertyObject = value; }
		}

		/// <summary>Gets the number of defined properties.</summary>
		public int Count {
			get { return valueMap.Count + classMap.Count(p => p.Value.IsModified); }
		}

		/// <summary>Get the base properties.</summary>
		public Properties BaseProperties {
			get { return baseProperties; }
			set { baseProperties = value; ConnectBaseProperties(); }
		}

		/// <summary>Gets the root base properties. Can be this.</summary>
		public Properties RootProperties {
			get {
				if (baseProperties != null)
					return baseProperties.RootProperties;
				return this;
			}
		}

		/// <summary>Returns true if the property collection has defined properties
		/// as well as a base collection.</summary>
		public bool HasModifiedProperties {
			get { return valueMap.Any() || classMap.Any(p => p.Value.IsModified); }
		}
	}
}
