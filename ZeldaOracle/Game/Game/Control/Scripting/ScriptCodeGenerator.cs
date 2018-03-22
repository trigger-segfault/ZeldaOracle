using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control.Scripting {

	public struct ScriptCodeGenerationInfo {
		public int Offset { get; set; }
		public string MethodName { get; set; }

		public ScriptCodeGenerationInfo(string name, int offset) {
			MethodName = name;
			Offset = offset;
		}
	}

	public class GeneratedScriptCode {
		public Dictionary<Script, ScriptCodeGenerationInfo> ScriptInfo { get; } =
			new Dictionary<Script, ScriptCodeGenerationInfo>();
		public string Code { get; set; }
	}


	public class ScriptCodeGenerator {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public static readonly string ScriptContextFullPath =
			"ZeldaAPI.CustomScripts.CustomScript";

		private World world;

		public ScriptCodeGenerator(World world = null) {
			this.world = world;
		}


		//-----------------------------------------------------------------------------
		// Code Generation
		//-----------------------------------------------------------------------------

		public GeneratedScriptCode GenerateTestCode(Script script, string code) {
			return GenerateTestCode(script, null, code);
		}
		
		public GeneratedScriptCode GenerateTestCode(Trigger trigger, string code) {
			return GenerateTestCode(null, trigger, code);
		}

		private GeneratedScriptCode GenerateTestCode(Script testScript, Trigger trigger, string code) {
			GeneratedScriptCode result = new GeneratedScriptCode();
			
			// Begin namespace and class
			result.Code = CreateUsingsString() + "\n";
			result.Code += CreateNamespaceAndClassOpening() + "\n";

			// Generate empty script methods
			foreach (Script script in world.Scripts.Values) {
				if (!script.IsHidden) {
					result.Code += CreateScriptMethodOpening(script);
					if (script == testScript) {
						result.ScriptInfo[script] = new ScriptCodeGenerationInfo() {
							MethodName = script.ID,
							Offset = result.Code.Length,
						};
						result.Code += code;
					}
					result.Code += CreateScriptMethodClosing(script) + "\n";
				}
			}

			// Generate function for the trigger script to test
			if (trigger != null && trigger.Script != null) {
				result.Code += CreateTriggerMethodOpening(trigger, 0);
				result.ScriptInfo[trigger.Script] = new ScriptCodeGenerationInfo() {
					MethodName = trigger.Script.ID,
					Offset = result.Code.Length,
				};
				result.Code += code;
				result.Code += CreateTriggerMethodClosing() + "\n";
			}

			// Close class and namespace
			result.Code += CreateClassAndNamespaceClosing();

			return result;
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
					result.ScriptInfo[script] = new ScriptCodeGenerationInfo() {
						MethodName = script.ID,
						Offset = result.Code.Length,
					};

					if (!script.HasErrors || includeErrors)
						result.Code += script.Code;
					result.Code += CreateScriptMethodClosing(script) + "\n";
				}
			}

			// Generate trigger script methods
			int index = 0;
			foreach (ITriggerObject triggerObject in world.GetEventObjects()) {
				foreach (Trigger trigger in triggerObject.Triggers) {
					if (trigger.Script != null) {
						result.Code += CreateTriggerMethodOpening(trigger, index);
						result.ScriptInfo[trigger.Script] = new ScriptCodeGenerationInfo() {
							MethodName = trigger.Script.ID,
							Offset = result.Code.Length,
						};

						if (!trigger.Script.HasErrors || includeErrors)
							result.Code += trigger.Script.Code;
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
		// Static Methods
		//-----------------------------------------------------------------------------

		// Code Generation ------------------------------------------------------------

		/// <summary>Creates a name for an internal script.</summary>
		//private static string CreateInternalScriptName(int internalID) {
		//	return "__internal_script_" + internalID + "__";
		//}

		/// <summary>Creates the default usings.</summary>
		public static string CreateUsingsString() {
			return	"using System.Collections.Generic;\n" +
					"using Console = System.Console;\n" +
					"using ZeldaAPI;\n" +
					"using ZeldaOracle.Game;\n" +
					"using ZeldaOracle.Game.API;\n" +
					"using ZeldaOracle.Common.Geometry;\n";
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
		
		private static string CreateScriptMethodOpening(Script script) {
			return  "        /// <summary>" + script.Description.Replace('\n', ' ') + "</summary>\n" +
					"        public void " + script.ID + "(" + CreateParametersString(script.Parameters) + ") {\n";
		}
		
		private static string CreateScriptMethodClosing(Script script) {
			return	"\n        }\n";
		}

		private static string CreateTriggerMethodOpening(Trigger trigger, int triggerIndex) {
			ITriggerObject owner = trigger.Collection.TriggerObject;
			string methodName = CreateTriggerMethodName(trigger, triggerIndex);
			Type ownerType = typeof(ZeldaAPI.Entity);
			return "        public void " + methodName + "(" + ownerType.Name + " This) {\n";
		}

		private static string CreateTriggerMethodName(Trigger trigger, int triggerIndex) {
			return string.Format("trigger_{0}", triggerIndex);
		}

		private static string CreateTriggerMethodClosing() {
			return "\n        }\n";
		}
		
		/// <summary>Lists the parameters in a string for placing in a function declaration.</summary>
		private static string CreateParametersString(List<ScriptParameter> scriptParameters) {
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

		public World World {
			get { return world; }
			set { world = value; }
		}
	}
}
