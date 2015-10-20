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
		private bool	isEditable;
		private bool	isHidden;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PropertyDocumentation() {
			readableName	= "";
			editorType		= "";
			editorSubType	= "";
			category		= "";
			description		= "";
			isEditable		= true;
			isHidden		= false;
		}

		public PropertyDocumentation(string readableName, string editorType, string editorSubType,
				string category, string description, bool isEditable, bool isHidden)
		{
			this.readableName	= readableName;
			this.editorType		= editorType;
			this.editorSubType	= editorSubType;
			this.category		= category;
			this.description	= description;
			this.isEditable		= isEditable;
			this.isHidden		= isHidden;
		}

		public PropertyDocumentation(PropertyDocumentation copy) {
			readableName	= copy.readableName;
			editorType		= copy.editorType;
			editorSubType	= copy.editorSubType;
			category		= copy.category;
			description		= copy.description;
			isEditable		= copy.isEditable;
			isHidden		= copy.isHidden;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ReadableName {
			get { return readableName; }
			set { readableName = value; }
		}

		public string EditorType {
			get { return editorType; }
			set { editorType = value; }
		}

		public string EditorSubType {
			get { return editorSubType; }
			set { editorSubType = value; }
		}

		public string Category {
			get { return category; }
			set { category = value; }
		}

		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		// Can the property be edited using the property editor?
		public bool IsEditable {
			get { return isEditable; }
			set { isEditable = value; }
		}

		// Is the property not shown in the property editor?
		public bool IsHidden {
			get { return isHidden; }
			set { isHidden = value; }
		}
	}
}
