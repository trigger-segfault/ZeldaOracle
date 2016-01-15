using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
			scripts[script.Name] = script;
		}

		public void RemoveScript(Script script) {
			scripts.Remove(script.Name);
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
				result.RawAssembly = File.ReadAllBytes(pathToAssembly);
				//rawAssembly = result.RawAssembly;
				File.Delete(pathToAssembly);
			}
			else {
				//rawAssembly = null;
			}

			return result;
		}

		public string CreateCode() {
			// Begin class and namespace.
			string code = 
				"namespace ZeldaAPI.CustomScripts" +
				"{" +
					"public class CustomScript : CustomScriptBase" +
					"{";

			// Script methods.
			foreach (KeyValuePair<string, Script> entry in scripts) {
				Script script = entry.Value;
				if (!script.HasErrors) {
					code += "public void RunScript_" + script.Name + "(" + CreateParametersString(script.Parameters) + ")" +
						"{" +
							script.Code +
						"}";
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
