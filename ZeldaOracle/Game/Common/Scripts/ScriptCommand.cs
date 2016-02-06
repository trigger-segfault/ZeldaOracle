using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {
	
	public class ScriptCommand {

		private string name;
		private Action<CommandParam> action;
		private List<CommandReferenceParam> parameterOverloads;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptCommand(string name, Action<CommandParam> action) {
			this.name		= name;
			this.action		= action;
			this.parameterOverloads	= new List<CommandReferenceParam>();
		}

		public ScriptCommand(string name, string[] parameterOverloads, Action<CommandParam> action) :
			this(name, action)
		{
			for (int i = 0; i < parameterOverloads.Length; i++) {
				CommandReferenceParam p =  CommandParamParser.ParseReferenceParams(parameterOverloads[i]);
				p.Name = parameterOverloads[i];
				this.parameterOverloads.Add(p);
			}
		}

		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Returns true if the command has the given name, ignoring case.
		public bool HasName(string commandName) {
			return (String.Compare(commandName, name, StringComparison.OrdinalIgnoreCase) == 0);
		}
		
		// Returns true if the given user passed-in parameters matches this
		// command's format, outputting new parameters specified in that format.
		public bool HasParameters(CommandParam userParameters, out CommandParam newParameters) {
			if (parameterOverloads.Count == 0) {
				newParameters = new CommandParam(userParameters);
				return true;
			}
			for (int i = 0; i < parameterOverloads.Count; i++) {
				if (AreParametersMatching(parameterOverloads[i], userParameters, out newParameters))
					return true;
			}
			newParameters = null;
			return false;
		}
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Recursively check whether the parameters are matching, outputting the new parameters.
		private static bool AreParametersMatching(CommandReferenceParam reference, CommandParam userParams, out CommandParam newParameters) {
			newParameters = null;

			if (reference == null) {
				newParameters = new CommandParam(userParams);
				return true;
			}
			if (!userParams.IsValidType(reference.Type)) {
				newParameters = null;
				return false;
			}

			// Make sure arrays are matching.
			if (reference.Type == CommandParamType.Array) {
				newParameters = new CommandParam(CommandParamType.Array);
				newParameters.Name = reference.Name;

				// Find the child index of the first parameter with a default value.
				int defaultIndex = 0;
				for (CommandReferenceParam p = reference.Children; p != null; p = p.NextParam) {
					if (p.DefaultValue != null)
						break;
					defaultIndex++;
				}

				// Verify the user parameter's child count is within the valid range.
				if (userParams.ChildCount < defaultIndex || userParams.ChildCount > reference.ChildCount) {
					newParameters = null;
					return false;
				}

				// Verify each child paremeter matches the reference.
				CommandReferenceParam referenceChild = reference.Children;
				CommandParam userChild = userParams.Children;
				for (int i = 0; i < reference.ChildCount; i++) {
					CommandParam newChild;
					if (i < userParams.ChildCount) {
						if (!AreParametersMatching(referenceChild, userChild, out newChild)) {
							newParameters = null;
							return false;
						}
						userChild = userChild.NextParam;
					}
					else {
						newChild = new CommandParam(referenceChild.DefaultValue);
					}
					newParameters.AddChild(newChild);
					referenceChild = referenceChild.NextParam;
				}
			}
			else {
				newParameters = new CommandParam(reference);
				newParameters.SetValueByParse(userParams.StringValue);
			}

			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
		}

		public Action<CommandParam> Action {
			get { return action; }
		}

		public List<CommandReferenceParam> ParameterOverloads {
			get { return parameterOverloads; }
		}
	}
}
