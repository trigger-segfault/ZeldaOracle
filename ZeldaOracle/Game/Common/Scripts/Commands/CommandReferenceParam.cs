using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {

	public class CommandReferenceParam {

		private CommandParamType		type;
		private string					name;
		private CommandReferenceParam	nextParam;
		private CommandReferenceParam	parent;
		private CommandReferenceParam	children;
		private CommandParam			defaultValue;
		private object					value;
		private bool					isVariadic;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CommandReferenceParam() :
			this(CommandParamType.String)
		{
		}
		
		public CommandReferenceParam(CommandParamType type) {
			this.type			= type;
			this.nextParam		= null;
			this.parent			= null;
			this.children		= null;
			this.defaultValue	= null;
			this.value			= null;
			this.name			= "";
			if (type == CommandParamType.Array)
				value = (int) 0;
		}

		
		//-----------------------------------------------------------------------------
		// Array Methods
		//-----------------------------------------------------------------------------
		
		// Get an enumerable list of an array paraneter's children.
		public IEnumerable<CommandReferenceParam> GetChildren() {
			for (CommandReferenceParam child = children; child != null; child = child.nextParam)
				yield return child;
		}

		// Add a parameter child.
		public CommandReferenceParam AddChild(CommandReferenceParam child) {
			if (type == CommandParamType.Array) {
				if (children == null) {
					children = child;
					value = (int) 1;
				}
				else {
					CommandReferenceParam lastChild = children;
					while (lastChild.NextParam != null)
						lastChild = lastChild.NextParam;
					lastChild.nextParam = child;
					value = ((int) value + 1);
				}
			}
			return child;
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
		
		public bool IsVariadic {
			get { return isVariadic; }
			set { isVariadic = value; }
		}
				
		// Arrays ---------------------------------------------------------------------
		
		public CommandReferenceParam NextParam {
			get { return nextParam; }
		}

		public CommandReferenceParam Parent {
			get { return parent; }
		}

		public CommandReferenceParam Children {
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
