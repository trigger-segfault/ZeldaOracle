using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Ini {
	public class IniSection : IEnumerable<IniProperty> {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The name of the section.</summary>
		private string name;
		/// <summary>The description of the section.</summary>
		private string comments;
		/// <summary>The collection of properties in the section.</summary>
		private Dictionary<string, IniProperty> properties;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ini section with the specified name and optional comments.</summary>
		public IniSection(string name, string comments = "") {
			this.name       = name;
			this.comments   = comments;
			this.properties = new Dictionary<string, IniProperty>();
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Gets the enumerator for the properties in the section.</summary>
		public IEnumerator<IniProperty> GetEnumerator() {
			return properties.Values.GetEnumerator();
		}

		/// <summary>Gets the enumerator for the properties in the section.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			return properties.Values.GetEnumerator();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the property in the section with the specified name.</summary>
		public IniProperty Get(string propertyName, bool returnDefault = true) {
			IniProperty property;
			properties.TryGetValue(propertyName, out property);
			if (property == null && returnDefault)
				return new IniProperty(propertyName, "");
			return property;
		}

		/// <summary>Returns true if this section contains the specified property.</summary>
		public bool Contains(IniProperty property) {
			return properties.ContainsKey(property.Name);
		}

		/// <summary>Returns true if this section contains a property with the specified name.</summary>
		public bool Contains(string propertyName) {
			return properties.ContainsKey(propertyName);
		}
		
		/// <summary>Returns true if the section has any properties.</summary>
		public bool Any() {
			return properties.Any();
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the property to the section.</summary>
		public IniProperty Add(IniProperty property) {
			properties[property.Name] = property;
			return property;
		}

		/// <summary>Adds a new property to the section.</summary>
		public IniProperty Add(string propertyName, string value = "", string comments = "",
			bool useQuotes = false)
		{
			return Add(new IniProperty(propertyName, value, comments, useQuotes));
		}

		/// <summary>Removes the property from the section.</summary>
		public void Remove(IniProperty property) {
			properties.Remove(property.Name);
		}

		/// <summary>Removes the property with the specified name from the section.</summary>
		public void Remove(string propertyName) {
			properties.Remove(propertyName);
		}

		/// <summary>Removes all of the properties from the section.</summary>
		public void Clear() {
			properties.Clear();
		}


		//-----------------------------------------------------------------------------
		// Get Property Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the value of the property as an enum.</summary>
		public TEnum GetEnum<TEnum>(string propertyName, EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			return Get(propertyName).GetEnum<TEnum>(format);
		}

		/// <summary>Gets the string value of the property.</summary>
		public string GetString(string propertyName) {
			return Get(propertyName).GetString();
		}

		/// <summary>Gets the integer value of the property.</summary>
		public int GetInt(string propertyName) {
			return Get(propertyName).GetInt();
		}

		/// <summary>Gets the float value of the property.</summary>
		public float GetFloat(string propertyName) {
			return Get(propertyName).GetFloat();
		}

		/// <summary>Get the boolean value of the property.</summary>
		public bool GetBool(string propertyName) {
			return Get(propertyName).GetBool();
		}

		/// <summary>Get the point value of the property.</summary>
		public Point2I GetPoint(string propertyName) {
			return Get(propertyName).GetPoint();
		}

		/// <summary>Get the vector value of the property.</summary>
		public Vector2F GetVector(string propertyName) {
			return Get(propertyName).GetVector();
		}


		//-----------------------------------------------------------------------------
		// Try Get Property Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Trys to get the value of the property as an enum.</summary>
		public bool TryGetEnum<TEnum>(string propertyName, out TEnum result, EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			return Get(propertyName).TryGetEnum<TEnum>(out result);
		}

		/// <summary>Trys to get the integer value of the property.</summary>
		public bool TryGetInt(string propertyName, out int result) {
			return Get(propertyName).TryGetInt(out result);
		}

		/// <summary>Trys to get the float value of the property.</summary>
		public bool TryGetFloat(string propertyName, out float result) {
			return Get(propertyName).TryGetFloat(out result);
		}

		/// <summary>Trys to get the boolean value of the property.</summary>
		public bool TryGetBool(string propertyName, out bool result) {
			return Get(propertyName).TryGetBool(out result);
		}

		/// <summary>Trys to get the point value of the property.</summary>
		public bool TryGetPoint(string propertyName, out Point2I result) {
			return Get(propertyName).TryGetPoint(out result);
		}

		/// <summary>Trys to get the vector value of the property.</summary>
		public bool TryGetVector(string propertyName, out Vector2F result) {
			return Get(propertyName).TryGetVector(out result);
		}


		//-----------------------------------------------------------------------------
		// Set Property Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Set the value of the property as an enum.</summary>
		public void SetEnum<TEnum>(string propertyName, TEnum value, EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			Get(propertyName).SetEnum<TEnum>(value, format);
		}

		/// <summary>Sets the string value of the property.</summary>
		public void SetString(string propertyName, string value) {
			Get(propertyName).SetString(value);
		}

		/// <summary>Sets the integer value of the property.</summary>
		public void SetInt(string propertyName, int value) {
			Get(propertyName).SetInt(value);
		}

		/// <summary>Sets the float value of the property.</summary>
		public void SetFloat(string propertyName, float value) {
			Get(propertyName).SetFloat(value);
		}

		/// <summary>Sets the boolean value of the property.</summary>
		public void SetBool(string propertyName, bool value) {
			Get(propertyName).SetBool(value);
		}

		/// <summary>Sets the point value of the property.</summary>
		public void SetPoint(string propertyName, Point2I value) {
			Get(propertyName).SetPoint(value);
		}

		/// <summary>Sets the vector value of the property.</summary>
		public void SetVector(string propertyName, Vector2F value) {
			Get(propertyName).SetVector(value);
		}

		/// <summary>Sets the object value of the property.</summary>
		public void SetObject(string propertyName, object value) {
			Get(propertyName).SetObject(value);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name of the section.</summary>
		public string Name {
			get { return name; }
		}

		/// <summary>Gets or sets the description of the section.</summary>
		public string Comments {
			get { return comments; }
			set { comments = value; }
		}

		/// <summary>Gets the number of properties in the section.</summary>
		public int Count {
			get { return properties.Count; }
		}
	}
}
