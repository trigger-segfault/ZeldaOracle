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
		public ScriptCompileResult CompileScripts() {
			return Compile(CreateCode());
		}

		// Compile all the scripts into one assembly.
		public ScriptCompileResult Compile(string code) {
			ScriptCompileResult result	= new ScriptCompileResult();
			string pathToAssembly = "";
			bool hasErrors = false;
			
			// Setup the compile options.
			CompilerParameters options	= new CompilerParameters();
			options.GenerateExecutable	= false;	// We want a Dll (Class Library)
			options.GenerateInMemory	= false;	// Save the assembly to a file.
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
				
				// Copy warnings and errors into the ScriptComileResult result.
				foreach (CompilerError csError in csResult.Errors) {
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
			if (!hasErrors) {
				result.FilePath = pathToAssembly;
				result.Assembly = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), pathToAssembly));
				result.RawAssembly = File.ReadAllBytes(pathToAssembly);
				//rawAssembly = result.RawAssembly;
			}
			else {
				//rawAssembly = null;
			}

			return result;
		}

		public string CreateCode() {
			// Begin class and namespace.
			string code = 
				"namespace ZeldaAPI.CustomScripts {" +
					"public class CustomScript : CustomScriptBase {";

			// Script methods.
			foreach (KeyValuePair<string, Script> entry in scripts) {
				Script script = entry.Value;
				if (!script.HasErrors) {
					code += "public void RunScript_" + script.ID + "(" + CreateParametersString(script.Parameters) + ") {" +
							script.Code +
						"}";
				}
				else {
					Console.WriteLine(" ! Script '{0}' has errors!", script.ID);
				}
			}

			// Close class and namespace.
			code += "}" +
				"}";

			return code;
		}

		public string CreateCode(Script newScript, string newCode, out int scriptStart) {
			// Begin class and namespace.
			string code =
				"namespace ZeldaAPI.CustomScripts {" +
					"public class CustomScript : CustomScriptBase {";
			scriptStart = 0;
			// Script methods.
			foreach (KeyValuePair<string, Script> entry in scripts) {
				Script script = entry.Value;
				if (!script.HasErrors) {
					code += "public void RunScript_" + script.ID + "(" + CreateParametersString(script.Parameters) + ") {\n";
					if (script == newScript) {
						scriptStart = code.Length;
						code += newCode;
					}
					else {
						code += script.Code;
					}
					code += "}";
				}
				else {
					Console.WriteLine(" ! Script '{0}' has errors!", script.ID);
				}
			}

			// Close class and namespace.
			code += "}" +
				"}";

			return code;
		}

		// Encapsulate the code inside a namsapce, class, and method.
		private static string CreateParametersString(List<ScriptParameter> scriptParameters) {
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
