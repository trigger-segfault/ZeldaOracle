using ZeldaOracle.Common.Audio;
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
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsUnit
	{

		public void MakeUnitFaceDirection(ZeldaAPI.Unit unit, Direction direction) {
			LogMessage("Making unit {0} face {1}", unit, direction);
			((Unit) unit).Direction = direction;
		}

		public void Kill(ZeldaAPI.Unit unit) {
			LogMessage("Killing unit {0}", unit);
			((Unit) unit).Kill();
		}

		public void Destroy(ZeldaAPI.Unit unit) {
			LogMessage("Destroying unit {0}", unit);
			((Unit) unit).Destroy();
		}

		public void Jump(ZeldaAPI.Unit unit, float jumpSpeed) {
			LogMessage("Making unit {0} jump with a speed of {1}", unit, jumpSpeed);
			((Unit) unit).Physics.ZVelocity = jumpSpeed;
		}

		public void MoveToPoint(ZeldaAPI.Unit unit, Vector2F point, float speed) {
			LogMessage(
				"Moving unit {0} to point {1} with a speed of {2} pixels/tick",
				unit, point, speed);
			Unit actualUnit = (Unit) unit;
			actualUnit.Direction = Direction.FromVector(point - actualUnit.Position);

			if (unit is Player) {
				Player player = (Player) unit;
				player.Graphics.PlayAnimation(player.Animations.Default);
			}

			ScriptInstance.PerformUpdate(delegate() {
				Vector2F vectorToPoint = point - actualUnit.Position;

				if (vectorToPoint.Length <= speed) {
					actualUnit.Position = vectorToPoint;
					return true;
				}
				else {
					actualUnit.Position += vectorToPoint.Normalized * speed;
					return false;
				}
			});

			if (unit is Player) {
				Player player = (Player) unit;
				player.Graphics.StopAnimation();
			}
		}

		public void Move(ZeldaAPI.Unit unit, Direction direction,
			Distance distance, float speed)
		{
			LogMessage(
				"Moving unit {0} {1} to the {2} with a speed of {3} pixels/tick",
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

		public void BeginMovingInDirection(ZeldaAPI.Entity entity, Direction direction, float speed) {
		}
	}
}
