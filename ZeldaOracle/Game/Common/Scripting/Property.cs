using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripting {
	
	public delegate void PropertyAction(IPropertyObject sender, object value);

	/// <summary>The possible raw data types for a property.</summary>
	public enum PropertyType {
		List,
		String,
		Integer,
		Float,
		Boolean,
		Point,
	}

	public class PropertyValue {
		/// <summary>The data type of the property.</summary>
		public PropertyType Type { get; set; }
		/// <summary>The object value for the property. For lists, this is used to store the child count as an integer.</summary>
		public object ObjectValue { get; set; }
	}


	/// <summary>A proprety represents a piece of data that can be represented
	/// multiple types including lists of other properties.</summary>
	public class Property {
		/// <summary>The name of the property.</summary>
		private string name;
		/// <summary>The data type of the property.</summary>
		private PropertyType type;
		/// <summary>The object value for the property. For lists, this is used to store the child count as an integer.</summary>
		private object objectValue;
		/// <summary>If this property is part of a list of properties, then 'next' points to the next property in the list.</summary>
		private Property nextSibling;
		/// <summary>If this property is a list of properties, then 'firstChild' points to the first property in the list.</summary>
		private Property firstChild;
		/// <summary>The documentation for this property, if it is a base-property.</summary>
		private PropertyDocumentation documentation;
		/// <summary>The action that occurs when the property is changed. NOTE: this is currently unused.</summary>
		private PropertyAction action;
		/// <summary>The properties containing this property.</summary>
		private Properties properties;
		/// <summary>The base property that this property modifies.</summary>
		private Property baseProperty;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct a property with the given name and type.</summary>
		public Property(string name, PropertyType type = PropertyType.String) {
			this.name			= name;
			this.type			= type;
			this.objectValue	= 0;
			this.nextSibling	= null;
			this.firstChild		= null;
			this.documentation	= null;
			this.action			= null;
			this.properties		= null;
			this.baseProperty	= null;

			// Add the property action.
			if (Resources.ContainsResource<PropertyAction>(name))
				this.action = Resources.GetResource<PropertyAction>(name);
		}

		/// <summary>Construct a property as a copy of another.</summary>
		public Property(Property copy) {
			name			= copy.name;
			type			= copy.type;
			objectValue		= copy.objectValue;
			nextSibling		= null;
			firstChild		= null;
			documentation	= copy.documentation;
			action			= copy.action;
			properties		= null;
			baseProperty	= null;

			if (copy.firstChild != null)
				firstChild = new Property(copy.firstChild);
			if (copy.nextSibling != null)
				nextSibling = new Property(copy.nextSibling);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Return true of this and another property have the equivilent values.</summary>
		public bool EqualsValue(Property other) {
			if (type != other.type || type == PropertyType.List)
				return false;
			return objectValue.Equals(other.objectValue);
		}

		/// <summary>Get the child at the given index (if this is a list).</summary>
		public Property GetChild(int index) {
			if (type != PropertyType.List)
				return null;

			Property p = firstChild;
			int i = 0;
			while (p != null) {
				if (i == index)
					return p;
				p = p.nextSibling;
				i++;
			}

			return null; // ERROR: Index out of bounds!
		}

		/// <summary>Find the root that this property is a modification of.</summary>
		public Property GetRootProperty() {
			return (baseProperty != null ? baseProperty.GetRootProperty() : this);
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string editorType, string editorSubType, string category,
				string description, bool isReadOnly = false, bool isBrowsable = true)
		{
			documentation = new PropertyDocumentation(readableName,
				editorType, editorSubType, category, description, isReadOnly, isBrowsable);
			return this;
		}

		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string category, string description)
		{
			documentation = new PropertyDocumentation(readableName, "", "", category, description, false, true);
			return this;
		}

		/// <summary>Set the property action to occur when this property is modified.</summary>
		public Property SetAction(PropertyAction action) {
			this.action = action;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Runs the property action.</summary>
		public void RunAction(IPropertyObject sender, object value) {
			if (action != null)
				action(sender, value);
		}


		//-----------------------------------------------------------------------------
		// Static Factory Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create a list of properties.</summary>
		public static Property CreateList(string name, params Property[] properties) {
			Property parent = new Property(name, PropertyType.List);
			parent.Count = properties.Length;

			if (properties.Length > 0) {
				Property p = new Property(properties[0]);
				parent.firstChild = p;
				for (int i = 0; i < properties.Length - 1; i++) {
					p.nextSibling = new Property(properties[i + 1]);
					p = p.nextSibling;
				}
			}
			return parent;
		}

		/// <summary>Create a string property with the given value.</summary>
		public static Property CreateString(string name, string value) {
			Property p = new Property(name, PropertyType.String);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create an integer property with the given value.</summary>
		public static Property CreateInt(string name, int value) {
			Property p = new Property(name, PropertyType.Integer);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create a float property with the given value.</summary>
		public static Property CreateFloat(string name, float value) {
			Property p = new Property(name, PropertyType.Float);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create a boolean property with the given value.</summary>
		public static Property CreateBool(string name, bool value) {
			Property p = new Property(name, PropertyType.Boolean);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create a boolean property with the given value.</summary>
		public static Property Create(string name, object value) {
			Property p = new Property(name, TypeToPropertyType(value.GetType()));
			p.objectValue = value;
			return p;
		}

		/// <summary>Convert a PropertyType to a System.Type.</summary>
		public static Type PropertyTypeToType(PropertyType propertyType) {
			if (propertyType == PropertyType.String)
				return typeof(string);
			if (propertyType == PropertyType.Integer)
				return typeof(int);
			if (propertyType == PropertyType.Float)
				return typeof(float);
			if (propertyType == PropertyType.Boolean)
				return typeof(bool);
			if (propertyType == PropertyType.Point)
				return typeof(Point2I);
			return null;
		}

		/// <summary>Convert a System.Type to a PropertyType.</summary>
		public static PropertyType TypeToPropertyType(Type type) {
			if (type == typeof(string))
				return PropertyType.String;
			if (type == typeof(int))
				return PropertyType.Integer;
			if (type == typeof(float))
				return PropertyType.Float;
			if (type == typeof(bool))
				return PropertyType.Boolean;
			if (type == typeof(Point2I))
				return PropertyType.Point;
			return PropertyType.Integer;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The name/key of the property.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>The properties container this property is contained in.</summary>
		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>The base property for this property.</summary>
		public Property BaseProperty {
			get { return baseProperty; }
			set { baseProperty = value; }
		}

		/* public PropertyAction Action {
			get { return action; }
			set { action = value; }
		}*/

		// Property Value ------------------------------------------------------------

		/// <summary>The data type of the property.</summary>
		public PropertyType Type {
			get { return type; }
		}

		/// <summary>The property's raw object value.</summary>
		public object ObjectValue {
			get { return objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		/// <summary>The property value as a string.</summary>
		public string StringValue {
			get { return (string) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		/// <summary>The property value as an integer.</summary>
		public int IntValue {
			get { return (int) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		/// <summary>The property value as a float.</summary>
		public float FloatValue {
			get { return (float) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		/// <summary>The property value as a boolean.</summary>
		public bool BoolValue {
			get { return (bool) objectValue; }
			set {
				objectValue = value;
				if (properties != null)
					RunAction(properties.PropertyObject, value);
			}
		}

		// Lists ---------------------------------------------------------------------

		/// <summary>The next sibling property (if this property is part of a list).</summary>
		public Property Next {
			get { return nextSibling; }
			set { nextSibling = value; }
		}

		/// <summary>The first child property in the list (if this property is a list).</summary>
		public Property FirstChild {
			get { return firstChild; }
			set { firstChild = value; }
		}

		/// <summary>Return the child at the given index.</summary>
		public Property this[int index] {
			get { return GetChild(index); }
		}

		/// <summary>Return the number of children.</summary>
		public int Count {
			get { return (int) objectValue; }
			set { objectValue = value; }
		}

		// Documentation -------------------------------------------------------------

		/// <summary>Documentation directly associated with this property.</summary>
		public PropertyDocumentation Documentation {
			get {
				if (documentation != null)
					return documentation;
				else if (baseProperty != null)
					return baseProperty.Documentation;
				return null;
			}
			set { documentation = value; }
		}

		/// <summary>Returns true if this property has documentation directly associated with it.</summary>
		public bool HasDocumentation {
			get { return (documentation != null || (baseProperty != null && baseProperty.HasDocumentation)); }
		}

		/// <summary>Gets the readable name of the property.</summary>
		public string ReadableName {
			get { return (HasDocumentation ? Documentation.ReadableName : ""); }
		}

		/// <summary>Gets the readable name of the property or just the name if none is defined.</summary>
		public string FinalReadableName {
			get {
				if (HasDocumentation && !string.IsNullOrWhiteSpace(Documentation.ReadableName))
					return Documentation.ReadableName;
				return name;
			}
		}

		/// <summary>Gets the editor type of the property.</summary>
		public string EditorType {
			get { return (HasDocumentation ? Documentation.EditorType : ""); }
		}

		/// <summary>Gets the editor subtype used for enum and enum_flags editor types.</summary>
		public string EditorSubType {
			get { return (HasDocumentation ? Documentation.EditorSubType : ""); }
		}

		/// <summary>Gets the category of the property.</summary>
		public string Category {
			get { return (HasDocumentation ? Documentation.Category : "Misc"); }
		}

		/// <summary>Gets the description of what the property does.</summary>
		public string Description {
			get { return (HasDocumentation ? Documentation.Description : ""); }
		}

		/// <summary>Can the property be edited using the property editor?<summary>
		public bool IsReadOnly {
			get { return (HasDocumentation ? Documentation.IsReadOnly : false); }
		}

		/// <summary>Is the property not shown in the property editor?<summary>
		public bool IsBrowsable {
			get { return (HasDocumentation ? Documentation.IsBrowsable : true); }
		}
	}
}
