
namespace ZeldaAPI {
	/// <summary>The base class that scripts are defined in.</summary>
	public class CustomScriptBase {
		/// <summary>Access to the current room.</summary>
		public Room room;
		/// <summary>Access to the area.</summary>
		public Area area;
		/// <summary>Access to the game.</summary>
		public Game game;
		/// <summary>Contains the possible script actions to perform.</summary>
		public ScriptActions Actions;
		/// <summary>Contains the possible script functions to call.</summary>
		public ScriptFunctions Functions;
		/// <summary>The player entity.</summary>
		public Player player;
	}
}
