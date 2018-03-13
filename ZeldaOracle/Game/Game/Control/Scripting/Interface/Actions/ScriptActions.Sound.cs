using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Control.Scripting.Interface.Actions {

	public class ScriptActionsSound :
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsSound
	{
		public void PlaySound(ZeldaAPI.Sound sound) {
			AudioSystem.PlaySound((Sound) sound);
		}

		public void PlayMusic(ZeldaAPI.Music music) {
			AudioSystem.PlaySong((Song) music);
		}
		
		public void StopMusic() {
			AudioSystem.StopMusic();
		}
	}
}
