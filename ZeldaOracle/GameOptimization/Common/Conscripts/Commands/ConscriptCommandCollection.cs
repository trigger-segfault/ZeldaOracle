using System.Collections.Generic;
using CommandLookup = System.Collections.Generic.Dictionary<
	string, System.Collections.Generic.List<
		ZeldaOracle.Common.Conscripts.Commands.ConscriptCommand>>;

namespace ZeldaOracle.Common.Conscripts.Commands {
	/// <summary>A collection of conscript commands with advanced lookup functionality.</summary>
	public class ConscriptCommandCollection {

		/// <summary>A list of all available commands.</summary>
		private List<ConscriptCommand> allCommands;
		/// <summary>All commands that can be called in any mode.</summary>
		private CommandLookup noModeCommands;
		/// <summary>All commands that can only be called from one or more modes.</summary>
		private Dictionary<int, CommandLookup> modeCommands;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the command collection.</summary>
		public ConscriptCommandCollection() {
			allCommands = new List<ConscriptCommand>();
			noModeCommands = new CommandLookup();
			modeCommands = new Dictionary<int, CommandLookup>();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Looks for all commands with the beginning name and mode.</summary>
		public IEnumerable<ConscriptCommand> Find(string name, int mode) {
			List<ConscriptCommand> commands;
			if (noModeCommands.TryGetValue(name, out commands)) {
				foreach (var command in commands)
					yield return command;
			}

			CommandLookup modeLookup;
			if (modeCommands.TryGetValue(mode, out modeLookup)) {
				if (modeLookup.TryGetValue(name, out commands)) {
					foreach (var command in commands)
						yield return command;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds a command to the mode.</summary>
		public void Add(ConscriptCommand command) {
			allCommands.Add(command);

			// Sort the command into the lookup tables
			string name = command.Names[0];
			if (command.Modes == null) {
				AddToLookup(command, noModeCommands);
			}
			else {
				foreach (int mode in command.Modes) {
					CommandLookup modeLookup;
					if (!modeCommands.TryGetValue(mode, out modeLookup)) {
						modeLookup = new CommandLookup();
						modeCommands.Add(mode, modeLookup);
					}
					AddToLookup(command, modeLookup);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Adds the command to the lookup list and creates the list if
		/// one does not already exist.</summary>
		private void AddToLookup(ConscriptCommand command, CommandLookup lookup) {
			string name = command.Names[0];
			List<ConscriptCommand> list;
			if (!lookup.TryGetValue(name, out list)) {
				list = new List<ConscriptCommand>();
				lookup.Add(name, list);
			}
			list.Add(command);
		}
	}
}
