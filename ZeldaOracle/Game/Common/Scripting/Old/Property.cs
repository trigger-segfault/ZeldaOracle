using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripting {
	
	/// <summary>The possible raw data types for a property.</summary>
	/*[Serializable]
	public enum VarType {
		String,
		Integer,
		Float,
		Boolean,
		Point,
	}*/

	
	//public class PropertyValue {
	//	/// <summary>The data type of the property.</summary>
	//	public PropertyType Type { get; set; }
	//	/// <summary>The object value for the property. For lists, this is used to store the child count as an integer.</summary>
	//	public object ObjectValue { get; set; }
	//}
	
	public class PropertyValue {
		/// <summary>The object value for the property. For lists, this is used to store the child count as an integer.</summary>
		private object objectValue;

		public PropertyValue(PropertyValue copy) {
			this.objectValue = copy.objectValue;
		}
	}

	/// <summary>A proprety represents a piece of data that can be represented
	/// multiple types including lists of other properties.</summary>
	[Serializable]
	public class Property {
		/// <summary>The name of the property.</summary>
		private string name;
		/// <summary>The data type of the property.</summary>
		private VarType type;
		/// <summary>The object value for the property. For lists, this is used to store the child count as an integer.</summary>
		private object objectValue;
		/// <summary>If this property is part of a list of properties, then 'next' points to the next property in the list.</summary>
		private Property nextSibling;
		/// <summary>If this property is a list of properties, then 'firstChild' points to the first property in the list.</summary>
		private Property firstChild;
		/// <summary>The documentation for this property, if it is a base-property.</summary>
		[NonSerialized]
		private PropertyDocumentation documentation;
		/// <summary>The properties containing this property.</summary>
		[NonSerialized]
		private Properties properties;
		/// <summary>The base property that this property modifies.</summary>
		[NonSerialized]
		private Property baseProperty;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct a property with the given name and type.</summary>
		public Property(string name, VarType type = VarType.String) {
			this.name			= name;
			this.type			= type;
			this.objectValue	= 0;
			this.nextSibling	= null;
			this.firstChild		= null;
			this.documentation	= null;
			this.properties		= null;
			this.baseProperty	= null;
		}

		/// <summary>Construct a property as a copy of another.</summary>
		public Property(Property copy) {
			name			= copy.name;
			type			= copy.type;
			objectValue		= copy.objectValue;
			nextSibling		= null;
			firstChild		= null;
			documentation	= copy.documentation;
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
			return objectValue.Equals(other.objectValue);
		}

		/// <summary>Return true of this and another property have the equivilent values.</summary>
		public bool EqualsValue(object other) {
			if (type != TypeToPropertyType(other.GetType()))
				return false;
			return objectValue.Equals(other);
		}

		/// <summary>Find the root that this property is a modification of.</summary>
		public Property GetRootProperty() {
			return (baseProperty != null ? baseProperty.GetRootProperty() : this);
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string editorType,
			string editorSubType, string category, string description,
			bool isReadOnly = false, bool isBrowsable = true)
		{
			documentation = new PropertyDocumentation(readableName, editorType,
				editorSubType, category, description, isReadOnly, isBrowsable);
			return this;
		}

		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string editorType,
			Type editorSubType, string category, string description,
			bool isReadOnly = false, bool isBrowsable = true)
		{
			documentation = new PropertyDocumentation(readableName, editorType,
				editorSubType, category, description, isReadOnly, isBrowsable);
			return this;
		}
		
		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string category,
			string description)
		{
			documentation = new PropertyDocumentation(readableName, "", "", category,
				description, false, true);
			return this;
		}

		/// <summary>Marks the property as unbrowsable and read only in the documentation.</summary>
		public void Hide() {
			if (documentation == null)
				documentation = new PropertyDocumentation(name, "Misc", "");
			documentation.IsReadOnly = true;
			documentation.IsBrowsable = false;
		}

		/// <summary>Marks a property as browsable and writable in the documentation.</summary>
		public void Unhide() {
			if (documentation == null)
				documentation = new PropertyDocumentation(name, "Misc", "");
			documentation.IsReadOnly = false;
			documentation.IsBrowsable = true;
		}


		//-----------------------------------------------------------------------------
		// Static Factory Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Create a string property with the given value.</summary>
		public static Property CreateString(string name, string value) {
			Property p = new Property(name, VarType.String);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create an integer property with the given value.</summary>
		public static Property CreateInt(string name, int value) {
			Property p = new Property(name, VarType.Integer);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create a float property with the given value.</summary>
		public static Property CreateFloat(string name, float value) {
			Property p = new Property(name, VarType.Float);
			p.objectValue = value;
			return p;
		}

		/// <summary>Create a boolean property with the given value.</summary>
		public static Property CreateBool(string name, bool value) {
			Property p = new Property(name, VarType.Boolean);
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
		public static Type PropertyTypeToType(VarType propertyType) {
			if (propertyType == VarType.String)
				return typeof(string);
			if (propertyType == VarType.Integer)
				return typeof(int);
			if (propertyType == VarType.Float)
				return typeof(float);
			if (propertyType == VarType.Boolean)
				return typeof(bool);
			if (propertyType == VarType.Point)
				return typeof(Point2I);
			return null;
		}

		/// <summary>Convert a System.Type to a PropertyType.</summary>
		public static VarType TypeToPropertyType(Type type) {
			if (type == typeof(string))
				return VarType.String;
			if (type == typeof(int))
				return VarType.Integer;
			if (type == typeof(float))
				return VarType.Float;
			if (type == typeof(bool))
				return VarType.Boolean;
			if (type == typeof(Point2I))
				return VarType.Point;
			return VarType.Integer;
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

		/// <summary>Gets if the property has a base.</summary>
		public bool HasBase {
			get { return baseProperty != null; }
		}

		/* public PropertyAction Action {
			get { return action; }
			set { action = value; }
		}*/

		// Property Value ------------------------------------------------------------

		/// <summary>The data type of the property.</summary>
		public VarType Type {
			get { return type; }
		}

		/// <summary>The property's raw object value.</summary>
		public object ObjectValue {
			get { return objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The property value as a string.</summary>
		public string StringValue {
			get { return (string) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The property value as an integer.</summary>
		public int IntValue {
			get { return (int) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The property value as a float.</summary>
		public float FloatValue {
			get { return (float) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The property value as a boolean.</summary>
		public bool BoolValue {
			get { return (bool) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The property value as a point.</summary>
		public Point2I PointValue {
			get { return (Point2I) objectValue; }
			set { objectValue = value; }
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
