using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripts.Commands {
	public class CommandParamDefinition {

		private string name;
		private List<CommandReferenceParam> parameterOverloads;

		public CommandParamDefinition(string name, string[] parameterOverloads, CommandParamDefinitions typeDefinitions) {
			this.name = name;
			this.parameterOverloads = new List<CommandReferenceParam>();
			for (int i = 0; i < parameterOverloads.Length; i++) {
				CommandReferenceParam p =  CommandParamParser.ParseReferenceParams(parameterOverloads[i], typeDefinitions);
				p.Name = parameterOverloads[i];
				this.parameterOverloads.Add(p);
				if (p.ChildCount != 1)
					throw new LoadContentException("Type definition must have 1 parameter or an array!");
			}
		}

		public string Name {
			get { return name; }
		}

		public List<CommandReferenceParam> ParameterOverloads {
			get { return parameterOverloads; }
		}
	}

	public class CommandParamDefinitions {

		private Dictionary<string, CommandParamDefinition> definitions;

		public CommandParamDefinitions() {
			definitions = new Dictionary<string, CommandParamDefinition>(StringComparer.OrdinalIgnoreCase);
		}

		public CommandParamDefinition Get(string name) {
			CommandParamDefinition def;
			definitions.TryGetValue(name, out def);
			return def;
		}

		public bool Contains(string name) {
			return definitions.ContainsKey(name);
		}

		public void Add(CommandParamDefinition definition) {
			definitions.Add(definition.Name, definition);
		}
	}
}
