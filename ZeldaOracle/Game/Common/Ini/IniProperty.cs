using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Ini {
	public class IniProperty {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The name of the property.</summary>
		private string name;
		/// <summary>The comments for the property.</summary>
		private string comments;
		/// <summary>The value of the property.</summary>
		private string value;

		// Save Settings
		/// <summary>True if the property uses quotes to format its value.</summary>
		private bool useQuotes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the ini property with the specified name, value, and settings.</summary>
		public IniProperty(string name, string value = "", string comments = "", bool useQuotes = false) {
			this.name       = name;
			this.comments   = comments;
			this.value      = value;
			this.useQuotes  = useQuotes;
		}


		//-----------------------------------------------------------------------------
		// Get Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the value of the property as an enum.</summary>
		public TEnum GetEnum<TEnum>(EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			if (format == EnumFormat.String) {
				TEnum result;
				Enum.TryParse(value, out result);
				return result;
			}
			else {
				return (TEnum) Enum.ToObject(typeof(TEnum), GetInt());
			}
		}

		/// <summary>Gets the string value of the property.</summary>
		public string GetString() {
			return value;
		}

		/// <summary>Gets the integer value of the property.</summary>
		public int GetInt() {
			int result;
			int.TryParse(value, out result);
			return result;
		}

		/// <summary>Gets the float value of the property.</summary>
		public float GetFloat() {
			float result;
			float.TryParse(value, out result);
			return result;
		}

		/// <summary>Get the boolean value of the property.</summary>
		public bool GetBool() {
			if (string.Compare(value, "true", true) == 0 ||
				string.Compare(value, "1") == 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>Get the point value of the property.</summary>
		public Point2I GetPoint() {
			Point2I result;
			Point2I.TryParse(value, out result);
			return result;
		}

		/// <summary>Get the vector value of the property.</summary>
		public Vector2F GetVector() {
			Vector2F result;
			Vector2F.TryParse(value, out result);
			return result;
		}


		//-----------------------------------------------------------------------------
		// Try Get Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Trys to get the value of the property as an enum.</summary>
		public bool TryGetEnum<TEnum>(out TEnum result, EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			try {
				if (format == EnumFormat.String)
					result = (TEnum) Enum.Parse(typeof(TEnum), value);
				else
					result = (TEnum) Enum.ToObject(typeof(TEnum), GetInt());
				return true;
			}
			catch {
				result = Activator.CreateInstance<TEnum>();
				return false;
			}
		}

		/// <summary>Trys to get the integer value of the property.</summary>
		public bool TryGetInt(out int result) {
			return int.TryParse(value, out result);
		}

		/// <summary>Trys to get the float value of the property.</summary>
		public bool TryGetFloat(out float result) {
			return float.TryParse(value, out result);
		}

		/// <summary>Trys to get the boolean value of the property.</summary>
		public bool TryGetBool(out bool result) {
			if (string.Compare(value, "true", true) == 0 ||
				string.Compare(value, "1") == 0)
			{
				result = true;
				return true;
			}
			if (string.Compare(value, "false", true) == 0 ||
				string.Compare(value, "0") == 0)
			{
				result = false;
				return true;
			}
			result = false;
			return false;
		}

		/// <summary>Trys to get the point value of the property.</summary>
		public bool TryGetPoint(out Point2I result) {
			return Point2I.TryParse(value, out result);
		}

		/// <summary>Trys to get the vector value of the property.</summary>
		public bool TryGetVector(out Vector2F result) {
			return Vector2F.TryParse(value, out result);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Set the value of the property as an enum.</summary>
		public void SetEnum<TEnum>(TEnum value, EnumFormat format = EnumFormat.String)
			where TEnum : struct, IConvertible
		{
			if (format == EnumFormat.String)
				this.value = value.ToString();
			else
				this.value = ((int)(object) value).ToString();
		}

		/// <summary>Sets the string value of the property.</summary>
		public void SetString(string value) {
			this.value = value;
		}

		/// <summary>Sets the integer value of the property.</summary>
		public void SetInt(int value) {
			this.value = value.ToString();
		}

		/// <summary>Sets the float value of the property.</summary>
		public void SetFloat(float value) {
			this.value = value.ToString();
		}

		/// <summary>Sets the boolean value of the property.</summary>
		public void SetBool(bool value) {
			this.value = value.ToString();
		}

		/// <summary>Sets the point value of the property.</summary>
		public void SetPoint(Point2I value) {
			this.value = value.X.ToString() + ", " + value.Y.ToString();
		}

		/// <summary>Sets the vector value of the property.</summary>
		public void SetVector(Vector2F value) {
			this.value = value.X.ToString() + ", " + value.Y.ToString();
		}

		/// <summary>Sets the object value of the property.</summary>
		public void SetObject(object value) {
			this.value = value.ToString();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name of the property.</summary>
		public string Name {
			get { return name; }
		}

		/// <summary>Gets or sets the comments for the property.</summary>
		public string Comments {
			get { return comments; }
			set { comments = value; }
		}

		/// <summary>Gets or sets the value of the property.</summary>
		public string Value {
			get { return value; }
			set { this.value = value; }
		}

		/// <summary>Gets or sets if the property uses quotes to format its value.</summary>
		public bool UseQuotes {
			get { return useQuotes; }
			set { useQuotes = value; }
		}
	}
}
