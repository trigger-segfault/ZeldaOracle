using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public class PlayerCutsceneState : PlayerState {
		public PlayerCutsceneState() {
			StateParameters.DisablePlayerControl = true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.StopPushing();
			player.Movement.StopMotion();
		}
	}

	public class ScriptActionsUnit :
		ScriptActionsSection, ZeldaAPI.ScriptActionsUnit
	{

		public void MakeUnitFaceDirection(ZeldaAPI.Unit unit, Direction direction) {
			LogMessage("Making unit '{0}' face {1}", unit, direction);
			((Unit) unit).Direction = direction;
		}

		public void BeginMovingInDirection(ZeldaAPI.Entity entity, Direction direction, float speed) {
		}

		public void Kill(ZeldaAPI.Unit unit) {
			((Unit) unit).Kill();
		}

		public void Destroy(ZeldaAPI.Unit unit) {
			((Unit) unit).Destroy();
		}

		public void Move(ZeldaAPI.Unit unit, Direction direction,
			Distance distance, float speed)
		{
			LogMessage(
				"Moving unit '{0}' {1} to the {2} with a speed of {3} pixels/tick",
				unit, distance.Pixels, direction, speed);

			float currentDistance = 0.0f;
			Vector2F move = direction.ToVector(speed);
			Unit actualUnit = (Unit) unit;

			actualUnit.Direction = direction;

			if (unit is Player) {
				Player player = (Player) unit;
				player.Graphics.PlayAnimation(player.Animations.Default);
			}

			ScriptInstance.PerformUpdate(delegate() {
				float amount = GMath.Min(speed, distance.Pixels - currentDistance);
				currentDistance += amount;
				actualUnit.Position += direction.ToVector(amount);
				return (currentDistance >= distance);
			});

			if (unit is Player) {
				Player player = (Player) unit;
				player.Graphics.StopAnimation();
			}
		}
	}
}
