using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>Information about a single script contained in a generated script
	/// code.</summary>
	public struct ScriptCodeGenerationInfo {
		/// <summary>The character offset where the script's code begins.</summary>
		public int Offset { get; }

		/// <summary>The character length of the script's code.</summary>
		public int Length { get; }

		/// <summary>The name of the script's method in code.</summary>
		public string MethodName { get; }

		public ScriptCodeGenerationInfo(string name, int offset, int length) {
			MethodName = name;
			Offset = offset;
			Length = length;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class GeneratedScriptCode {
		public Dictionary<Script, ScriptCodeGenerationInfo> ScriptInfo { get; } =
			new Dictionary<Script, ScriptCodeGenerationInfo>();
		public string Code { get; set; }
	}
	
	/// <summary>Generates a single, compilable block-of-code that contains all of a
	/// world's scripts.</summary>
	public class ScriptCodeGenerator {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>Class and namespace path to the script context class in the API.
		/// </summary>
		public static readonly string ScriptContextFullPath =
			"ZeldaAPI.CustomScripts.CustomScript";


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The world to generate code for.</summary>
		private World world;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCodeGenerator(World world = null) {
			this.world = world;
		}


		//-----------------------------------------------------------------------------
		// Code Generation
		//-----------------------------------------------------------------------------

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

		/// <summary>Generate code used to test-compile a single script.</summary>
		public GeneratedScriptCode GenerateTestCode(Script script, string code) {
			return GenerateTestCode(script, null, code);
		}
		
		/// <summary>Generate code used to test-compile a single trigger.</summary>
		public GeneratedScriptCode GenerateTestCode(Trigger trigger, string code) {
			return GenerateTestCode(null, trigger, code);
		}

		/// <summary>Creates code used in compiling all scripts.</summary>
		public GeneratedScriptCode GenerateCode(bool includeErrors) {
			GeneratedScriptCode result = new GeneratedScriptCode();

			// Begin namespace and class
			result.Code = CreateUsingsString() + "\n";
			result.Code += CreateNamespaceAndClassOpening() + "\n";

			// Generate script methods
			foreach (Script script in world.Scripts.Values) {
				if (!script.IsHidden) {
					result.Code += CreateScriptMethodOpening(script);

					string scriptCode = "";
					if (!script.HasErrors || includeErrors)
						scriptCode = script.Code;

					result.ScriptInfo[script] = new ScriptCodeGenerationInfo(
						script.ID, result.Code.Length, scriptCode.Length);

					if (!script.HasErrors || includeErrors)
						result.Code += scriptCode;
					result.Code += CreateScriptMethodClosing(script) + "\n";
				}
			}

			// Generate trigger script methods
			int index = 0;
			foreach (ITriggerObject triggerObject in world.GetEventObjects()) {
				foreach (Trigger trigger in triggerObject.Triggers) {
					if (trigger.Script != null) {
						string methodName = "";
						result.Code += CreateTriggerMethodOpening(
							trigger, index, out methodName);
						
						string scriptCode = "";
						if (!trigger.Script.HasErrors || includeErrors)
							scriptCode = trigger.Script.Code;

						result.ScriptInfo[trigger.Script] = new ScriptCodeGenerationInfo(
							methodName, result.Code.Length, scriptCode.Length);
						
						result.Code += scriptCode;
						result.Code += CreateTriggerMethodClosing() + "\n";
						index++;
					}
				}
			}

			// Close class and namespace
			result.Code += CreateClassAndNamespaceClosing();

			return result;
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Generate code used to test-compile a single script or trigger.
		/// </summary>
		private GeneratedScriptCode GenerateTestCode(
			Script testScript, Trigger trigger, string code)
		{
			GeneratedScriptCode result = new GeneratedScriptCode();
			
			// Begin namespace and class
			result.Code = CreateUsingsString() + "\n";
			result.Code += CreateNamespaceAndClassOpening() + "\n";

			// Generate empty script methods
			foreach (Script script in world.Scripts.Values) {
				if (!script.IsHidden) {
					result.Code += CreateScriptMethodOpening(script);
					if (script == testScript) {
						result.ScriptInfo[script] = new ScriptCodeGenerationInfo(
							script.ID, result.Code.Length, code.Length);
						result.Code += code;
					}
					result.Code += CreateScriptMethodClosing(script) + "\n";
				}
			}

			// Generate function for the trigger script to test
			if (trigger != null && trigger.Script != null) {
				string methodName = "";
				result.Code += CreateTriggerMethodOpening(trigger, 0, out methodName);
				result.ScriptInfo[trigger.Script] = new ScriptCodeGenerationInfo(
					methodName, result.Code.Length, code.Length);
				result.Code += code;
				result.Code += CreateTriggerMethodClosing() + "\n";
			}

			// Close class and namespace
			result.Code += CreateClassAndNamespaceClosing();

			return result;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		// Code Generation ------------------------------------------------------------

		/// <summary>Creates the default usings.</summary>
		public static string CreateUsingsString() {
			return	"using Console = System.Console;\n" +
					"using System.Collections;\n" +
					"using System.Collections.Generic;\n" +
					"using ZeldaAPI;\n" +
					"using ZeldaOracle.Common.Geometry;\n" +
					"using ZeldaOracle.Game;\n";
		}
		
		/// <summary>Creates the opening namespace and class.</summary>
		private static string CreateNamespaceAndClassOpening() {
			return	"namespace ZeldaAPI.CustomScripts {\n" +
					"    public class CustomScript : CustomScriptBase {\n";
		}

		/// <summary>Creates the closing braces for the class and namespace.</summary>
		private static string CreateClassAndNamespaceClosing() {
			return	"    }\n" +
					"}\n";
		}
		
		/// <summary>Creates the method opening for a script, including the script's
		/// description, name, and parameters.</summary>
		private static string CreateScriptMethodOpening(Script script) {
			return "        /// <summary>" +
				script.Description.Replace('\n', ' ') + "</summary>\n" +
				"        public void " + script.ID + "(" +
				CreateParametersString(script.Parameters) + ") {\n";
		}
		
		/// <summary>Creates the method closing for a script.</summary>
		private static string CreateScriptMethodClosing(Script script) {
			return	"\n        }\n";
		}
		
		/// <summary>Creates the method opening for a script, including the triggers's
		/// name, and 'This' parameter.</summary>
		private static string CreateTriggerMethodOpening(
			Trigger trigger, int triggerIndex, out string methodName)
		{
			ITriggerObject owner = trigger.Collection.TriggerObject;
			methodName = CreateTriggerMethodName(trigger, triggerIndex);
			Type ownerType = ReflectionHelper.GetApiObjectType(
				trigger.Collection.TriggerObject.TriggerObjectType);
			return "        public void " + methodName + "(" +
				ownerType.Name + " This) {\n";
		}

		/// <summary>Create a new method name for a trigger, by simply using the
		/// triggerIndex as a suffix.</summary>
		private static string CreateTriggerMethodName(
			Trigger trigger, int triggerIndex)
		{
			return string.Format("trigger_{0}", triggerIndex);
		}

		/// <summary>Creates the method closing for a trigger.</summary>
		private static string CreateTriggerMethodClosing() {
			return "\n        }\n";
		}
		
		/// <summary>Lists the parameters in a string for placing in a function
		/// declaration.</summary>
		private static string CreateParametersString(
			List<ScriptParameter> scriptParameters)
		{
			string parametersString = "";
			for (int i = 0; i < scriptParameters.Count; i++) {
				if (i > 0)
					parametersString += ", ";
				parametersString += scriptParameters[i].Type + " " +
					scriptParameters[i].Name;
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
			set { world = value; }
		}
	}
}
