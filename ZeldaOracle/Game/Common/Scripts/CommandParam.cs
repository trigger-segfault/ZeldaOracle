using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripts {

	public enum CommandParamType {
		String,
		Integer,
		Float,
		Boolean,
		Array,

		Any, // Any value that's not an array.

		Unknown,
	}


	public class CommandParam {
		
		private CommandParamType	type;
		private CommandParam		nextParam;
		private CommandParam		parent;
		private CommandParam		children;
		private int					count;
		private string				stringValue;
		private bool				boolValue;
		private int					intValue;
		private float				floatValue;

		private int					lineIndex; // Line index where this parameter is specified.
		private int					charIndex; // Character index within the line this parameter is specified.
		private string				name;	// Only used for command parameters structure.

		private CommandParam		defaultValue;

		
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
			this.stringValue	= "";
			this.floatValue		= 0.0f;
			this.intValue		= 0;
			this.boolValue		= false;
			this.charIndex		= -1;
			this.lineIndex		= -1;
			this.defaultValue	= null;
		}

		public CommandParam(string str) :
			this(CommandParamType.String)
		{
			this.stringValue = str;

			float.TryParse(str, out floatValue);
			int.TryParse(str, out intValue);
			bool.TryParse(str, out boolValue);
		}

		public CommandParam(CommandParam copy) {
			this.name			= copy.name;
			this.type			= copy.Type;
			this.parent			= null;
			this.defaultValue	= null;
			this.stringValue	= copy.stringValue;
			this.floatValue		= copy.floatValue;
			this.intValue		= copy.intValue;
			this.boolValue		= copy.boolValue;
			this.charIndex		= copy.charIndex;

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

		
		//-----------------------------------------------------------------------------
		// Type Checks
		//-----------------------------------------------------------------------------

		public bool IsValidType(CommandParamType checkType) {
			if (type == CommandParamType.Array || checkType == CommandParamType.Array)
				return (type == checkType);
			if (checkType == CommandParamType.String || checkType == CommandParamType.Any)
				return true;
			if (checkType == CommandParamType.Integer)
				return int.TryParse(stringValue, out intValue);
			if (checkType == CommandParamType.Float)
				return float.TryParse(stringValue, out floatValue);
			if (checkType == CommandParamType.Boolean)
				return bool.TryParse(stringValue, out boolValue);
			return false;
		}

		
		//-----------------------------------------------------------------------------
		// Array Parameters
		//-----------------------------------------------------------------------------
		
		public CommandParam GetParam(int index) {
			return this[index];
		}

		public string GetString(int index) {
			return GetParam(index).Str;
		}

		public bool GetBool(int index) {
			return GetParam(index).Boolean;
		}
		

		public bool GetBool(int index, bool defaultValue) {
			if (index < count)
				return GetParam(index).Boolean;
			return defaultValue;
		}
		
		public int GetInt(int index) {
			return GetParam(index).Integer;
		}
		
		public float GetFloat(int index) {
			return GetParam(index).floatValue;
		}
		
		public Point2I GetPoint(int index) {
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.Count == 2)
				return new Point2I(p.GetInt(0), p.GetInt(1));
			return Point2I.Zero;
		}
		
		public Vector2F GetVector(int index) {
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.Count == 2)
				return new Vector2F(p.GetFloat(0), p.GetFloat(1));
			return Vector2F.Zero;
		}
		
		public Point2I GetPoint(int index, Point2I defaultValue) {
			if (index >= count)
				return defaultValue;
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.Count == 2)
				return new Point2I(p.GetInt(0), p.GetInt(1));
			return Point2I.Zero;
		}
		
		
		//-----------------------------------------------------------------------------
		// Array Mutators
		//-----------------------------------------------------------------------------

		public void SetValueByParse(string value) {
			stringValue = value;
			
			float.TryParse(value, out floatValue);
			int.TryParse(value, out intValue);
			bool.TryParse(value, out boolValue);
		}

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
		
		public CommandParam NextParam {
			get { return nextParam; }
			set { nextParam = value; }
		}

		public CommandParam Parent {
			get { return parent; }
			set { parent = value; }
		}

		public CommandParam Children {
			get { return children; }
			set { children = value; }
		}

		public int Count {
			get { return count; }
			set { count = value; }
		}

		public bool Boolean {
			get { return boolValue; }
		}

		public int Integer {
			get { return intValue; }
		}
	
		public float Float {
			get { return floatValue; }
		}
	
		public string Str {
			get { return stringValue; }
		}
	
		public int CharIndex {
			get { return charIndex; }
			set { charIndex = value; }
		}
	
		public int LineIndex {
			get { return lineIndex; }
			set { lineIndex = value; }
		}

		public CommandParam this[int index] {
			get {
				CommandParam p = children;
				int i = 0;
				while (i < index && p != null) {
					p = p.nextParam;
					i++;
				}
				if (i != index || p == null) {
					// ERROR!!
					return null; 
				}
				return p;
			}
		}

		public CommandParam DefaultValue {
			get { return defaultValue; }
			set { defaultValue = value; }
		}
	}
}

