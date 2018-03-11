using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public class ScriptFunctions : ScriptInterface, ZeldaAPI.ScriptFunctions {

		public ZeldaAPI.ScriptFunctionsUnit		Unit { get; set; }
		public ZeldaAPI.ScriptFunctionsEntity	Entity { get; set; }
		public ZeldaAPI.ScriptFunctionsTile		Tile { get; set; }
		public ZeldaAPI.ScriptFunctionsSound	Sound { get; set; }
		public ZeldaAPI.ScriptFunctionsMusic	Music { get; set; }

		public ScriptFunctions(ScriptInstance script) : base(script) {
			Unit	= CreateSection(new ScriptFunctionsUnit());
			Entity	= CreateSection(new ScriptFunctionsEntity());
			Tile	= CreateSection(new ScriptFunctionsTile());
			Sound	= CreateSection(new ScriptFunctionsSound());
			Music	= CreateSection(new ScriptFunctionsMusic());
		}
	}

	public class ScriptFunctionsUnit :
		ScriptInterfaceSection, ZeldaAPI.ScriptFunctionsUnit
	{
		public ZeldaAPI.Unit UnitByID(string id) {
			return RoomControl.EntityManager.FindEntityByID(id) as ZeldaAPI.Unit;
		}
	}

	public class ScriptFunctionsEntity :
		ScriptInterfaceSection, ZeldaAPI.ScriptFunctionsEntity
	{
		public ZeldaAPI.Entity EntityByID(string id) {
			return RoomControl.EntityManager.FindEntityByID(id);
		}
	}

	public class ScriptFunctionsTile :
		ScriptInterfaceSection, ZeldaAPI.ScriptFunctionsTile
	{
	}

	public class ScriptFunctionsSound :
		ScriptInterfaceSection, ZeldaAPI.ScriptFunctionsSound
	{
		public ZeldaAPI.Sound SoundByID(string id) {
			return Resources.GetSound(id);
		}
	}

	public class ScriptFunctionsMusic :
		ScriptInterfaceSection, ZeldaAPI.ScriptFunctionsMusic
	{
		public ZeldaAPI.Music MusicByID(string id) {
			return Resources.GetSong(id);
		}
	}
}
