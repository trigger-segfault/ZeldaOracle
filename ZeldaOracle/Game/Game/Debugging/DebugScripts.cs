using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Debugging {

	public class DebugScripts : ZeldaAPI.CustomScriptBase {

		public void DebugScript1() {
			Actions.General.BeginCutscene();
			Actions.Unit.MakeUnitFaceDirection(player, Direction.Down);
			Actions.General.Wait(60);
			Actions.Camera.SetCameraTargetToPoint(new Vector2F());
			Actions.General.Wait(60);
			Actions.Camera.ResetCamera();
			Actions.General.Wait(60);
			Actions.General.EndCutscene();
		}

	}
}
