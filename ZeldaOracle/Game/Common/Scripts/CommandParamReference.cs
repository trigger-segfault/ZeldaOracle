using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {

	public class CommandParamReference {

		private CommandParamType		type;
		private string					name;
		private CommandParamReference	nextParam;
		private CommandParamReference	parent;
		private CommandParamReference	children;
		private CommandParam			defaultValue;
		private object					value;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CommandParamReference() {
			this.type			= CommandParamType.String;
			this.nextParam		= null;
			this.parent			= null;
			this.children		= null;
			this.defaultValue	= null;
			this.value			= null;
			this.name			= "";
		}
		
		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static CommandParamType ParseCommandParamType(string paramTypeName) {
			paramTypeName = paramTypeName.ToLower();
			if (paramTypeName == "int" || paramTypeName == "integer")
				return CommandParamType.Integer;
			if (paramTypeName == "bool" || paramTypeName == "boolean")
				return CommandParamType.Boolean;
			if (paramTypeName == "float")
				return CommandParamType.Float;
			if (paramTypeName == "string" || paramTypeName == "str")
				return CommandParamType.String;
			if (paramTypeName == "any")
				return CommandParamType.Any;
			return CommandParamType.Unknown;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public CommandParamType Type {
			get { return type; }
			set { type = value; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
				
		// Arrays ---------------------------------------------------------------------
		
		public CommandParamReference NextParam {
			get { return nextParam; }
		}

		public CommandParamReference Parent {
			get { return parent; }
		}

		public CommandParamReference Children {
			get { return children; }
		}

		public int ChildCount {
			get { return (int) value; }
		}
		
		// Values ---------------------------------------------------------------------

		public int IntValue {
			get { return (int) value; }
		}
		
		public float FloatValue {
			get { return (float) value; }
		}
		
		public bool BoolValue {
			get { return (bool) value; }
		}
		
		public string StringValue {
			get { return (string) value; }
		}
		
		public CommandParam DefaultValue {
			get { return defaultValue; }
			set { defaultValue = value; }
		}
	}
}
