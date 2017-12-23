using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts.Commands {
	
	public class ScriptCommand {

		private string name;
		private Action<CommandParam> action;
		private List<CommandReferenceParam> parameterOverloads;
		private int[] modes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptCommand(string name, int[] modes, Action<CommandParam> action) {
			this.name		= name;
			this.action		= action;
			this.parameterOverloads	= new List<CommandReferenceParam>();
			this.modes      = modes;
		}

		public ScriptCommand(string name, int[] modes, string[] parameterOverloads, Action<CommandParam> action) :
			this(name, modes, action)
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
			return (string.Compare(commandName, name, StringComparison.OrdinalIgnoreCase) == 0);
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

				// Check if the reference is variadic on the last parameter.
				CommandReferenceParam lastRefChild = reference.GetChildren().LastOrDefault();
				bool isVariadic = false;
				if (lastRefChild != null && lastRefChild.IsVariadic)
					isVariadic = true;

				// Verify the user parameter's child count is within the valid range.
				if (userParams.ChildCount < defaultIndex ||
					(userParams.ChildCount > reference.ChildCount && !isVariadic))
				{
					newParameters = null;
					return false;
				}

				// Verify each child paremeter matches the reference.
				CommandReferenceParam referenceChild = reference.Children;
				CommandParam userChild = userParams.Children;
				int count = reference.ChildCount;
				if (isVariadic)
					count = Math.Max(count, userParams.ChildCount);
				for (int i = 0; i < count; i++) {
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
					if (referenceChild.NextParam != null)
						referenceChild = referenceChild.NextParam;
				}
			}
			else {
				if (userParams.Type == CommandParamType.Array) {
					newParameters = new CommandParam(CommandParamType.Array);
					foreach (CommandParam userChild in userParams.GetChildren())
						newParameters.AddChild(new CommandParam(userChild));
				}
				else {
					newParameters = new CommandParam(reference);
					newParameters.SetValueByParse(userParams.StringValue);
				}
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

		public int[] Modes {
			get { return modes; }
		}
	}
}
