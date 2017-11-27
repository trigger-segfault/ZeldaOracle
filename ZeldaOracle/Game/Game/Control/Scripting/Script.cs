using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;
using ZeldaOracle.Common.Scripting;
using System.Collections;

namespace ZeldaOracle.Game.Control.Scripting {
	
	public class ScriptCompileResult {

		public ScriptCompileResult() {
			Errors		= new List<ScriptCompileError>();
			Warnings	= new List<ScriptCompileError>();
			RawAssembly	= null;
		}

		public List<ScriptCompileError> Errors { get; set; }
		public List<ScriptCompileError> Warnings { get; set; }
		public byte[] RawAssembly { get; set; }

	}

	public class ScriptCompileError {
		
		public ScriptCompileError() {
			Line		= 0;
			Column		= 0;
			ErrorNumber	= "";
			ErrorText	= "";
			IsWarning	= false;
		}

		public ScriptCompileError(int line, int column, string errorNumber, string errorText, bool isWarning) {
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

		public int Line { get; set; }
		public int Column { get; set; }
		public string ErrorText { get; set; }
		public string ErrorNumber { get; set; }
		public bool IsWarning { get; set; }

		public override string ToString() {
			return "ERROR" + " at line " + Line + " pos " + Column + ": " + ErrorText;
		}
	}

	public class Script : IIDObject {

		private string id; // The script's identifier and name
		private string code; // User-entered code for the script.
		private bool isHidden; // Is this script visible in the editor? Hidden scripts are used in object events.
		private List<ScriptCompileError> errors;
		private List<ScriptCompileError> warnings;
		private List<ScriptParameter> parameters; // Parameters that are passed into the script.

		//private MethodInfo method;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Script() {
			id   				= "";
			code				= "";
			isHidden			= false;
			errors				= new List<ScriptCompileError>();
			warnings			= new List<ScriptCompileError>();
			parameters			= new List<ScriptParameter>();
		}
		
		// Copy constructor.
		public Script(Script copy) {
			id	    			= copy.id;
			code				= copy.code;
			isHidden			= copy.isHidden;
			// Paremters are never modified so they can be referenced in multiple places
			parameters          = copy.parameters;
			errors				= new List<ScriptCompileError>();
			warnings			= new List<ScriptCompileError>();

			// Copy errors and warnings.
			for (int i = 0; i < copy.errors.Count; i++)
				errors.Add(new ScriptCompileError(copy.errors[i]));
			for (int i = 0; i < copy.warnings.Count; i++)
				warnings.Add(new ScriptCompileError(copy.warnings[i]));
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return id; }
			set { id = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}

		public bool IsHidden {
			get { return isHidden; }
			set { isHidden = value; }
		}

		public List<ScriptCompileError> Errors {
			get { return errors; }
			set { errors = value; }
		}

		public List<ScriptCompileError> Warnings {
			get { return warnings; }
			set { warnings = value; }
		}

		public List<ScriptParameter> Parameters {
			get { return parameters; }
			set { parameters = value; }
		}

		public int ParameterCount {
			get { return parameters.Count; }
		}

		public bool HasErrors {
			get { return (errors.Count > 0); }
		}

		public bool HasWarnings {
			get { return (warnings.Count > 0); }
		}
	}
}
