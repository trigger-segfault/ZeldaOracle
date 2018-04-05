using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Conscripts.Commands {
	/// <summary>A definitions for a custom command parameter type.</summary>
	public class CommandParamDefinition {

		/// <summary>The name of the type.</summary>
		private string name;
		/// <summary>The different overloads available for the type.</summary>
		private List<CommandReferenceParam> parameterOverloads;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the command parameter type definition.</summary>
		public CommandParamDefinition(string name, string[] parameterOverloads,
			CommandParamDefinitions typeDefinitions)
		{
			this.name = name;
			this.parameterOverloads = new List<CommandReferenceParam>();

			// Parse and add all parameter overloads
			for (int i = 0; i < parameterOverloads.Length; i++) {
				// typeDefinitions is needed incase this type
				// uses existing types to define itself.
				CommandReferenceParam p =  CommandParamParser.ParseReferenceParams(
					parameterOverloads[i], typeDefinitions);
				p.Name = parameterOverloads[i];
				this.parameterOverloads.Add(p);
				if (p.ChildCount != 1)
					throw new LoadContentException("Type definition must have 1 " +
						"parameter or an array!");
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name of the type.</summary>
		public string Name {
			get { return name; }
		}

		/// <summary>Gets the different overloads available for the type.</summary>
		public List<CommandReferenceParam> ParameterOverloads {
			get { return parameterOverloads; }
		}
	}

	/// <summary>A collection of custom command parameter type definitions.</summary>
	public class CommandParamDefinitions {

		/// <summary>The collection of custom command parameter type definitions.</summary>
		private Dictionary<string, CommandParamDefinition> definitions;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the command parameter type definition collection.</summary>
		public CommandParamDefinitions() {
			definitions = new Dictionary<string, CommandParamDefinition>(
				StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>Constructs a copy of the command parameter type definition
		/// collection.</summary>
		public CommandParamDefinitions(CommandParamDefinitions defs) {
			definitions = new Dictionary<string, CommandParamDefinition>(
				defs.definitions, StringComparer.OrdinalIgnoreCase);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the parameter type with the specified name.</summary>
		public CommandParamDefinition Get(string name) {
			CommandParamDefinition def;
			definitions.TryGetValue(name, out def);
			return def;
		}

		/// <summary>Gets if this collection contains a parameter type with the
		/// specified name.</summary>
		public bool Contains(string name) {
			return definitions.ContainsKey(name);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds a parameter type to the collection.</summary>
		public void Add(CommandParamDefinition definition) {
			definitions.Add(definition.Name, definition);
		}
	}
}
