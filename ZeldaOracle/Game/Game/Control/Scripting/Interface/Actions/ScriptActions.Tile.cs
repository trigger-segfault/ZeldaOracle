using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Control.Scripting.Interface.Actions {

	public class ScriptActionsTile :
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsTile
	{

		public void LightLantern(ZeldaAPI.Lantern lantern) {
			lantern.Light();
		}
		
		public void PutOutLantern(ZeldaAPI.Lantern lantern) {
			lantern.PutOut();
		}
		
		public void FlipSwitch(ZeldaAPI.Lever lever) {
			lever.Flip();
		}
		
		public void SetColor(ZeldaAPI.ColorTile tile, PuzzleColor color) {
			tile.Color = color;
		}
		
		public void Rotate(ZeldaAPI.ColorTile tile) {
		}
		
		public void BuildBridge(ZeldaAPI.Bridge bridge) {
			bridge.BuildBridge();
		}
		
		public void DestroyBridge(ZeldaAPI.Bridge bridge) {
			bridge.DestroyBridge();
		}
	}
}
