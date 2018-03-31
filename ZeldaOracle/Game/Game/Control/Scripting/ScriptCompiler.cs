using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ZeldaOracle.Game.Control.Scripting {
	/// <summary>Result information from compiling script code, including the 
	/// encountered errors/warnings and also the raw assembly data.</summary>
	public class ScriptCompileResult {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCompileResult() {
			Errors		= new List<ScriptCompileError>();
			Warnings	= new List<ScriptCompileError>();
			RawAssembly	= null;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Errors encountered during compilation.</summary>
		public List<ScriptCompileError> Errors { get; set; }

		/// <summary>Warnings encountered during compilation.</summary>
		public List<ScriptCompileError> Warnings { get; set; }
		
		/// <summary>True if there where no errors.</summary>
		public bool Succeeded {
			get { return (Errors.Count == 0); }
		}

		/// <summary>True if there where errors.</summary>
		public bool Failed {
			get { return (Errors.Count > 0); }
		}

		/// <summary>Raw assembly output from the compiler. If there where compiler
		/// errors, then this will be null.</summary>
		public byte[] RawAssembly { get; set; }
	}
	

	/// <summary>Represents a compiler error or warning.</summary>
	[Serializable]
	public class ScriptCompileError {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCompileError() {
			Line		= 0;
			Column		= 0;
			ErrorNumber	= "";
			ErrorText	= "";
			IsWarning	= false;
		}

		public ScriptCompileError(int line, int column, string errorNumber,
			string errorText, bool isWarning)
		{
			Line		= line;
			Column		= column;
			ErrorNumber	= errorNumber;
			ErrorText	= errorText;
			IsWarning	= isWarning;
		}
		
		public ScriptCompileError(ScriptCompileError copy) {
			Line		= copy.Line;
			Column		= copy.Column;
			ErrorNumber	= copy.ErrorNumber;
			ErrorText	= copy.ErrorText;
			IsWarning	= copy.IsWarning;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override string ToString() {
			return "ERROR" + " at line " + Line + " pos " + Column + ": " + ErrorText;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Line { get; set; }
		public int Column { get; set; }
		public string ErrorText { get; set; }
		public string ErrorNumber { get; set; }
		public bool IsWarning { get; set; }
	}


	/// <summary>Class used to compile code and return the compile result.</summary>
	public class ScriptCompiler {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCompiler() {
		}


		//-----------------------------------------------------------------------------
		// Compilation
		//-----------------------------------------------------------------------------

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
	}
}
