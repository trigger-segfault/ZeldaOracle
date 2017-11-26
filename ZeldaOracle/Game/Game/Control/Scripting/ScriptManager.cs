using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control.Scripting {

	public class ScriptManager {

		private Dictionary<string, Script> scripts;
		private byte[] rawAssembly;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptManager() {
			rawAssembly	= null;
			scripts		= new Dictionary<string, Script>();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Script GetScript(string scriptID) {
			if (scripts.ContainsKey(scriptID))
				return scripts[scriptID];
			return null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddScript(Script script) {
			scripts[script.ID] = script;
		}

		public void RemoveScript(Script script) {
			scripts.Remove(script.ID);
		}

		public void RemoveScript(string scriptID) {
			if (scripts.ContainsKey(scriptID))
				scripts.Remove(scriptID);
		}

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

		public bool ContainsScript(string scriptID) {
			return scripts.ContainsKey(scriptID);
		}

		// Compile all the scripts into one assembly.
		public void CompileAndWriteAssembly(World world) {
			var result = Compile(CreateCode(world, false));
			rawAssembly = result.RawAssembly;
		}

		// Compile all the scripts into one assembly.
		public ScriptCompileResult CompileScripts(World world, bool includeErrors) {
			return Compile(CreateCode(world, includeErrors));
		}

		// Compile all the scripts into one assembly.
		public ScriptCompileResult Compile(string code, bool generateAssembly = true, int firstLineStart = 0) {
			ScriptCompileResult result	= new ScriptCompileResult();
			string pathToAssembly = "";
			bool hasErrors = false;
			
			// Setup the compile options.
			CompilerParameters options	= new CompilerParameters();
			options.GenerateExecutable	= false;				// We want a Dll (Class Library)
			options.GenerateInMemory	= !generateAssembly;	// Save the assembly to a file.
			options.OutputAssembly		= "ZWD2CompiledScript.dll";

			// Add the assembly references.
			options.ReferencedAssemblies.Add(GetZeldaAPIAssembly().Location);

			// Create a C# code provider and compile the code.
			// The 'using' statement is necessary so the created DLL file isn't
			// locked when we try to load its contents.
			using (Microsoft.CSharp.CSharpCodeProvider csProvider = new Microsoft.CSharp.CSharpCodeProvider()) {
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
				File.Delete(pathToAssembly);
			}

			return result;
		}

		public string CreateCode(World world, bool includeErrors) {
			// Begin class and namespace.
			string code = CreateUsingsString() + CreateClassString();

			// Script methods.
			foreach (Script script in scripts.Values) {
				if (!script.HasErrors || includeErrors) {
					code += CreateMethodString(script);
					/*code += "public void RunScript_" + script.ID + "(" + CreateParametersString(script.Parameters) + ") {" +
							script.Code +
						"}";*/
				}
				else {
					code += CreateEmptyMethodString(script);
					Console.WriteLine(" ! Script '{0}' has errors!", script.ID);
				}
			}
			if (world != null) {
				int internalID = 0;
				foreach (IEventObject eventObject in world.GetEventObjects()) {
					foreach (Event evnt in eventObject.Events.GetEvents()) {
						if (evnt.IsDefined) {
							if ((!evnt.Script.HasErrors || includeErrors)) {
								string existingScript = evnt.GetExistingScript(scripts);
								if (existingScript == null) {
									evnt.Script.ID = "__internal_script_" + internalID + "__";
									code += CreateMethodString(evnt.Script);
									internalID++;
								}
							}
							else {
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

		public string CreateTestScriptCode(Script newScript, string newCode, out int scriptStart) {
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

		public static string CreateUsingsString() {
			return	"using System.Collections.Generic; " +
					"using Console = System.Console; " +
					"using ZeldaAPI; ";
		}

		public static string CreateClassString() {
			return	"namespace ZeldaAPI.CustomScripts {" +
						"public class CustomScript : CustomScriptBase { ";
		}

		public static string CreateClosingClassString() {
			return		"}" +
					"}";
		}

		public static string CreateTestMethodHeadString(Script script) {
			string name = (script.ID.StartsWith("__") || string.IsNullOrWhiteSpace(script.ID) ? "__internal_script__" : script.ID);
			return	"public void " + name + "(" + CreateParametersString(script.Parameters) + ") { ";
		}
		public static string CreateMethodString(Script script, string newCode = null) {
			return	"public void " + script.ID + "(" + CreateParametersString(script.Parameters) + ") { " +
						(newCode ?? script.Code) + 
					"}";
		}
		public static string CreateEmptyMethodString(Script script) {
			return "public void " + script.ID + "(" + CreateParametersString(script.Parameters) + ") { " +
						"" +
					"} ";
		}

		// Encapsulate the code inside a namsapce, class, and method.
		public static string CreateParametersString(List<ScriptParameter> scriptParameters) {
			string parametersString = "";
			for (int i = 0; i < scriptParameters.Count; i++) {
				if (i > 0)
					parametersString += ", ";
				parametersString += scriptParameters[i].Type + " " + scriptParameters[i].Name;
			}
			return parametersString;
		}
		
		// Get the assembly for the Zelda API.
		private static Assembly GetZeldaAPIAssembly() {
			return Assembly.GetAssembly(typeof(ZeldaAPI.Room));
		}

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
		
		public Dictionary<string, Script> Scripts {
			get { return scripts; }
		}
		
		public byte[] RawAssembly {
			get { return rawAssembly; }
			set { rawAssembly = value; }
		}
	}
}
