using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripts {

	public enum CommandParamType {
		Array,

		String,
		Integer,
		Float,
		Boolean,	
		Any,		// Any value that's not an array.

		Unknown,
	}


	public class CommandParam {
		
		private CommandParamType	type;
		private CommandParam		nextParam;
		private CommandParam		parent;
		private CommandParam		children;
		private int					count;
		private string				stringValue;
		private object				value;

		private int					lineIndex;	// Line index where this parameter is specified.
		private int					charIndex;	// Character index within the line this parameter is specified.
		private string				name;		// Only used for command parameters structure.

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public CommandParam() :
			this(CommandParamType.String)
		{
		}

		public CommandParam(CommandParamType type) {
			this.name			= "";
			this.parent			= null;
			this.children		= null;
			this.nextParam		= null;
			this.type			= type;
			this.count			= 0;
			this.value			= null;
			this.charIndex		= -1;
			this.lineIndex		= -1;
		}

		public CommandParam(string str) :
			this(CommandParamType.Any)
		{
			this.stringValue = str;
		}
		
		public CommandParam(CommandParam copy) {
			this.name			= copy.name;
			this.type			= copy.Type;
			this.parent			= null;
			this.nextParam		= null;
			this.value			= copy.value;
			this.stringValue	= copy.stringValue;
			this.charIndex		= copy.charIndex;
			this.lineIndex		= copy.lineIndex;

			// Copy array children.
			this.count = 0;
			if (type == CommandParamType.Array) {
				CommandParam copyChild = copy.Children;
				while (copyChild != null) {
					AddChild(new CommandParam(copyChild));
					copyChild = copyChild.NextParam;
				}
			}
		}

		public CommandParam(CommandReferenceParam referenceParam) :
			this()
		{
			this.name = referenceParam.Name;
			this.type = referenceParam.Type;

			// Copy array children.
			this.count = 0;
			if (type == CommandParamType.Array) {
				CommandReferenceParam copyChild = referenceParam.Children;
				while (copyChild != null) {
					AddChild(new CommandParam(copyChild));
					copyChild = copyChild.NextParam;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		// Returns true if this parameter is valid as the given type.
		public bool IsValidType(CommandParamType checkType) {
			if (type == CommandParamType.Array || checkType == CommandParamType.Array)
				return (type == checkType);
			if (checkType == CommandParamType.String || checkType == CommandParamType.Any)
				return true;
			if (checkType == CommandParamType.Integer) {
				int intValue;
				return int.TryParse(stringValue, out intValue);
			}
			if (checkType == CommandParamType.Float) {
				float floatValue;
				return float.TryParse(stringValue, out floatValue);
			}
			if (checkType == CommandParamType.Boolean) {
				bool boolValue;
				return bool.TryParse(stringValue, out boolValue);
			}
			return false;
		}

		// Set the value of the parameter by parsing the given string.
		public void SetValueByParse(string str) {
			this.stringValue = str;

			if (type == CommandParamType.String)
				this.value = str;
			else if (type == CommandParamType.Integer)
				this.value = int.Parse(str);
			else if (type == CommandParamType.Float)
				this.value = float.Parse(str);
			else if (type == CommandParamType.Boolean)
				this.value = bool.Parse(str);
		}

		
		//-----------------------------------------------------------------------------
		// Array Methods
		//-----------------------------------------------------------------------------
		
		// Get an enumerable list of an array paraneter's children.
		public IEnumerable<CommandParam> GetChildren() {
			for (CommandParam child = children; child != null; child = child.nextParam)
				yield return child;
		}

		// Get the child at the given index.
		public CommandParam GetParam(int index) {
			return this[index];
		}

		// Get an indexed child's string value.
		public string GetString(int index) {
			return GetParam(index).StringValue;
		}

		// Get an indexed child's boolean value.
		public bool GetBool(int index) {
			return GetParam(index).BoolValue;
		}
		
		// Get an indexed child's boolean value, with a default if the child doesn't exist.
		public bool GetBool(int index, bool defaultValue) {
			if (index < count)
				return GetParam(index).BoolValue;
			return defaultValue;
		}
				
		// Get an indexed child's integer value.
		public int GetInt(int index) {
			return GetParam(index).IntValue;
		}
				
		// Get an indexed child's float value.
		public float GetFloat(int index) {
			return GetParam(index).FloatValue;
		}
				
		// Get an indexed child's Point value.
		public Point2I GetPoint(int index) {
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.ChildCount == 2)
				return new Point2I(p.GetInt(0), p.GetInt(1));
			return Point2I.Zero;
		}
				
		// Get an indexed child's Vector value.
		public Vector2F GetVector(int index) {
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.ChildCount == 2)
				return new Vector2F(p.GetFloat(0), p.GetFloat(1));
			return Vector2F.Zero;
		}
		
		// Get an indexed child's Point value, with a default if the child doesn't exist.
		public Point2I GetPoint(int index, Point2I defaultValue) {
			if (index >= count)
				return defaultValue;
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.ChildCount == 2)
				return new Point2I(p.GetInt(0), p.GetInt(1));
			return Point2I.Zero;
		}
		
		// Add a parameter child.
		public CommandParam AddChild(CommandParam child) {
			if (type == CommandParamType.Array) {
				if (Children == null) {
					Children = child;
				}
				else {
					CommandParam lastChild = Children;
					while (lastChild.NextParam != null)
						lastChild = lastChild.NextParam;
					lastChild.NextParam = child;
				}
				count++;
			}
			return child;
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

		public CommandParam Parent {
			get { return parent; }
			set { parent = value; }
		}

		public CommandParam Children {
			get { return children; }
			set { children = value; }
		}
		
		public CommandParam NextParam {
			get { return nextParam; }
			set { nextParam = value; }
		}

		public int ChildCount {
			get { return count; }
			set { count = value; }
		}

		public CommandParam this[int index] {
			get { return GetChildren().ElementAt(index); }
		}

		// Values ---------------------------------------------------------------------

		public bool BoolValue {
			get {
				if (type == CommandParamType.Boolean)
					return (bool) value;
				else if (type == CommandParamType.String)
					return bool.Parse((string) stringValue);
				return false;
			}
		}

		public int IntValue {
			get {
				if (type == CommandParamType.Integer)
					return (int) value;
				else if (type == CommandParamType.String)
					return int.Parse((string) stringValue);
				return 0;
			}
		}
	
		public float FloatValue {
			get {
				if (type == CommandParamType.Float)
					return (float) value;
				else if (type == CommandParamType.String)
					return float.Parse((string) stringValue);
				return 0.0f;
			}
		}
	
		public string StringValue {
			get {
				if (type == CommandParamType.String)
					return (string) value;
				return stringValue;
			}
		}
			
		// Script Reader Info ----------------------------------------------------------------

		public int CharIndex {
			get { return charIndex; }
			set { charIndex = value; }
		}
	
		public int LineIndex {
			get { return lineIndex; }
			set { lineIndex = value; }
		}
	}
}

