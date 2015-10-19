using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripting {
	
	public delegate void PropertyAction(IPropertyObject sender, object value);

	/// <summary>
	/// The possible raw data types for a property.
	/// </summary>
	public enum PropertyType {
		List,
		String,
		Integer,
		Float,
		Boolean
	}


	/// <summary>
	/// A proprety represents a piece of data that can be represented
	/// multiple types including lists of other properties.
	/// </summary>
	public class Property {

		// The name of the property.
		private string name;
		// The data type of the property.
		private PropertyType type;
		// The object value for the property. For lists, this is used to store the child count as an integer.
		private object objectValue;
		// If this property is part of a list of properties, then 'next' points to the next property in the list.
		private Property next;
		// If this property is a list of properties, then 'firstChild' points to the first property in the list.
		private Property firstChild;
		// The documentation for this property, if it is a base-property.
		private PropertyDocumentation documentation;
		// The action that occurs when the property is changed.
		private PropertyAction action;
		// The properties containing this property.
		private Properties properties;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Construct a property with the given name and type.
		public Property(string name, PropertyType type = PropertyType.String) {
			this.name			= name;
			this.type			= type;
			this.objectValue	= 0;
			this.next			= null;
			this.firstChild		= null;
			this.documentation	= null;
			this.action			= null;
			this.properties		= null;
			
			// Add the property action.
			if (Resources.ExistsResource<PropertyAction>(name))
				this.action = Resources.GetResource<PropertyAction>(name);
		}
		
		// Construct a property as a copy of another.
		public Property(Property copy) {
			name			= copy.name;
			type			= copy.type;
			objectValue		= copy.objectValue;
			next			= null;
			firstChild		= null;
			documentation	= null;
			action			= copy.action;
			properties		= null;

			if (copy.firstChild != null)
				firstChild = new Property(copy.firstChild);
			if (copy.next != null)
				next = new Property(copy.next);
			if (copy.documentation != null)
				documentation = new PropertyDocumentation(copy.documentation);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		// Return true of this and another property have the equivilent values.
		public bool EqualsValue(Property other) {
			if (type != other.type || type == PropertyType.List)
				return false;
			return objectValue.Equals(other.objectValue);
		}

		// Get the child at the given index (if this is a list).
		public Property GetChild(int index) {
			if (type != PropertyType.List)
				return null;

			Property p = firstChild;
			int i = 0;
			while (p != null) {
				if (i == index)
					return p;
				p = p.next;
				i++;
			}

			return null; // ERROR: Index out of bounds!
		}

		// Find the root that this property is a modification of.
		public Property GetRootProperty() {
			if (properties.BaseProperties == null)
				return this;
			return properties.GetRootProperty(name);
		}

		// Get the root documentation for this property.
		public PropertyDocumentation GetRootDocumentation() {
			return GetRootProperty().Documentation;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Set the value of this property by another property.
		// NOTE: This won't work with lists.
		public void SetValue(Property other) {
			if (type == PropertyType.List || other.type == PropertyType.List) {
				Console.WriteLine("ERROR: Lists not supported in properties yet!");
				return;
			}
			type = other.type;
			ObjectValue = other.objectValue;
		}

		// Create the documentation for this property.
		public Property SetDocumentation(string readableName, string editorType, string editorSubType, string category,
				string description, bool isEditable = true, bool isHidden = false)
		{
			documentation = new PropertyDocumentation(readableName,
				editorType, editorSubType, category, description, isEditable, isHidden);
			return this;
		}

		// Set the property action to occur when this property is modified.
		public Property SetAction(PropertyAction action) {
			this.action = action;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		public void RunAction(IPropertyObject sender, object value) {
			if (action != null)
				action(sender, value);
		}


		//-----------------------------------------------------------------------------
		// Static Factory Methods
		//-----------------------------------------------------------------------------
		
		// Create a list of properties.
		public static Property CreateList(string name, params Property[] properties) {
			Property parent = new Property(name, PropertyType.List);
			parent.Count = properties.Length;

			if (properties.Length > 0) {
				Property p = new Property(properties[0]);
				parent.firstChild = p;
				for (int i = 0; i < properties.Length - 1; i++) {
					p.next = new Property(properties[i + 1]);
					p = p.next;
				}
			}
			return parent;
		}
		
		// Create a string property with the given value.
		public static Property CreateString(string name, string value) {
			Property p = new Property(name, PropertyType.String);
			p.objectValue = value;
			return p;
		}
		
		// Create an integer property with the given value.
		public static Property CreateInt(string name, int value) {
			Property p = new Property(name, PropertyType.Integer);
			p.objectValue = value;
			return p;
		}
		
		// Create a float property with the given value.
		public static Property CreateFloat(string name, float value) {
			Property p = new Property(name, PropertyType.Float);
			p.objectValue = value;
			return p;
		}
		
		// Create a boolean property with the given value.
		public static Property CreateBool(string name, bool value) {
			Property p = new Property(name, PropertyType.Boolean);
			p.objectValue = value;
			return p;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public PropertyType Type {
			get { return type; }
			set { type = value; }
		}

		public PropertyDocumentation Documentation {
			get { return documentation; }
			set { documentation = value; }
		}

		public bool HasDocumentation {
			get { return (documentation != null); }
		}

		// Values.

		public object ObjectValue {
			get { return objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		public string StringValue {
			get { return (string) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		public int IntValue {
			get { return (int) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		public float FloatValue {
			get { return (float) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		public bool BoolValue {
			get { return (bool) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		public object Value {
			get { return objectValue; }
		}

		// Lists.

		public Property Next {
			get { return next; }
			set { next = value; }
		}

		public Property FirstChild {
			get { return firstChild; }
			set { firstChild = value; }
		}

		// Return the child at the given index.
		public Property this[int index] {
			get { return GetChild(index); }
		}

		// Return the number of children
		public int Count {
			get { return (int) objectValue; }
			set { objectValue = value; }
		}

		public PropertyAction Action {
			get { return action; }
			set { action = value; }
		}

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}
	}
}
