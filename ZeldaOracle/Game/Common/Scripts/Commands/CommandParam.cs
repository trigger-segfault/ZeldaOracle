using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Scripts.Commands {

	public enum CommandParamType {
		Array,

		Const,		// Can only be the name defined in the function
		String,
		Integer,
		Float,
		Boolean,	
		Any,		// Any value that's not an array.

		Custom,
		//Color,		// RGB or RGBA in an array
		//Point,		// integer XY in an array
		//Vector,		// floating XY in an array
		//Sprite,

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
		private string              commandPrefix;

		private int					lineIndex;	// Line index where this parameter is specified.
		private int					charIndex;	// Character index within the line this parameter is specified.
		private string				name;       // Only used for command parameters structure.

		private CommandParam		namedChildren;
		private int					namedCount;

		
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
			this.commandPrefix  = null;

			this.namedChildren	= null;
			this.namedCount		= 0;
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
			this.commandPrefix  = copy.commandPrefix;

			// Copy array children.
			this.count = 0;
			if (type == CommandParamType.Array) {
				foreach (CommandParam copyChild in copy.GetChildren())
					AddChild(new CommandParam(copyChild));
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
			if (checkType == CommandParamType.Any)
				return true;
			/*if (checkType == CommandParamType.Point && type == CommandParamType.Array && count == 2) {
				CommandParam x = children;
				CommandParam y = x.nextParam;
				int intValue;
				return	int.TryParse(x.stringValue, out intValue) &&
						int.TryParse(y.stringValue, out intValue);
			}
			if (checkType == CommandParamType.Vector && type == CommandParamType.Array && count == 2) {
				CommandParam x = children;
				CommandParam y = x.nextParam;
				float floatValue;
				return	float.TryParse(x.stringValue, out floatValue) &&
						float.TryParse(y.stringValue, out floatValue);
			}
			if (checkType == CommandParamType.Color && type == CommandParamType.Array &&
				count == 3 || count == 4) {
				CommandParam r = children;
				CommandParam g = r.nextParam;
				CommandParam b = g.nextParam;
				CommandParam a = b.nextParam;
				byte byteValue;
				return	byte.TryParse(r.stringValue, out byteValue) &&
						byte.TryParse(g.stringValue, out byteValue) &&
						byte.TryParse(b.stringValue, out byteValue) &&
						(count == 3 || byte.TryParse(a.stringValue, out byteValue));
			}
			if (checkType == CommandParamType.Sprite) {
				if (type != CommandParamType.Array)
					return true;
				else if (count == 2) {
					CommandParam param1 = children;
					CommandParam param2 = param1.nextParam;
					if (param1.type == CommandParamType.Array)
						return param1.IsValidType(CommandParamType.Point) && param2.IsValidType(CommandParamType.String);
					else if (param2.type == CommandParamType.Array)
						return param1.IsValidType(CommandParamType.String) && param2.IsValidType(CommandParamType.Point);
					else
						return true;
				}
				else if (count == 3) {
					CommandParam param1 = children;
					CommandParam param2 = param1.nextParam;
					CommandParam param3 = param2.nextParam;
					return param1.IsValidType(CommandParamType.String) && param2.IsValidType(CommandParamType.Point) &&
						param3.IsValidType(CommandParamType.String);
				}
				return false;
			}*/
			if (type == CommandParamType.Array || checkType == CommandParamType.Array)
				return (type == checkType);
			if (checkType == CommandParamType.String || checkType == CommandParamType.Const)
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
			
			if (type == CommandParamType.String || type == CommandParamType.Const)
				this.value = str;
			else if (type == CommandParamType.Integer)
				this.value = int.Parse(str);
			else if (type == CommandParamType.Float)
				this.value = float.Parse(str);
			else if (type == CommandParamType.Boolean)
				this.value = bool.Parse(str);
		}


		//-----------------------------------------------------------------------------
		// Prefix
		//-----------------------------------------------------------------------------

		public bool HasPrefix(string prefix = null) {
			if (prefix == null)
				return !string.IsNullOrEmpty(commandPrefix);
			else
				return string.Compare(commandPrefix, prefix, true) == 0;
		}


		//-----------------------------------------------------------------------------
		// Array Methods
		//-----------------------------------------------------------------------------

		// Get an enumerable list of an array parameter's children.
		public IEnumerable<CommandParam> GetChildren() {
			for (CommandParam child = children; child != null; child = child.nextParam)
				yield return child;
		}

		// Get an enumerable list of named parameter children.
		public IEnumerable<CommandParam> GetNamedChildren() {
			for (CommandParam child = namedChildren; child != null; child = child.nextParam)
				yield return child;
		}

		// Get the child at the given index.
		public CommandParam GetParam(int index) {
			return this[index];
		}

		// Get the named parameter at the given index.
		public CommandParam GetNamedParam(int index) {
			return GetNamedChildren().ElementAt(index);
		}

		// Get the named parameter with the given name.
		public CommandParam GetNamedParam(string name) {
			for (CommandParam child = namedChildren; child != null; child = child.nextParam) {
				if (string.Compare(child.name, name, true) == 0)
					return child;
			}
			return null;
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

		// Get an indexed child's Vector value, with a default if the child doesn't exist.
		public Vector2F GetVector(int index, Point2I defaultValue) {
			if (index >= count)
				return defaultValue;
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.ChildCount == 2)
				return new Vector2F(p.GetFloat(0), p.GetFloat(1));
			return Vector2F.Zero;
		}

		public Color GetColor(int index) {
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && (p.count == 3 || p.count == 4))
				return new Color(p.GetInt(0), p.GetInt(1), p.GetInt(2),
					p.count == 3 ? 255 : p.GetInt(3));
			return Color.Black;
		}

		// Add a parameter child.
		public CommandParam AddChild(CommandParam child) {
			if (type == CommandParamType.Array) {
				if (children == null) {
					children = child;
				}
				else {
					CommandParam lastChild = children;
					while (lastChild.NextParam != null)
						lastChild = lastChild.NextParam;
					lastChild.NextParam = child;
				}
				count++;
			}
			return child;
		}

		// Add a named parameter child.
		public CommandParam AddNamedChild(CommandParam child) {
			if (type == CommandParamType.Array) {
				if (namedChildren == null) {
					namedChildren = child;
				}
				else {
					CommandParam lastChild = namedChildren;
					while (lastChild.NextParam != null)
						lastChild = lastChild.NextParam;
					lastChild.NextParam = child;
				}
				namedCount++;
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

		public string Prefix {
			get { return commandPrefix; }
			set { commandPrefix = value; }
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

		public CommandParam NamedChildren {
			get { return namedChildren; }
			set { namedChildren = value; }
		}

		public bool HasNamedParams {
			get { return namedChildren != null; }
		}

		public int NamedChildCount {
			get { return namedCount; }
			set { namedCount = value; }
		}

		// Values ---------------------------------------------------------------------

		public bool BoolValue {
			get {
				if (type == CommandParamType.Boolean)
					return (bool) value;
				else if (type == CommandParamType.String || type == CommandParamType.Any)
					return bool.Parse((string) stringValue);
				return false;
			}
		}

		public int IntValue {
			get {
				if (type == CommandParamType.Integer)
					return (int) value;
				else if (type == CommandParamType.String || type == CommandParamType.Any)
					return int.Parse((string) stringValue);
				return 0;
			}
		}
	
		public float FloatValue {
			get {
				if (type == CommandParamType.Float)
					return (float) value;
				else if (type == CommandParamType.String || type == CommandParamType.Any)
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

