using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
		
	public class ScriptParameter {
		public string Type { get; set; }
		public string Name { get; set; }
		
		public ScriptParameter() {
			Type = "";
			Name = "";
		}
		
		public ScriptParameter(string type, string name) {
			this.Type = type;
			this.Name = name;
		}

		public ScriptParameter(ScriptParameter copy) {
			this.Type = copy.Type;
			this.Name = copy.Name;
		}
	}

	public class ObjectEvent {
		private string name;
		private string readableName;
		private string description;
		private List<ScriptParameter> parameters;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectEvent(string name, string readableName, string description, ScriptParameter[] parameters) {
			this.name			= name;
			this.readableName	= readableName;
			this.description	= description;
			this.parameters		= new List<ScriptParameter>();
			this.parameters.AddRange(parameters);
		}
		
		public ObjectEvent(ObjectEvent copy) {
			this.name			= copy.name;
			this.readableName	= copy.readableName;
			this.description	= copy.description;
			this.parameters		= new List<ScriptParameter>();
			for (int i = 0; i < copy.parameters.Count; i++)
				this.parameters.Add(new ScriptParameter(copy.parameters[i]));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string ReadableName {
			get { return readableName; }
			set { readableName = value; }
		}
		
		public string Description {
			get { return description; }
			set { description = value; }
		}

		public List<ScriptParameter> Parameters {
			get { return parameters; }
			set { parameters = value; }
		}
	}
}
