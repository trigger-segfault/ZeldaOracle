using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransitionPush : RoomTransition {
		private const int	TRANSITION_DELAY			= 8;		// Ticks
		private const int	TRANSITION_SPEED			= 4;		// Pixels per tick
		private const float	TRANSITION_PLAYER_HSPEED	= 0.38f;	// Pixels per tick
		private const float	TRANSITION_PLAYER_VSPEED	= 0.5f;		// Pixels per tick
		private const int	TRANSITION_LERP_FRAMES		= 27;
		
		private int timer;
		private int distance;
		private int direction;
		private int maxDistance;
		private float playerSpeed;
		private bool isWaitingForView;
		private float lerpRatio;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public RoomTransitionPush(int direction) {
			this.direction = direction;
		}
		
		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			
			isWaitingForView	= true;
			timer				= 0;
			distance			= 0;
			maxDistance			= GameSettings.VIEW_SIZE[direction % 2];
			playerSpeed			= TRANSITION_PLAYER_HSPEED;

			// Because this transition uses render targets, we need to disable
			// visual room effects which would need to use the same render
			// targets.
			NewRoomControl.DisableVisualEffect = true;
			OldRoomControl.DisableVisualEffect = true;

			if (Directions.IsVertical(direction))
				playerSpeed = TRANSITION_PLAYER_VSPEED;
		}

		public override void Update() {
			timer++;

			// Wait for the view to pan to the player
			if (isWaitingForView) {
				if (OldRoomControl.ViewControl.IsCenteredOnTarget()) {
					// Convert the player's position from the old room to the
					// new room
					Vector2F playerPosInNewRoom = Player.Position -
						(Directions.ToPoint(direction) *
						NewRoomControl.RoomBounds.Size);

					// Setup the new room while pretending the player is in his
					// final position after transitioning
					Vector2F totalMovement = Directions.ToVector(direction) *
						(playerSpeed * ((float) maxDistance / TRANSITION_SPEED));
					Player.Position = playerPosInNewRoom + totalMovement;
					SetupNewRoom(false);

					// Move the player back a bit so we can smoothly transition
					// him between the room border
					Player.Position -= totalMovement;
					isWaitingForView = false;
					return;
				}
			}

			// Update HUD
			GameControl.HUD.Update();

			// Update screen panning
			if (timer > TRANSITION_DELAY) {
				distance += TRANSITION_SPEED;
				Player.Position += (Vector2F) Directions.ToPoint(direction) * playerSpeed;
				
				// Check if we are done panning
				if (distance >= maxDistance) {
					DestroyOldRoom();
					EndTransition();
					NewRoomControl.DisableVisualEffect = false;
				}
			}

			// Although it seems strange, the number of frames to lerp for is the
			// same for both horizontal and vertical transitions, even though the
			// total number of frames for each transition are 40 and 32 respectively.
			if (timer % 2 == 1) {
				lerpRatio = GMath.Min(1f, (float) (timer - TRANSITION_DELAY) /
					TRANSITION_LERP_FRAMES);
			}
		}

		public override void AssignPalettes() {
			OldRoomControl.AssignPalettes();
			NewRoomControl.AssignLerpPalettes();
			GameData.PaletteShader.TileRatio = lerpRatio;
			GameData.PaletteShader.EntityRatio = lerpRatio;
		}

		public override void Draw(Graphics2D g) {
			Zone zoneOld = OldRoomControl.Room.Zone;
			Zone zoneNew = NewRoomControl.Room.Zone;
			
			// Determine room draw positions.
			Point2I panOld = Directions.ToPoint(direction) * (-distance);
			Point2I panNew = Directions.ToPoint(direction) *
				(GameSettings.VIEW_SIZE - distance);

			// Draw the old and new rooms.
			OldRoomControl.DrawRoom(g, new Vector2F(0, 16) + panOld, RoomDrawing.DrawBelow);
			NewRoomControl.DrawRoom(g, new Vector2F(0, 16) + panNew, RoomDrawing.DrawAll);
			OldRoomControl.DrawRoom(g, new Vector2F(0, 16) + panOld, RoomDrawing.DrawAbove);

			// Draw the HUD.
			GameControl.HUD.Draw(g);
		}
		
	}
}
