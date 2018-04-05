using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Control.Scripting {
	
	[Serializable]
	public class Script : IIDObject {

		// Script Definition ----------------------------------------------------------

		/// <summary>The script's identifier and name.</summary>
		private string id;
		/// <summary>User-entered code for the script.</summary>
		private string code;
		/// <summary>True if this script is visible in the editor. Hidden scripts are
		/// used in object events.</summary>
		private bool isHidden;
		/// <summary>Parameters that are passed into the script.</summary>
		private List<ScriptParameter> parameters;

		private string description;
		
		// Code Generation ------------------------------------------------------------

		/// <summary>Character offset for the beginning of this script's code within
		/// the entire comiled code.</summary>
		private int offsetInCode;
		/// <summary>Name of the method containing the script code.</summary>
		private string methodName;
		
		// Compile Result -------------------------------------------------------------

		/// <summary>Errors encountered when compiling this script.</summary>
		private List<ScriptCompileError> errors;
		/// <summary>Warnings encountered when compiling this script.</summary>
		private List<ScriptCompileError> warnings;

		// Loaded Assembly ------------------------------------------------------------

		/// <summary>The script's MethodInfo found in the loaded assembly.</summary>
		[NonSerialized]
		private MethodInfo methodInfo;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Script() {
			id   		= "";
			code		= "";
			description	= "";
			isHidden	= false;
			parameters	= new List<ScriptParameter>();

			errors		= new List<ScriptCompileError>();
			warnings	= new List<ScriptCompileError>();

			offsetInCode = 0;
			methodName = "";

			methodInfo	= null;
		}
		
		/// <summary>Copy constructor.</summary>
		public Script(Script copy) {
			id	    	= copy.id;
			code		= copy.code;
			description	= copy.description;
			isHidden	= copy.isHidden;

			// Parameters are never modified so they can be referenced in multiple
			// places
			parameters	= copy.parameters;

			// Copy errors and warnings
			errors = new List<ScriptCompileError>();
			for (int i = 0; i < copy.errors.Count; i++)
				errors.Add(new ScriptCompileError(copy.errors[i]));
			warnings = new List<ScriptCompileError>();
			for (int i = 0; i < copy.warnings.Count; i++)
				warnings.Add(new ScriptCompileError(copy.warnings[i]));

			offsetInCode = copy.offsetInCode;
			methodName = copy.methodName;
			methodInfo = copy.methodInfo;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Script Definition ----------------------------------------------------------

		/// <summary>The script's identifier and name.</summary>
		public string ID {
			get { return id; }
			set { id = value; }
		}

		/// <summary>User-entered code for the script.</summary>
		public string Code {
			get { return code; }
			set { code = value; }
		}

		/// <summary>User-entered code for the script.</summary>
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		/// <summary>Is this script visible in the editor? Hidden scripts are used in
		/// object events.</summary>
		public bool IsHidden {
			get { return isHidden; }
			set { isHidden = value; }
		}

		/// <summary>Parameters that are passed into the script.</summary>
		public List<ScriptParameter> Parameters {
			get { return parameters; }
			set { parameters = value; }
		}

		public int ParameterCount {
			get { return parameters.Count; }
		}

		// Code Generation-------------------------------------------------------------

		/// <summary>Character offset for the beginning of this script's code within
		/// the entire comiled code.</summary>
		public int OffsetInCode {
			get { return offsetInCode; }
			set { offsetInCode = value; }
		}
		/// <summary>Name of the method containing the script code.</summary>
		public string MethodName {
			get { return methodName; }
			set { methodName = value; }
		}
		
		// Compile Result -------------------------------------------------------------

		/// <summary>Errors encountered when compiling this script.</summary>
		public List<ScriptCompileError> Errors {
			get { return errors; }
			set { errors = value; }
		}

		/// <summary>Warnings encountered when compiling this script.</summary>
		public List<ScriptCompileError> Warnings {
			get { return warnings; }
			set { warnings = value; }
		}

		/// <summary>True if the script has compiler errors.</summary>
		public bool HasErrors {
			get { return (errors.Count > 0); }
		}

		/// <summary>True if the script has compiler warnings.</summary>
		public bool HasWarnings {
			get { return (warnings.Count > 0); }
		}

		// Loaded Assembly ------------------------------------------------------------

		/// <summary>The script's MethodInfo found in the loaded assembly.</summary>
		public MethodInfo MethodInfo {
			get { return methodInfo; }
			set { methodInfo = value; }
		}
	}
}
