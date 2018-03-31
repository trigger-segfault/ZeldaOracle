using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Conscripts.Commands {
	
	public class ScriptCommand {

		private string[] names;
		private Action<CommandParam> action;
		private List<CommandReferenceParam> parameterOverloads;
		private int[] modes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptCommand(string name, int[] modes, Action<CommandParam> action) {
			this.names		= name.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			this.action		= action;
			this.parameterOverloads	= new List<CommandReferenceParam>();
			this.modes      = modes;
		}

		public ScriptCommand(string name, int[] modes, string[] parameterOverloads, Action<CommandParam> action,
			CommandParamDefinitions typeDefinitions) :
			this(name, modes, action)
		{
			for (int i = 0; i < parameterOverloads.Length; i++) {
				CommandReferenceParam p =  CommandParamParser.ParseReferenceParams(parameterOverloads[i], typeDefinitions);
				p.Name = parameterOverloads[i];
				this.parameterOverloads.Add(p);
			}
		}

		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Returns true if the command has the given name, ignoring case.
		public bool HasName(string commandName, CommandParam parameters) {
			if (string.Compare(commandName, names[0], StringComparison.OrdinalIgnoreCase) == 0 &&
				parameters.ChildCount + 1 >= names.Length)
			{
				CommandParam child = parameters.Children;
				for (int i = 1; i < names.Length; i++) {
					if (string.Compare(child.StringValue, names[i], StringComparison.OrdinalIgnoreCase) != 0)
						return false;
					child = child.NextParam;
				}
				return true;
			}
			return false;
		}
		
		// Returns true if the given user passed-in parameters matches this
		// command's format, outputting new parameters specified in that format.
		public bool HasParameters(CommandParam userParameters, out CommandParam newParameters, CommandParamDefinitions typeDefinitions) {
			if (parameterOverloads.Count == 0) {
				newParameters = new CommandParam(userParameters);
				return true;
			}
			var originalChildren = userParameters.Children;
			int originalChildCount = userParameters.ChildCount;
			if (names.Length > 1) {
				userParameters.Children = userParameters.GetParam(names.Length - 1);
				userParameters.ChildCount -= names.Length - 1;
			}
			for (int i = 0; i < parameterOverloads.Count; i++) {
				// Strip the parameters that are part of the command name
				if (AreParametersMatching(parameterOverloads[i], userParameters, out newParameters, typeDefinitions))
					return true;
			}
			userParameters.Children = originalChildren;
			userParameters.ChildCount = originalChildCount;
			newParameters = null;
			return false;
		}
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Recursively check whether the parameters are matching, outputting the new parameters.
		public static bool AreParametersMatching(CommandReferenceParam reference, CommandParam userParams, out CommandParam newParameters, CommandParamDefinitions typeDefinitions) {
			newParameters = null;

			if (reference == null) {
				newParameters = new CommandParam(userParams);
				return true;
			}

			if (reference.HasCustomType) {
				CommandParamDefinition def = typeDefinitions.Get(reference.CustomTypeName);
				for (int i = 0; i < def.ParameterOverloads.Count; i++) {
					if (AreParametersMatching(def.ParameterOverloads[i].Children, userParams, out newParameters, typeDefinitions))
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
					count = GMath.Max(count, userParams.ChildCount);
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
								if (!AreParametersMatching(referenceChild, namedParam, out newChild, typeDefinitions)) {
									newParameters = null;
									return false;
								}
								namedParamsUsed++;
							}
							namedParamMode = true;
						}
						else {
							if (!AreParametersMatching(referenceChild, userChild, out newChild, typeDefinitions)) {
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

		public string[] Names {
			get { return names; }
		}
		public string FullName {
			get { return string.Join(" ", names); }
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
