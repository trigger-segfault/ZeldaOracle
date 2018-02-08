using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public class RoomStateTurnstile : RoomState {

		private enum State {
			BeforeTurn,
			Turning,
			AfterTurn,
			Walking
		}

		private int timer;
		private TileTurnstile turnstile;
		private State state;
		private int enterDirection;
		private int exitDirection;
		private float walkDistance;


		//-----------------------------------------------------------------------------
		// Contsants
		//-----------------------------------------------------------------------------
		
		// Player offsets while turning clockwise from left to up:
		// * left 12
		// * left 10, up 2
		// * left 8, up 8
		// * left 2, up 10
		// * up 12
		// * up 27 (after walking)

		private const int		PLAYER_EXIT_WALK_DISTANCE		= 15;
		private const int		SCREEN_SHAKE_DURATION			= 4;
		private const int		SCREEN_SHAKE_MAGNITUDE			= 3;
		private const int		BEFORE_TURN_DELAY				= 15;
		private const int		AFTER_TURN_DELAY				= 7;
		private static float[]	PLAYER_ROTATE_OFFSETS			= { 12, 10, 8, 2, 0 };
		private const int		PLAYER_ROTATE_OFFSET_INTERVAL	= 4;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateTurnstile(TileTurnstile turnstile, int enterDirection)
		{
			this.enterDirection		= enterDirection;
			this.updateRoom			= false;
			this.animateRoom		= true;
			this.turnstile			= turnstile;
			this.timer				= 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer			= 0;
			state			= State.BeforeTurn;
			walkDistance	= 0.0f;
			exitDirection	= Directions.Add(enterDirection, 1, turnstile.WindingOrder);
			
			// Setup the player.
			Player player = RoomControl.Player;
			player.InterruptItems();
			player.SetPositionByCenter(turnstile.Center +
				(Directions.ToVector(enterDirection) * PLAYER_ROTATE_OFFSETS[0]));
			player.Direction = exitDirection;
			player.Graphics.PlayAnimation(player.MoveAnimation);
		}

		public override void Update() {
			Player player = RoomControl.Player;

			// Update turnstile states.
			if (state == State.BeforeTurn) {
				// Wait before turning, while shaking the screen.
				timer++;
				if (timer <= SCREEN_SHAKE_DURATION) {
					int mag = SCREEN_SHAKE_MAGNITUDE;
					RoomControl.ViewControl.ShakeOffset = new Vector2F(
						GRandom.NextInt(-mag, mag), GRandom.NextInt(-mag, mag));
				}
				if (timer >= BEFORE_TURN_DELAY) {
					timer = 0;
					state = State.Turning;
					turnstile.Rotate();
				}
			}
			else if (state == State.Turning) {
				// Move the player while turning.
				// Offset the player in the enter and exit directions separately.
				timer++;
				int enterOffsetIndex = (timer / PLAYER_ROTATE_OFFSET_INTERVAL) + 1;
				enterOffsetIndex = GMath.Clamp(enterOffsetIndex, 0, PLAYER_ROTATE_OFFSETS.Length - 1);
				int exitOffsetIndex = PLAYER_ROTATE_OFFSETS.Length - 1 - enterOffsetIndex;
				player.SetPositionByCenter(turnstile.Center);
				player.Position += PLAYER_ROTATE_OFFSETS[enterOffsetIndex] * Directions.ToVector(enterDirection);
				player.Position += PLAYER_ROTATE_OFFSETS[exitOffsetIndex]  * Directions.ToVector(exitDirection);

				if (turnstile.IsDoneTurning()) {
					state = State.AfterTurn;
					timer = 0;
				}
			}
			else if (state == State.AfterTurn) {
				// Wait before exiting the turnstile.
				timer++;
				if (timer >= AFTER_TURN_DELAY) {
					state = State.Walking;
					timer = 0;
					player.Graphics.PlayAnimation(player.MoveAnimation);
					player.SetPositionByCenter(turnstile.Center + (Directions.ToVector(exitDirection) * 12));
				}
			}
			else if (state == State.Walking) {
				// Player exits the turnstile.
				timer++;
				player.Graphics.AnimationPlayer.Update();
				player.Position += Directions.ToVector(exitDirection) * GameSettings.PLAYER_MOVE_SPEED;
				walkDistance += GameSettings.PLAYER_MOVE_SPEED;

				if (walkDistance >= PLAYER_EXIT_WALK_DISTANCE) {
					turnstile.ToggleDirection();
					player.LandOnSurface();
					gameControl.PopRoomState();
				}
			}
		}
	}
}
