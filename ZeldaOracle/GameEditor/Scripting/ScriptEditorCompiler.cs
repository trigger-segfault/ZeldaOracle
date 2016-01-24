using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Scripting {

	public static class ScriptEditorCompiler {

		//-----------------------------------------------------------------------------
		// Compiling Methods
		//-----------------------------------------------------------------------------

		// Compile a script asyncronously.
		public static Task<ScriptCompileResult> CompileScriptAsync(Script script) {
			string code = CreateScriptCompileCode(script);
			return Task.Run(() => CompileScript(code));
		}

		// Compile a script.
		public static ScriptCompileResult CompileScript(Script script) {
			string code = CreateScriptCompileCode(script);
			return CompileScript(code);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Compile the final code for a script only to find any errors and/or warnings.
		private static ScriptCompileResult CompileScript(string code) {
			// Setup the compile options.
			CompilerParameters options	= new CompilerParameters();
			options.GenerateExecutable	= false; // We want a Dll (Class Library)
			options.GenerateInMemory	= true;

			// Add the assembly references.
			Assembly zeldaApiAssembly = Assembly.GetAssembly(typeof(ZeldaAPI.CustomScriptBase));
			options.ReferencedAssemblies.Add(zeldaApiAssembly.Location);

			// Create a C# code provider and compile the code.
			Microsoft.CSharp.CSharpCodeProvider csProvider = new Microsoft.CSharp.CSharpCodeProvider();
			CompilerResults csResult = csProvider.CompileAssemblyFromSource(options, code);
				
			// Copy warnings and errors into the ScriptComileResult.
			ScriptCompileResult result	= new ScriptCompileResult();
			foreach (CompilerError csError in csResult.Errors) {
				ScriptCompileError error = new ScriptCompileError(csError.Line,
					csError.Column, csError.ErrorNumber, csError.ErrorText, csError.IsWarning);
				if (error.IsWarning)
					result.Warnings.Add(error);
				else
					result.Errors.Add(error);
			}

			return result;
		}

		// Create the final code that will be compiled from a script.
		private static string CreateScriptCompileCode(Script script) {
			// Create the parameters string.
			string parametersString = "";
			for (int i = 0; i < script.Parameters.Count; i++) {
				if (i > 0)
					parametersString += ", ";
				parametersString += script.Parameters[i].Type + " " + script.Parameters[i].Name;
			}

			return
				"namespace ZeldaAPI.CustomScripts" +
				"{" +
					"public class CustomScript : CustomScriptBase" +
					"{" +
						"public void RunScript(" + parametersString + ")" +
						"{" +
							script.Code +
						"}" +
					"}" +
				"}";
		}
	}
}
