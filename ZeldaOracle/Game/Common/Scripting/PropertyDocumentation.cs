using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripting {
	
	/// <summary>
	/// PropertyDocumentation
	/// </summary>
	public class PropertyDocumentation {
		private string	readableName; // A name that's more human-readable
		private string	editorType;
		private string	editorSubType; // Example: for enums, there needs to be a subtype for which enum.
		private string	category;
		private string	description;
		private bool	isReadOnly;
		private bool	isBrowsable;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PropertyDocumentation() {
			this.readableName	= "";
			this.editorType		= "";
			this.editorSubType	= "";
			this.category		= "Misc";
			this.description	= "";
			this.isReadOnly		= false;
			this.isBrowsable	= true;
		}

		public PropertyDocumentation(string readableName, string category, string description) {
			this.readableName   = readableName;
			this.editorType     = "";
			this.editorSubType  = "";
			this.category       = category;
			this.description    = description;
			this.isReadOnly     = false;
			this.isBrowsable    = true;
			if (string.IsNullOrWhiteSpace(this.category))
				this.category = "Misc";
		}

		public PropertyDocumentation(string readableName, string editorType, string editorSubType,
				string category, string description, bool isReadOnly = false, bool isBrowsable = true)
		{
			this.readableName	= readableName;
			this.editorType		= editorType;
			this.editorSubType	= editorSubType;
			this.category		= category;
			this.description	= description;
			this.isReadOnly		= isReadOnly;
			this.isBrowsable	= isBrowsable;
			if (string.IsNullOrWhiteSpace(this.category))
				this.category = "Misc";
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the readable name of the property.</summary>
		public string ReadableName {
			get { return readableName; }
		}

		/// <summary>Gets the editor type of the property.</summary>
		public string EditorType {
			get { return editorType; }
		}

		/// <summary>Gets the editor subtype used for enum and enum_flags editor types.</summary>
		public string EditorSubType {
			get { return editorSubType; }
		}

		/// <summary>Gets the category of the property.</summary>
		public string Category {
			get {
				if (string.IsNullOrWhiteSpace(category))
					return "Misc";
				return category;
			}
		}

		/// <summary>Gets the description of what the property does.</summary>
		public string Description {
			get { return description; }
		}

		/// <summary>Can the property be edited using the property editor?<summary>
		public bool IsReadOnly {
			get { return isReadOnly; }
		}

		/// <summary>Is the property not shown in the property editor?<summary>
		public bool IsBrowsable {
			get { return isBrowsable; }
		}
	}
}
