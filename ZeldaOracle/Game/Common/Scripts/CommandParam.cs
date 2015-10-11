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
		Array
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

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CommandParam(string str) {
			stringValue = str;
			parent = null;
			type = CommandParamType.String;
			count = 0;
			// TODO: parse int/float.
			
			float.TryParse(str, out floatValue);
			int.TryParse(str, out intValue);
			bool.TryParse(str, out boolValue);
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
		
		public Point2I GetPoint(int index, Point2I defaultValue) {
			if (index >= count)
				return defaultValue;
			CommandParam p = GetParam(index);
			if (p.type == CommandParamType.Array && p.Count == 2)
				return new Point2I(p.GetInt(0), p.GetInt(1));
			return Point2I.Zero;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public CommandParamType Type {
			get { return type; }
			set { type = value; }
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

	}
}

