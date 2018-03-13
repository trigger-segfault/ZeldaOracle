using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Worlds;
using CSharpCodeProvider = Microsoft.CSharp.CSharpCodeProvider;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>The manager for storing and compiling scripts.</summary>
	public class ScriptManager {

		/// <summary>The collection of scripts.</summary>
		private Dictionary<string, Script> scripts;
		/// <summary>The raw data for the compiled script assembly.</summary>
		private byte[] rawAssembly;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the script manager.</summary>
		public ScriptManager() {
			scripts		= new Dictionary<string, Script>();
			rawAssembly = null;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the script with the specified ID.</summary>
		public Script GetScript(string scriptID) {
			Script script;
			scripts.TryGetValue(scriptID, out script);
			return script;
		}

		/// <summary>Returns true if the script exists in the collection.</summary>
		public bool ContainsScript(Script script) {
			return scripts.ContainsKey(script.ID);
		}

		/// <summary>Returns true if a script with the specified ID exists.</summary>
		public bool ContainsScript(string scriptID) {
			return scripts.ContainsKey(scriptID);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the script from the collection.</summary>
		public void AddScript(Script script) {
			scripts.Add(script.ID, script);
		}

		/// <summary>Removes the script from the collection.</summary>
		public void RemoveScript(Script script) {
			scripts.Remove(script.ID);
		}

		/// <summary>Removes the script with the specified id from the collection.
		/// </summary>
		public void RemoveScript(string scriptID) {
			scripts.Remove(scriptID);
		}

		/// <summary>Renames the specified script.</summary>
		public bool RenameScript(Script script, string newScriptID) {
			if (newScriptID != script.ID) {
				if (scripts.ContainsKey(newScriptID)) {
					return false;
				}
				scripts.Remove(script.ID);
				script.ID = newScriptID;
				scripts.Add(script.ID, script);
			}
			return true;
		}

		/// <summary>Renames the script with the specified ID.</summary>
		public bool RenameScript(string oldScriptID, string newScriptID) {
			return RenameScript(scripts[oldScriptID], newScriptID);
		}


		//-----------------------------------------------------------------------------
		// Compiling
		//-----------------------------------------------------------------------------
		
		/// <summary>Compile all the scripts and save the result to the raw
		/// assembly data.</summary>
		public ScriptCompileResult CompileAndWriteAssembly(World world) {
			// Compile the scripts and get the generated assembly
			Logs.Scripts.LogNotice("Compiling all scripts...");
			ScriptCompileResult result = Compile(CreateCode(world, false));
			rawAssembly = result.RawAssembly;
			
			// Log the compile result
			if (result.Errors.Count > 0 && result.Warnings.Count > 0) {
				Logs.Scripts.LogError(
					"There where {0} errors and {1} warnings during compilation:",
					result.Errors.Count, result.Warnings.Count);
			}
			else if (result.Errors.Count > 0) {
				Logs.Scripts.LogError(
					"There where {0} errors during compilation:",
					result.Errors.Count);
			}
			else if (result.Warnings.Count > 0) {
				Logs.Scripts.LogWarning(
					"There where {0} warnings during compilation:",
					result.Warnings.Count);
			}
			else {
				Logs.Scripts.LogNotice("Scripts compiled successfully!");
			}

			// Log individual errors and warnings
			foreach (var error in result.Errors)
				Logs.Scripts.LogError(error.ToString());
			foreach (var warning in result.Warnings)
				Logs.Scripts.LogWarning(warning.ToString());

			return result;
		}

		/// <summary>Compile all the scripts into one assembly.</summary>
		public ScriptCompileResult CompileScripts(World world, bool includeErrors,
			List<ScriptStart> scriptStarts = null)
		{
			return Compile(CreateCode(world, includeErrors, scriptStarts));
		}

		/// <summary>Compile the specified code.</summary>
		public ScriptCompileResult Compile(string code, bool generateAssembly = true,
			int firstLineStart = 0)
		{
			ScriptCompileResult result	= new ScriptCompileResult();
			string pathToAssembly = "";
			bool hasErrors = false;
			
			// Setup the compile options
			CompilerParameters options	= new CompilerParameters();
			options.GenerateExecutable	= false;			 // We want a DLL (Class Library)
			options.GenerateInMemory    = !generateAssembly; // Save the assembly to a file
			options.OutputAssembly		=
				Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + "Scripts.dll";

			// Add the assembly references
			options.ReferencedAssemblies.Add(Assemblies.ZeldaCommon.Location);
			options.ReferencedAssemblies.Add(Assemblies.ZeldaAPI.Location);

			// Create a C# code provider and compile the code.
			// The 'using' statement is necessary so the created DLL file isn't
			// locked when we try to load its contents.
			using (CSharpCodeProvider csProvider = new CSharpCodeProvider()) {
				CompilerResults csResult = csProvider.CompileAssemblyFromSource(options, code);
				pathToAssembly	= csResult.PathToAssembly;
				hasErrors		= csResult.Errors.HasErrors;

				// Disable System.IO access
				int index = code.IndexOf("System.IO");
				if (index != -1) {
					int line = code.Take(index).Count(c => c == '\n') + 1;
					int column = 0;
					for (int i = index; i >= 0; i--, column++) {
						if (code[i] == '\n')
							break;
					}
					if (line == 0)
						column -= firstLineStart;
					result.Errors.Add(new ScriptCompileError(line, column, "", "System.IO is not allowed", false));
				}

				// Copy warnings and errors into the ScriptComileResult result.
				foreach (CompilerError csError in csResult.Errors) {
					if (csError.Line == 0)
						csError.Column -= firstLineStart;
					ScriptCompileError error = new ScriptCompileError(csError.Line,
						csError.Column, csError.ErrorNumber, csError.ErrorText, csError.IsWarning);
					if (error.IsWarning)
						result.Warnings.Add(error);
					else
						result.Errors.Add(error);
				}
			}
			
			// If the compile was successful, then load the created.
			// DLL file into memory and then delete the file.
			if (!hasErrors && generateAssembly) {
				result.RawAssembly = File.ReadAllBytes(pathToAssembly);
				try {
					File.Delete(pathToAssembly);
				} catch { }
			}

			return result;
		}


		//-----------------------------------------------------------------------------
		// Code
		//-----------------------------------------------------------------------------

		/// <summary>Creates code used in compiling all scripts.</summary>
		public string CreateCode(World world, bool includeErrors,
			List<ScriptStart> scriptStarts = null)
		{
			// Begin class and namespace.
			string code = CreateUsingsString() + CreateClassString();

			// Script methods.
			int index = 0;
			foreach (Script script in scripts.Values) {
				if (scriptStarts != null)
					scriptStarts.Add(new ScriptStart(code.Length, script.ID));
				if (!script.HasErrors || includeErrors) {
					code += CreateMethodString(script);
				}
				else {
					code += CreateEmptyMethodString(script);
					Console.WriteLine(" ! Script '{0}' has errors!", script.ID);
				}
				index++;
			}

			// Iterate through all defined event scripts
			if (world != null) {
				int internalID = 0;
				foreach (IEventObject eventObject in world.GetEventObjects()) {
					foreach (Event evnt in eventObject.Events.GetEvents()) {
						if (evnt.IsDefined) {
							if ((!evnt.Script.HasErrors || includeErrors)) {
								string existingScript = evnt.GetExistingScript(scripts);
								if (existingScript == null) {
									evnt.Script.ID = CreateInternalScriptName(internalID++);
									if (scriptStarts != null)
										scriptStarts.Add(new ScriptStart(code.Length,
											evnt.Script.ID));
									code += CreateMethodString(evnt.Script);
								}
							}
							else {
								evnt.Script.ID = CreateInternalScriptName(internalID++);
								if (scriptStarts != null)
									scriptStarts.Add(new ScriptStart(code.Length,
										evnt.Script.ID));
								code += CreateEmptyMethodString(evnt.Script);
							}
						}
					}
				}
			}

			// Close class and namespace.
			code += CreateClosingClassString();

			return code;
		}

		/// <summary>Creates code for testing a single script.</summary>
		public string CreateTestScriptCode(Script newScript, string newCode,
			out int scriptStart)
		{
			// Begin class and namespace.
			string code = CreateUsingsString() + CreateClassString();
			
			code += CreateTestMethodHeadString(newScript);
			scriptStart = code.Length;
			code += newCode + "}";

			// Script methods.
			foreach (KeyValuePair<string, Script> entry in scripts) {
				Script script = entry.Value;
				if (script == newScript)
					continue;
				if (!script.IsHidden) {
					code += CreateEmptyMethodString(script);
				}
			}

			// Close class and namespace.
			code += CreateClosingClassString();

			return code;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		// Code -----------------------------------------------------------------------

		/// <summary>Creates a name for an internal script.</summary>
		public static string CreateInternalScriptName(int internalID) {
			return "__internal_script_" + internalID + "__";
		}

		/// <summary>Creates the default usings.</summary>
		public static string CreateUsingsString() {
			return	"using System.Collections.Generic; " +
					"using Console = System.Console; " +
					"using ZeldaAPI; " +
					"using ZeldaOracle.Game; " +
					"using ZeldaOracle.Game.API; " +
					"using ZeldaOracle.Common.Geometry; ";
		}

		/// <summary>Creates the opening namespace and class.</summary>
		public static string CreateClassString() {
			return	"namespace ZeldaAPI.CustomScripts {" +
						"public class CustomScript : CustomScriptBase { ";
		}

		/// <summary>Creates the closing braces for the class and namespace.</summary>
		public static string CreateClosingClassString() {
			return		"}" +
					"}";
		}

		/// <summary>Creates a method head used for testing a script.</summary>
		public static string CreateTestMethodHeadString(Script script) {
			string name = (script.IsHidden ? "__internal_script__" : script.ID);
			return	"public void " + name + "(" + CreateParametersString(script.Parameters) + ") { ";
		}

		/// <summary>Creates a method from the script.</summary>
		public static string CreateMethodString(Script script, string newCode = null) {
			return	"public void " + script.ID + "(" + CreateParametersString(script.Parameters) + ") { " +
						(newCode ?? script.Code) + 
					"}";
		}

		/// <summary>Creates an empty method from the script.</summary>
		public static string CreateEmptyMethodString(Script script) {
			return "public void " + script.ID + "(" + CreateParametersString(script.Parameters) + ") { " +
						"" +
					"} ";
		}

		/// <summary>Lists the parameters in a string for placing in a function declaration.</summary>
		public static string CreateParametersString(List<ScriptParameter> scriptParameters) {
			string parametersString = "";
			for (int i = 0; i < scriptParameters.Count; i++) {
				if (i > 0)
					parametersString += ", ";
				parametersString += scriptParameters[i].Type + " " + scriptParameters[i].Name;
			}
			return parametersString;
		}

		// Misc -----------------------------------------------------------------------

		/// <summary>Get the assembly for the Zelda Common.</summary>
		private static Assembly GetZeldaCommonAssembly() {
			return Assembly.GetAssembly(typeof(ZeldaOracle.Common.Geometry.Point2I));
		}

		/// <summary>Get the assembly for the Zelda API.</summary>
		private static Assembly GetZeldaAPIAssembly() {
			return Assembly.GetAssembly(typeof(ZeldaAPI.Game));
		}

		/// <summary>Returns true if the script has a valid function name.</summary>
		public static bool IsValidScriptName(string name) {
			if (name.Length == 0)
				return false;
			if (!char.IsLetter(name[0]) && name[0] != '_')
				return false;

			for (int i = 1; i < name.Length; i++) {
				char c = name[i];
				if (!char.IsLetterOrDigit(c) && c != '_') {
					return false;
				}
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of scripts.</summary>
		public ReadOnlyDictionary<string, Script> Scripts {
			get { return new ReadOnlyDictionary<string, Script>(scripts); }
		}

		/// <summary>Gets the raw data for the compiled script assembly.</summary>
		public byte[] RawAssembly {
			get { return rawAssembly; }
			set { rawAssembly = value; }
		}

		/// <summary>Gets the number of scripts stored in the script manager.</summary>
		public int ScriptCount {
			get { return scripts.Count; }
		}
	}
}
