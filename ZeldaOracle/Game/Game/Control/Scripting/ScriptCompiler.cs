using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ZeldaOracle.Game.Control.Scripting {

	public class ScriptCompiler {



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCompiler() {

		}

		/// <summary>Compile the specified code.</summary>
		public ScriptCompileResult Compile(string code, bool generateAssembly = true) {
			ScriptCompileResult result = new ScriptCompileResult();
			string pathToAssembly = "";
			bool hasErrors = false;
			
			// Setup the compile options
			CompilerParameters options = new CompilerParameters();
			options.GenerateExecutable = false;
			options.GenerateInMemory = !generateAssembly;
			options.OutputAssembly =
				Path.GetFileNameWithoutExtension(
					Assembly.GetEntryAssembly().Location) + "Scripts.dll";

			// Add the assembly references
			options.ReferencedAssemblies.Add(Assemblies.ZeldaCommon.Location);
			options.ReferencedAssemblies.Add(Assemblies.ZeldaAPI.Location);

			// Create a C# code provider and compile the code.
			// The 'using' statement is necessary so the created DLL file isn't
			// locked when we try to load its contents.
			using (CSharpCodeProvider csProvider = new CSharpCodeProvider()) {
				// Compile the source code
				CompilerResults csResult = csProvider.CompileAssemblyFromSource(
					options, code);
				pathToAssembly = csResult.PathToAssembly;
				hasErrors = csResult.Errors.HasErrors;
				
				// Disable System.IO access
				//int index = code.IndexOf("System.IO");
				//if (index != -1) {
				//	int line = code.Take(index).Count(c => c == '\n') + 1;
				//	int column = 0;
				//	for (int i = index; i >= 0; i--, column++) {
				//		if (code[i] == '\n')
				//			break;
				//	}
				//	if (line == 0)
				//		column -= firstLineStart;
				//	result.Errors.Add(new ScriptCompileError(
				//		line, column, "", "System.IO is not allowed", false));
				//}

				// Copy warnings and errors into the compile result
				foreach (CompilerError csError in csResult.Errors) {
					ScriptCompileError error = new ScriptCompileError(
						csError.Line, csError.Column, csError.ErrorNumber,
						csError.ErrorText, csError.IsWarning);
					if (error.IsWarning)
						result.Warnings.Add(error);
					else
						result.Errors.Add(error);
				}
			}

			// If the compile was successful, then load the DLL's file contents and
			// remove the DLL file.
			if (!hasErrors && generateAssembly) {
				result.RawAssembly = File.ReadAllBytes(pathToAssembly);
				try {
					File.Delete(pathToAssembly);
				} catch { }
			}

			return result;
		}

		//-----------------------------------------------------------------------------
		// Compilation
		//-----------------------------------------------------------------------------

	}
}
