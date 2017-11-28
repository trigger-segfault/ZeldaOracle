using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaOracle.Common.Scripting {
		
	public class Event {
		/// <summary>The documentation on the event.</summary>
		private EventDocumentation documentation;
		/// <summary>The defined script for the event. This is only used in the editor. In-game, internalID refers to the script.</summary>
		private Script script;
		/// <summary>The internal ID of the script that this event called. This value may change often when used in the editor.</summary>
		private string internalID;
		/// <summary>The event collection containing this event.</summary>
		private EventCollection events;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the event with no existing documentation.</summary>
		public Event(string name, params ScriptParameter[] parameters) {
			this.documentation  = new EventDocumentation(name, parameters);
			this.script         = null;
			this.internalID     = "";
		}

		/// <summary>Constructs the event with the existing documentation.</summary>
		public Event(EventDocumentation documentation) {
			this.documentation  = documentation;
			this.script			= null;
			this.internalID		= "";
		}

		/// <summary>Constructs a copy of the event.</summary>
		public Event(Event copy) {
			this.documentation  = copy.documentation;
			this.script			= (copy.Script != null ? new Script(copy.Script) : null);
			this.internalID		= "";
		}


		//-----------------------------------------------------------------------------
		// Scripting
		//-----------------------------------------------------------------------------

		/// <summary>Defines the script and adds comments for the parameters.</summary>
		public void DefineScript(string code = null) {
			script = new Script();
			script.ID = "__internal_script__";
			script.IsHidden = true;
			script.Parameters = Parameters;
			if (code != null) {
				script.Code = code;
			}
			else if (Parameters.Any()) {
				script.Code = "// Parameters = (";
				for (int i = 0; i < Parameters.Count; i++) {
					ScriptParameter parameter = Parameters[i];
					if (i > 0)
						script.Code += ", ";
					script.Code += parameter.Type + " " + parameter.Name;
				}
				script.Code += ")\r\n\r\n";
			}
		}

		/// <summary>Undefines the script and sets it back to null.</summary>
		public void UndefineScript() {
			script = null;
		}

		/// <summary>Checks if this event references a non-internal script. Returns the name of the script or null.</summary>
		public string GetExistingScript(IDictionary<string, Script> scripts) {
			if (script != null && script.Code.Length <= 53) {
				string code = script.Code.Trim(' ', '\t', '\n', '\r');
				if (code.EndsWith("();")) {
					string functionName = code.Substring(0, code.Length - 3);
					if (scripts.ContainsKey(functionName)) {
						return functionName;
					}
				}
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the internal name of the event.</summary>
		public string Name {
			get { return documentation.Name; }
		}

		/// <summary>Gets or sets the documentation for this event.</summary>
		public EventDocumentation Documentation {
			get { return documentation; }
			set { documentation = value; }
		}

		/// <summary>Gets or sets the event collection containing this event.</summary>
		public EventCollection Events {
			get { return events; }
			set { events = value; }
		}

		// Documentation --------------------------------------------------------------

		/// <summary>Gets the readable name of the event.</summary>
		public string ReadableName {
			get { return documentation.ReadableName; }
		}

		/// <summary>Gets the readable name of the event or just the name if none is defined.</summary>
		public string FinalReadableName {
			get { return documentation.FinalReadableName; }
		}

		/// <summary>Gets the category of the event.</summary>
		public string Category {
			get { return documentation.Category; }
		}

		/// <summary>Gets the description of the event.</summary>
		public string Description {
			get { return documentation.Description; }
		}

		/// <summary>The parameters for the event.</summary>
		public List<ScriptParameter> Parameters {
			get { return documentation.Parameters; }
		}

		/// <summary>Gets the number of parameters for the event.</summary>
		public int ParameterCount {
			get { return documentation.ParameterCount; }
		}
		
		// Scripting ------------------------------------------------------------------

		/// <summary>Returns true if the script is defined and non-null.</summary>
		public bool IsDefined {
			get { return script != null; }
		}

		/// <summary>Gets the script of the event.</summary>
		public Script Script {
			get { return script; }
			set { script = value; }
		}

		/// <summary>Gets the internal script ID of the event. This value is changed depending on where the event is used.</summary>
		public string InternalScriptID {
			get { return internalID; }
			set { internalID = value; }
		}
	}
}
