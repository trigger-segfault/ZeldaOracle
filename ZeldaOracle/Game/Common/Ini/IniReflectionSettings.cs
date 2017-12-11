using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Ini {
	public abstract class IniReflectionSettings {

		//-----------------------------------------------------------------------------
		// Settings Attributes
		//-----------------------------------------------------------------------------

		/// <summary>Specifies that the property is an ini section that
		/// contains properties.</summary>
		[AttributeUsage(AttributeTargets.Property, Inherited = false)]
		protected class SectionAttribute : Attribute { }

		/// <summary>Specifies that the property should be surrounded in quotes.</summary>
		[AttributeUsage(AttributeTargets.Property, Inherited = false)]
		protected class UseQuotesAttribute : Attribute { }

		/// <summary>Specifies comments to save to the ini file.</summary>
		[AttributeUsage(AttributeTargets.Property, Inherited = false)]
		protected class CommentsAttribute : Attribute {
			/// <summary>The comments to display.</summary>
			public string Comments { get; private set; }

			/// <summary>Constructs the comments attribute.</summary>
			public CommentsAttribute(string comments) {
				this.Comments = comments;
			}
		}


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base ini reflection settings and initializes
		/// the values.</summary>
		protected IniReflectionSettings() {
			Reset();
		}

		
		//-----------------------------------------------------------------------------
		// Loading/Saving
		//-----------------------------------------------------------------------------

		/// <summary>Loads the settings from the ini file.</summary>
		public bool Load() {
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
			}
			catch (Exception ex) {
				PostLoadFailed(ex);
				return false;
			}
			if (!File.Exists(SettingsPath)) {
				Save();
			}
			else {
				// Keep this outside to prevent unwanted exception catching
				PreLoad();
				try {
					IniDocument document = new IniDocument();
					document.LoadFromFile(SettingsPath);
					LoadValues(document);
				}
				catch (Exception ex) {
					PostLoadFailed(ex);
					return false;
				}
			}

			// Keep this outside to prevent unwanted exception catching
			PostLoadSuccess();
			return true;
		}

		/// <summary>Saves the user settings.</summary>
		public bool Save() {
			// Keep this outside to prevent unwanted exception catching
			PreSave();
			try {
				IniDocument document = new IniDocument();
				SaveValues(document);
				document.SaveToFile(SettingsPath);
			}
			catch (Exception ex) {
				PostSaveFailed(ex);
				return false;
			}

			// Keep this outside to prevent unwanted exception catching
			PostSaveSuccess();
			return true;
		}

		/// <summary>Resets all values in the settings.</summary>
		public void Reset() {
			ResetValues(this.GetType(), this);
		}


		//-----------------------------------------------------------------------------
		// Virtual Loading/Saving
		//-----------------------------------------------------------------------------

		/// <summary>Called before loading commences only if the settings file exists.</summary>
		protected virtual void PreLoad() { }
		/// <summary>Called after loading finishes only if the load was successful.</summary>
		protected virtual void PostLoadSuccess() { }
		/// <summary>Called after loading finishes only if the load was unsuccessful.</summary>
		protected virtual void PostLoadFailed(Exception ex) { }
		/// <summary>Called before saving commences.</summary>
		protected virtual void PreSave() { }
		/// <summary>Called after saving finishes only if the save was successful.</summary>
		protected virtual void PostSaveSuccess() { }
		/// <summary>Called after saving finishes only if the save was unsuccessful.</summary>
		protected virtual void PostSaveFailed(Exception ex) { }


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the property's type matche the specified type.</summary>
		private static bool PropertyIs<T>(PropertyInfo propertyInfo) {
			return propertyInfo.PropertyType == typeof(T);
		}


		// Loading --------------------------------------------------------------------

		/// <summary>Loads the values from the ini document using reflection.</summary>
		private void LoadValues(IniDocument document) {
			Type type = this.GetType();

			IniSection globalSection = document.GlobalSection;

			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties) {
				var isSection = propertyInfo.GetCustomAttribute<SectionAttribute>();
				if (isSection != null) {
					LoadSection(document.Get(propertyInfo.Name), propertyInfo.PropertyType,
						propertyInfo.GetValue(this));
				}
				else {
					if (!LoadProperty(globalSection.Get(propertyInfo.Name), propertyInfo, this))
						ResetProperty(propertyInfo, this);
				}
			}
		}

		/// <summary>Loads the values from the ini section using reflection.</summary>
		private static void LoadSection(IniSection section, Type type, object instance) {
			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties) {
				if (section.Contains(propertyInfo.Name))
					LoadProperty(section.Get(propertyInfo.Name), propertyInfo, instance);
				else
					ResetProperty(propertyInfo, instance);
			}
		}

		/// <summary>Loads the value from the ini property using reflection.</summary>
		private static bool LoadProperty(IniProperty property, PropertyInfo propertyInfo, object instance) {
			object value = null;
			if (propertyInfo.PropertyType.IsEnum) {
				string text = property.GetString();
				try {
					value = Enum.Parse(propertyInfo.PropertyType, text);
				}
				catch {
					return false;
				}
			}
			else if (PropertyIs<string>(propertyInfo)) {
				value = property.GetString();
			}
			else if (PropertyIs<bool>(propertyInfo)) {
				bool tryValue;
				if (!property.TryGetBool(out tryValue)) return false;
				value = tryValue;
			}
			else if (PropertyIs<int>(propertyInfo)) {
				int tryValue;
				if (!property.TryGetInt(out tryValue)) return false;
				value = tryValue;
			}
			else if (PropertyIs<float>(propertyInfo)) {
				float tryValue;
				if (!property.TryGetFloat(out tryValue)) return false;
				value = tryValue;
			}
			else if (PropertyIs<Point2I>(propertyInfo)) {
				Point2I tryValue;
				if (!property.TryGetPoint(out tryValue)) return false;
				value = tryValue;
			}
			else if (PropertyIs<Vector2F>(propertyInfo)) {
				Vector2F tryValue;
				if (!property.TryGetVector(out tryValue)) return false;
				value = tryValue;
			}
			else {
				return false;
			}
			propertyInfo.SetValue(instance, value);
			return true;
		}


		// Saving ---------------------------------------------------------------------

		/// <summary>Saves the values to the ini document using reflection.</summary>
		private void SaveValues(IniDocument document) {
			Type type = this.GetType();
			IniSection globalSection = document.GlobalSection;
			globalSection.Comments = HeaderComments;

			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties) {
				var isSection = propertyInfo.GetCustomAttribute<SectionAttribute>();
				if (isSection != null) {
					IniSection section = SaveSection(propertyInfo, propertyInfo.GetValue(this));
					if (section.Any())
						document.Add(section);
				}
				else {
					IniProperty property = SaveProperty(propertyInfo, this);
					globalSection.Add(property);
				}
			}
		}

		/// <summary>Saves the values to the ini section using reflection.</summary>
		private IniSection SaveSection(PropertyInfo propertyInfo, object instance) {
			Type type = propertyInfo.PropertyType;
			
			IniSection section = new IniSection(
				propertyInfo.Name, GetComments(propertyInfo));

			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo subPropertyInfo in properties) {
				section.Add(SaveProperty(subPropertyInfo, instance));
			}
			return section;
		}

		/// <summary>Saves the value to the ini property using reflection.</summary>
		private IniProperty SaveProperty(PropertyInfo propertyInfo, object instance) {
			string value = propertyInfo.GetValue(instance).ToString();
			string comments = GetComments(propertyInfo);
			bool useQuotes = propertyInfo.GetCustomAttribute<UseQuotesAttribute>() != null;
			return new IniProperty(propertyInfo.Name, value, comments, useQuotes);
		}

		/// <summary>Gets the ini comments for the property using reflection.</summary>
		private string GetComments(PropertyInfo propertyInfo) {
			var commentsAttribute = propertyInfo.GetCustomAttribute<CommentsAttribute>();
			if (commentsAttribute != null)
				return commentsAttribute.Comments;
			return "";
		}

		// Resetting ------------------------------------------------------------------

		/// <summary>Resets all values in the settings.</summary>
		private void ResetValues(Type type, object instance) {
			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties) {
				var isSection = propertyInfo.GetCustomAttribute<SectionAttribute>();
				if (isSection != null) {
					ResetValues(propertyInfo.PropertyType, propertyInfo.GetValue(this));
				}
				else {
					ResetProperty(propertyInfo, instance);
				}
			}
		}

		/// <summary>Resets the property to its default value.</summary>
		private static void ResetProperty(PropertyInfo propertyInfo, object instance) {
			var attribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
			if (attribute != null)
				propertyInfo.SetValue(instance, attribute.Value);
		}
		

		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		/// <summary>The path to the settings file.</summary>
		protected abstract string SettingsPath { get; }


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>The comments to display at the top of the ini file.</summary>
		protected virtual string HeaderComments { get { return ""; } }


	}
}
