using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting.Internal;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A proprety represents a piece of data that can be represented
	/// multiple types including lists of other properties.</summary>
	[Serializable]
	public class Property : VarBase {

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

		/// <summary>Constructs a property of the specified type.</summary>
		public Property(string name, VarType varType, ListType listType,
			int length = 0) : base(name, varType, listType, length)
		{
		}

		/// <summary>Constructs a property of the specified type.</summary>
		public Property(string name, Type type, int length = 0)
			: base(name, type, length)
		{
		}

		/// <summary>Constructs a property with the specified value.</summary>
		public Property(string name, object value) : base(name, value) {
		}

		/// <summary>Constructs a copy of the property.</summary>
		public Property(Property copy) : base(copy) {
			//properties		= copy.properties;
			//baseProperty	= copy.baseProperty;
			documentation	= copy.documentation;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string editorType,
			string editorSubType, string category, string description,
			bool isReadOnly = false, bool isBrowsable = true) {
			documentation = new PropertyDocumentation(readableName, editorType,
				editorSubType, category, description, isReadOnly, isBrowsable);
			return this;
		}

		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string editorType,
			Type editorSubType, string category, string description,
			bool isReadOnly = false, bool isBrowsable = true) {
			documentation = new PropertyDocumentation(readableName, editorType,
				editorSubType, category, description, isReadOnly, isBrowsable);
			return this;
		}

		/// <summary>Create the documentation for this property.</summary>
		public Property SetDocumentation(string readableName, string category,
			string description) {
			documentation = new PropertyDocumentation(readableName, "", "", category,
				description, false, true);
			return this;
		}

		/// <summary>Marks the property as unbrowsable and read only in the documentation.</summary>
		public void Hide() {
			if (documentation == null)
				documentation = new PropertyDocumentation(Name, "Misc", "");
			documentation.IsReadOnly = true;
			documentation.IsBrowsable = false;
		}

		/// <summary>Marks a property as browsable and writable in the documentation.</summary>
		public void Unhide() {
			if (documentation == null)
				documentation = new PropertyDocumentation(Name, "Misc", "");
			documentation.IsReadOnly = false;
			documentation.IsBrowsable = true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

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

		/// <summary>Get the root that this property is a modification of.</summary>
		public Property RootProperty {
			get { return (baseProperty?.RootProperty ?? this); }
		}

		/// <summary>Gets if the property has a base.</summary>
		public bool HasBase {
			get { return baseProperty != null; }
		}

		/// <summary>Gets if the property is modified from the base property or
		/// does not have a base property.</summary>
		public bool IsModified {
			get { return (baseProperty?.EqualsValue(this) ?? true); }
		}

		// Documentation -------------------------------------------------------------

		/// <summary>Documentation directly associated with this property.</summary>
		public PropertyDocumentation Documentation {
			get { return documentation ?? baseProperty?.Documentation; }
			set { documentation = value; }
		}

		/// <summary>Returns true if this property has documentation directly
		/// associated with it.</summary>
		public bool HasDocumentation {
			get {
				return (documentation != null ||
					(baseProperty?.HasDocumentation ?? false));
			}
		}

		/// <summary>Gets the readable name of the property.</summary>
		public string ReadableName {
			get { return (Documentation?.ReadableName ?? ""); }
		}

		/// <summary>Gets the readable name of the property or just the name if none is defined.</summary>
		public string FinalReadableName {
			get {
				if (!string.IsNullOrWhiteSpace(ReadableName))
					return ReadableName;
				return Name;
			}
		}

		/// <summary>Gets the editor type of the property.</summary>
		public string EditorType {
			get { return (Documentation?.EditorType ?? ""); }
		}

		/// <summary>Gets the editor subtype used for enum and enum_flags editor types.</summary>
		public string EditorSubType {
			get { return (Documentation?.EditorSubType ?? ""); }
		}

		/// <summary>Gets the category of the property.</summary>
		public string Category {
			get { return (Documentation?.Category ?? "Misc"); }
		}

		/// <summary>Gets the description of what the property does.</summary>
		public string Description {
			get { return (Documentation?.Description ?? ""); }
		}

		/// <summary>Can the property be edited using the property editor?<summary>
		public bool IsReadOnly {
			get { return (Documentation?.IsReadOnly ?? false); }
		}

		/// <summary>Is the property not shown in the property editor?<summary>
		public bool IsBrowsable {
			get { return (Documentation?.IsBrowsable ?? true); }
		}
	}
}
