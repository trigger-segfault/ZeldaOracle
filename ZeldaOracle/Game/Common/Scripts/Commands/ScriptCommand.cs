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
			this.name       = name.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
			this.action		= action;
			this.parameterOverloads	= new List<CommandReferenceParam>();
			this.modes      = modes;
		}

		public ScriptCommand(string name, int[] modes, string[] parameterOverloads, Action<CommandParam> action,
			CommandParamDefinitions typeDefinitions) :
			this(name, modes, action)
		{
			string frontConstsStr = "";
			string[] frontConsts = name.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 1; i < frontConsts.Length; i++) {
				if (i > 1)
					frontConstsStr += ", ";
				frontConstsStr += "const " + frontConsts[i];
			}
			for (int i = 0; i < parameterOverloads.Length; i++) {
				string front = frontConstsStr;
				if (!string.IsNullOrEmpty(frontConstsStr) && !string.IsNullOrWhiteSpace(parameterOverloads[i]))
					front += ", ";
				CommandReferenceParam p =  CommandParamParser.ParseReferenceParams(front + parameterOverloads[i], typeDefinitions);
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
		public bool HasParameters(CommandParam userParameters, out CommandParam newParameters, CommandParamDefinitions defs) {
			if (parameterOverloads.Count == 0) {
				newParameters = new CommandParam(userParameters);
				return true;
			}
			for (int i = 0; i < parameterOverloads.Count; i++) {
				if (AreParametersMatching(parameterOverloads[i], userParameters, out newParameters, defs))
					return true;
			}
			newParameters = null;
			return false;
		}
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Recursively check whether the parameters are matching, outputting the new parameters.
		public static bool AreParametersMatching(CommandReferenceParam reference, CommandParam userParams, out CommandParam newParameters, CommandParamDefinitions defs) {
			newParameters = null;

			if (reference == null) {
				newParameters = new CommandParam(userParams);
				return true;
			}

			if (reference.HasCustomType) {
				CommandParamDefinition def = defs.Get(reference.CustomTypeName);
				for (int i = 0; i < def.ParameterOverloads.Count; i++) {
					if (AreParametersMatching(def.ParameterOverloads[i].Children, userParams, out newParameters, defs))
						return true;
				}
			}
			else if (!userParams.IsValidType(reference.Type)) {
				newParameters = null;
				return false;
			}
			if (reference.Type == CommandParamType.Const && string.Compare(reference.Name, userParams.StringValue, true) != 0) {
				return false;
			}

			// Make sure arrays are matching.
			if (reference.Type == CommandParamType.Array) {
				// If this value is not equal to the number of named
				// params specified then the parameters do not match.
				// All parameters after the first named parameter must
				// be named.
				int namedParamsUsed = 0;
				bool namedParamMode = false;

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
				if (lastRefChild != null && lastRefChild.IsVariadic) {
					isVariadic = true;

					// Named parameters cannot be used in variadic functions
					if (userParams.HasNamedParams)
						return false;
				}

				// Verify the user parameter's child count is within the valid range.
				if (userParams.ChildCount < defaultIndex ||
					(userParams.ChildCount > reference.ChildCount && !isVariadic))
				{
					newParameters = null;
					return false;
				}

				// Verify each child parameter matches the reference.
				CommandReferenceParam referenceChild = reference.Children;
				CommandParam userChild = userParams.Children;
				int count = reference.ChildCount;
				if (isVariadic)
					count = Math.Max(count, userParams.ChildCount);
				for (int i = 0; i < count; i++) {
					CommandParam newChild;
					if (i < userParams.ChildCount || namedParamMode) {
						// Is this a named parameter
						// Named params can only be used on parameters with default values.
						if (namedParamMode || !string.IsNullOrEmpty(userChild.Name)) {
							CommandParam namedParam = userParams.GetNamedParam(referenceChild.Name);
							if (namedParam == null) {
								if (referenceChild.DefaultValue == null)
									return false;
								newChild = new CommandParam(referenceChild.DefaultValue);
							}
							else {
								if (!AreParametersMatching(referenceChild, namedParam, out newChild, defs)) {
									newParameters = null;
									return false;
								}
								namedParamsUsed++;
							}
							namedParamMode = true;
						}
						else {
							if (!AreParametersMatching(referenceChild, userChild, out newChild, defs)) {
								newParameters = null;
								return false;
							}
						}

						if (!namedParamMode)
							userChild = userChild.NextParam;
					}
					else {
						newChild = new CommandParam(referenceChild.DefaultValue);
					}
					newParameters.AddChild(newChild);
					if (referenceChild.NextParam != null)
						referenceChild = referenceChild.NextParam;
				}

				// Make sure all of the named parameters were used
				if (userParams.NamedChildCount != namedParamsUsed)
					return false;
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
