using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A single parameter in a script event.</summary>
	[Serializable]
	public class ScriptParameter {
		/// <summary>The type name of the parameter.</summary>
		public string Type { get; set; }
		/// <summary>The variable name of the parameter.</summary>
		public string Name { get; set; }

		/// <summary>Constructs an empty script parameter.</summary>
		public ScriptParameter() {
			Type = "";
			Name = "";
		}

		/// <summary>Constructs a script parameter with the specified type and name.</summary>
		public ScriptParameter(string type, string name) {
			this.Type = type;
			this.Name = name;
		}

		/// <summary>Constructs a script parameter with the specified type and name.</summary>
		public ScriptParameter(Type type, string name) {
			if (!Assemblies.Scripting.Contains(type.Assembly))
				throw new ArgumentException("Script parameter type '" + type.Name +
					"' does not come from an assembly referenced by scripts!");
			this.Type = type.Name;
			this.Name = name;
		}
	}

	/// <summary>Documentation on an event as well as its parameters.</summary>
	public class EventDocumentation {
		/// <summary>The internal name of the event.</summary>
		private string name;
		/// <summary>The readable name of the event.</summary>
		private string readableName;
		/// <summary>The category of the event.</summary>
		private string category;
		/// <summary>The description of the event.</summary>
		private string description;
		/// <summary>The parameters for the event.</summary>
		private List<ScriptParameter> parameters;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the event documentation.</summary>
		public EventDocumentation(string name, string readableName, string category, string description, params ScriptParameter[] parameters) {
			this.name           = name;
			this.readableName   = readableName;
			this.category       = category;
			this.description    = description;
			this.parameters     = new List<ScriptParameter>(parameters);
		}

		/// <summary>Constructs the event documentation.</summary>
		public EventDocumentation(string name, params ScriptParameter[] parameters) :
			this(name, "", "", "Misc", parameters) {
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the internal name of the event.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets the readable name of the event.</summary>
		public string ReadableName {
			get { return readableName; }
		}

		/// <summary>Gets the readable name of the event or just the name if none is defined.</summary>
		public string FinalReadableName {
			get { return (string.IsNullOrWhiteSpace(readableName) ? name : readableName); }
		}

		/// <summary>Gets the category of the event.</summary>
		public string Category {
			get { return category; }
		}

		/// <summary>Gets the description of the event.</summary>
		public string Description {
			get { return description; }
		}

		/// <summary>Gets the parameters for the event.</summary>
		public List<ScriptParameter> Parameters {
			get { return parameters; }
		}

		/// <summary>Gets the number of parameters for the event.</summary>
		public int ParameterCount {
			get { return parameters.Count; }
		}
	}
}
