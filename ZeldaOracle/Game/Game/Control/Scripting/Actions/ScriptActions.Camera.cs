
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public class ScriptActionsCamera :
		ScriptActionsSection, ZeldaAPI.ScriptActionsCamera
	{

		public void LockCameraTargetToEntity(ZeldaAPI.Entity entity) {
			RoomControl.ViewControl.SetTarget((Entity) entity);
		}

		public void LockCameraTargetToPoint(Vector2F point) {
			RoomControl.ViewControl.SetTarget(point);
		}

		public void ResetCamera() {
			//RoomControl.ViewControl
		}

		public void PanCamera(Vector2F point) {

			//ScriptInstance.PerformUpdate(delegate() {



			//	return true;
			//});
		}
	}
}
