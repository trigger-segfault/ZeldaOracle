using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Conscripts {
	/// <summary>The abstract class for executing a conscript.</summary>
	public abstract class ConscriptRunner {

		/// <summary>The current reader for this script.</summary>
		private ScriptReader reader;
		/// <summary>The script readers reading the scripts lower in the call stack.</summary>
		private Stack<ScriptReader> readerStack;
		
		/// <summary>The list of custom parameter types for the script commands.</summary>
		private CommandParamDefinitions typeDefinitions;
		/// <summary>The list of available commands for the script.</summary>
		private List<ScriptCommand> commands;

		/// <summary>The temporary resource library.</summary>
		private TemporaryResources tempResources;
		/// <summary>The previous temporary resources in the call stack.</summary>
		private Stack<TemporaryResources> tempResourcesStack;

		/// <summary>The collection of scripts that have already been called.
		/// Used to prevent the same script from being called multiple times.</summary>
		private HashSet<string> calledScripts;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base script runner.</summary>
		protected ConscriptRunner() {
			reader = null;
			readerStack = new Stack<ScriptReader>();
			typeDefinitions = new CommandParamDefinitions();
			commands = new List<ScriptCommand>();
			tempResources = new TemporaryResources();
			tempResourcesStack = new Stack<TemporaryResources>();
			calledScripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			//=====================================================================================
			// TYPE DEFINITIONS
			//=====================================================================================
			AddType("Color",
				"(int r, int g, int b)",
				"(int r, int g, int b, int a)"
			);
			AddType("Point",
				"(int x, int y)"
			);
			AddType("Vector",
				"(float x, float y)"
			);
			AddType("Rectangle",
				"(int x, int y, int width, int height)"
			);
			AddType("RectangleF",
				"(float x, float y, float width, float height)"
			);
			//=====================================================================================
			// LOAD SUBSCRIPT
			//=====================================================================================
			AddCommand("LOAD",
				"string relativeScriptPath, bool keepTempResources",
				"string relativeScriptPath",
			delegate (CommandParam parameters) {
				// Add the current script to the list of
				// scripts that cannot be called again.
				calledScripts.Add(PathHelper.NormalizePath(FileName));

				string path = Path.Combine(Directory, parameters.GetString(0));
				string normalizedPath = PathHelper.NormalizePath(path);

				// Check to make sure we don't enter an infinite
				// loop by calling the same script twice
				if (calledScripts.Contains(normalizedPath)) {
					throw new Exception("Infinite loop detected! '" + Path.GetFileName(path) +
						"' has already been called!");
				}

				bool keepTempResources = parameters.GetBool(1, false);
				if (!keepTempResources) {
					tempResourcesStack.Push(tempResources);
					tempResources = new TemporaryResources();
				}

				Resources.LoadScript(path, this);
				
				if (!keepTempResources) {
					tempResources = tempResourcesStack.Pop();
				}
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Command Creation (No Modes)
		//-----------------------------------------------------------------------------

		/// <summary>Add a command with no parameter format.</summary>
		protected void AddCommand(string name, Action<CommandParam> action) {
			AddCommand(name, new string[] { }, action);
		}

		/// <summary>Add a command with one parameter format.</summary>
		protected void AddCommand(string name, string params1, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1 }, action);
		}

		/// <summary>Add a command that handles 2 overloads.</summary>
		protected void AddCommand(string name, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2 }, action);
		}

		/// <summary>Add a command that handles 3 overloads.</summary>
		protected void AddCommand(string name, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3 }, action);
		}

		/// <summary>Add a command that handles 4 overloads.</summary>
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4 }, action);
		}

		/// <summary>Add a command that handles 5 overloads.</summary>
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4, params5 }, action);
		}

		/// <summary>Add a command that handles 6 overloads.</summary>
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}

		/// <summary>Add a command that handles the given list of overloads.</summary>
		protected void AddCommand(string name, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, null, parameterOverloads, action, typeDefinitions));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Single Mode)
		//-----------------------------------------------------------------------------

		/// <summary>Add a command with no parameter format.</summary>
		protected void AddCommand(string name, int mode, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { }, action);
		}

		/// <summary>Add a command with one parameter format.</summary>
		protected void AddCommand(string name, int mode, string params1, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1 }, action);
		}

		/// <summary>Add a command that handles 2 overloads.</summary>
		protected void AddCommand(string name, int mode, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2 }, action);
		}

		/// <summary>Add a command that handles 3 overloads.</summary>
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3 }, action);
		}

		/// <summary>Add a command that handles 4 overloads.</summary>
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4 }, action);
		}

		/// <summary>Add a command that handles 5 overloads.</summary>
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4, params5 }, action);
		}

		/// <summary>Add a command that handles 6 overloads.</summary>
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}

		/// <summary>Add a command that handles the given list of overloads.</summary>
		protected void AddCommand(string name, int mode, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, new int[] { mode }, parameterOverloads, action, typeDefinitions));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Multiple Modes)
		//-----------------------------------------------------------------------------

		/// <summary>Add a command with no parameter format.</summary>
		protected void AddCommand(string name, int[] modes, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { }, action);
		}

		/// <summary>Add a command with one parameter format.</summary>
		protected void AddCommand(string name, int[] modes, string params1, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1 }, action);
		}

		/// <summary>Add a command that handles 2 overloads.</summary>
		protected void AddCommand(string name, int[] modes, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2 }, action);
		}

		/// <summary>Add a command that handles 3 overloads.</summary>
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3 }, action);
		}

		/// <summary>Add a command that handles 4 overloads.</summary>
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4 }, action);
		}

		/// <summary>Add a command that handles 5 overloads.</summary>
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4, params5 }, action);
		}

		/// <summary>Add a command that handles 6 overloads.</summary>
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}

		/// <summary>Add a command that handles the given list of overloads.</summary>
		protected void AddCommand(string name, int[] modes, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, modes, parameterOverloads, action, typeDefinitions));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Final)
		//-----------------------------------------------------------------------------

		/// <summary>Add a script command.</summary>
		protected void AddCommand(ScriptCommand command) {
			commands.Add(command);
		}


		//-----------------------------------------------------------------------------
		// Type Definition Creation
		//-----------------------------------------------------------------------------

		/// <summary>Adds a custom script parameter type.</summary>
		protected void AddType(string name, params string[] parameterOverloads) {
			typeDefinitions.Add(new CommandParamDefinition(name, parameterOverloads, typeDefinitions));
		}


		//-----------------------------------------------------------------------------
		// Resource Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the resource with the specified name.<para/>
		/// Does error handling and temporary resources with "temp_" prefix.</summary>
		protected T GetResource<T>(string name, bool allowEmptyNames = false) {
			if (name.StartsWith("temp_")) {
				if (!tempResources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' does not exist!");
				return tempResources.GetResource<T>(name);
			}
			else {
				if (!Resources.Contains<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' does not exist!");
				return Resources.Get<T>(name, allowEmptyNames);
			}
		}

		/// <summary>Returns true if a resource with the specified name exists.<para/>
		/// Does temporary resources with "temp_" prefix.</summary>
		protected bool ContainsResource<T>(string name) {
			if (name.StartsWith("temp_")) {
				return tempResources.ContainsResource<T>(name);
			}
			else {
				return Resources.Contains<T>(name);
			}
		}

		/// <summary>Sets the resource with the specified name.<para/>
		/// Does error handling and temporary resources with "temp_" prefix.</summary>
		protected T SetResource<T>(string name, T resource) {
			if (name.StartsWith("temp_")) {
				tempResources.SetResource<T>(name, resource);
			}
			else {
				Resources.Set<T>(name, resource);
			}
			return resource;
		}

		/// <summary>Adds the resource with the specified name.<para/>
		/// Does error handling and temporary resources with "temp_" prefix.</summary>
		protected T AddResource<T>(string name, T resource) {
			if (name.StartsWith("temp_")) {
				if (tempResources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' already exists!");
				tempResources.AddResource<T>(name, resource);
			}
			else {
				if (Resources.Contains<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' already exists!");
				Resources.Add<T>(name, resource);
			}
			return resource;
		}


		//-----------------------------------------------------------------------------
		// Property Methods
		//-----------------------------------------------------------------------------

		/// <summary>Sets the object's properties from a set property command param.</summary>
		protected void SetProperty(IPropertyObject propertyObject,
			CommandParam parameters) {
			SetProperty(propertyObject.Properties, parameters);
		}

		/// <summary>Sets the properties from a set property command param.</summary>
		protected void SetProperty(Properties properties, CommandParam parameters) {
			parameters = parameters.GetParam(0);
			string name = parameters.GetString(0);
			Property property = properties.GetProperty(name, false);
			if (!properties.Contains(name))
				ThrowCommandParseError("Property with the name '" + name +
					"' does not exist!");

			object value = ParsePropertyValue(parameters.GetParam(1), property.Type);
			properties.SetGeneric(name, value);
		}

		/// <summary>Parses the value of the property from the command param.</summary>
		private object ParsePropertyValue(CommandParam param, VarType varType,
			ListType listType)
		{
			int count = param.ChildCount;
			Type elementType = VarBase.VarTypeToType(varType);
			switch (listType) {
			case ListType.Single: return ParsePropertyValue(param, varType);
			case ListType.Array:
				Array array = Array.CreateInstance(elementType, count);
				for (int i = 0; i < count; i++)
					array.SetValue(ParsePropertyValue(param.GetParam(i), varType), i);
				return array;
			case ListType.List:
				Type newType = typeof(List<>).MakeGenericType(elementType);
				IList list = (IList) Activator.CreateInstance(newType);
				for (int i = 0; i < count; i++)
					list.Add(ParsePropertyValue(param.GetParam(i), varType));
				return list;
			default:
				return null;
			}
		}

		/// <summary>Parses the value of the property from the command param.</summary>
		private object ParsePropertyValue(CommandParam param, VarType varType) {
			switch (varType) {
			case VarType.String:
				if (param.IsValidType(CommandParamType.String))
					return param.StringValue; break;
			case VarType.Integer:
				if (param.IsValidType(CommandParamType.String))
					return param.StringValue; break;
			case VarType.Float:
				if (param.IsValidType(CommandParamType.Float))
					return param.FloatValue; break;
			case VarType.Boolean:
				if (param.IsValidType(CommandParamType.Boolean))
					return param.BoolValue; break;
			case VarType.Point:
				if (param.IsValidType(CommandParamType.Array))
					return param.PointValue; break;
			case VarType.Vector:
				if (param.IsValidType(CommandParamType.Array))
					return param.VectorValue; break;
			case VarType.RangeI:
				if (param.IsValidType(CommandParamType.Array))
					return param.RangeIValue; break;
			case VarType.RangeF:
				if (param.IsValidType(CommandParamType.Array))
					return param.RangeFValue; break;
			case VarType.RectangleI:
				if (param.IsValidType(CommandParamType.Array))
					return param.RectangleIValue; break;
			case VarType.RectangleF:
				if (param.IsValidType(CommandParamType.Array))
					return param.RectangleFValue; break;
			}
			ThrowParseError("The property value '" + param.StringValue + "' is not of type " + varType.ToString());
			return null;
		}


		//-----------------------------------------------------------------------------
		// Errors Methods
		//-----------------------------------------------------------------------------

		/// <summary>Throw a parse error exception, optionally showing a caret.</summary>
		protected void ThrowParseError(string message, bool showCaret = true) {
			reader.ThrowParseError(message, showCaret);
		}

		/// <summary>Throw a parse error exception for a specific argument.</summary>
		protected void ThrowParseError(string message, CommandParam param) {
			reader.ThrowParseError(message, param);
		}

		/// <summary>Throw a parse error exception pointing to the command name.</summary>
		protected void ThrowCommandParseError(string message) {
			reader.ThrowCommandParseError(message);
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Begins reading the script.</summary>
		public void BeginReading(ScriptReader reader) {
			this.reader = reader;
			readerStack.Push(reader);
			OnBeginReading();
		}

		/// <summary>Ends reading the script.</summary>
		public void EndReading() {
			OnEndReading();
			readerStack.Pop();
			reader = (readerStack.Any() ? readerStack.Peek() : null);
		}

		/// <summary>Begins reading the script.</summary>
		protected virtual void OnBeginReading() { }

		/// <summary>Ends reading the script.</summary>
		protected virtual void OnEndReading() { }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the current mode of the reader used to determine valid commands.</summary>
		protected int Mode {
			get { return reader.Mode; }
			set { reader.Mode = value; }
		}

		/// <summary>Gets the file path for this conscript.</summary>
		public string FileName {
			get { return reader.FileName; }
		}

		/// <summary>Gets the file directory for this conscript.</summary>
		public string Directory {
			get { return reader.Directory; }
		}

		/// <summary>Gets or sets the script reader parsing the script.</summary>
		public ScriptReader Reader {
			get { return reader; }
		}

		/// <summary>Gets the list of commands for the script.</summary>
		public IReadOnlyList<ScriptCommand> Commands {
			get { return commands; }
		}

		/// <summary>Gets the command parameter type definititions for the script.</summary>
		public CommandParamDefinitions TypeDefinitions {
			get { return typeDefinitions; }
		}

		/// <summary>Gets the temporary resources for the script.</summary>
		public TemporaryResources TempResources {
			get { return tempResources; }
		}
	}
}
