using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	public enum EnterExitType {
		Enter,
		Exit,
	}

	public class RoomEnterExitState : GameState {

		private string enterMessage; // Message to display after entering.
		private float walkDistance; // Distance the player should walk.
		private int walkDirection;
		private float walkSpeed;
		private float distance;
		private EnterExitType type;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		private RoomEnterExitState(EnterExitType type, int walkDirection, float walkDistance, string enterMessage = null) :
			this(type, walkDirection, walkDistance, 0, enterMessage)
		{
		}

		private RoomEnterExitState(EnterExitType type, int walkDirection, float walkDistance, float walkSpeed, string enterMessage = null) {
			this.type			= type;
			this.enterMessage	= enterMessage;
			this.walkDirection	= walkDirection;
			this.walkDistance	= walkDistance;
			this.walkSpeed		= walkSpeed;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			distance = 0;
			Player player = GameControl.Player;
			player.Direction = walkDirection;

			if (type == EnterExitType.Enter) {
				Vector2F destination = player.Position +
					Directions.ToVector(walkDirection) * walkDistance;
				player.RequestSpawnNaturalState(destination, true);
			}

			// Keep directional animations
			if (player.StateParameters.AlwaysFaceUp) {
				player.Direction = Direction.Up;
			}
			else if (player.StateParameters.AlwaysFaceLeftOrRight) {
				if (player.Direction == Direction.Up)
					player.Direction = Direction.Right;
				if (player.Direction == Direction.Down)
					player.Direction = Direction.Left;
			}

			if (walkSpeed == 0)
				walkSpeed = player.Movement.BaseMovementSpeed;

			player.Graphics.PlayAnimation(player.MoveAnimation);
		}

		public override void OnEnd() {
		}

		public override void Update() {
			Player player = GameControl.Player;

			// Move the player
			distance += walkSpeed;
			player.Position += Directions.ToVector(walkDirection) * walkSpeed;
			player.UpdateGraphics();

			if (distance >= walkDistance) {
				player.Graphics.StopAnimation();
				End();
				//GameManager.PopGameState();
				if (enterMessage != null && type == EnterExitType.Enter)
					GameControl.DisplayMessage(enterMessage);
				if (type == EnterExitType.Enter)
					player.MarkRespawn();
			}
		}

		public override void Draw(Graphics2D g) {
			
		}


		//-----------------------------------------------------------------------------
		// Static Factory Methods
		//-----------------------------------------------------------------------------

		public static RoomEnterExitState CreateEnter(int walkDirection, float walkDistance, string enterMessage = null) {
			return new RoomEnterExitState(EnterExitType.Enter, walkDirection, walkDistance, enterMessage);
		}

		public static RoomEnterExitState CreateExit(int walkDirection, float walkDistance) {
			return new RoomEnterExitState(EnterExitType.Exit, walkDirection, walkDistance, null);
		}
	}
}
