using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransitionPush : RoomTransition {
		private const int	TRANSITION_DELAY			= 8;		// Ticks
		private const int	TRANSITION_SPEED			= 4;		// Pixels per tick
		private const float	TRANSITION_PLAYER_HSPEED	= 0.38f;	// Pixels per tick
		private const float	TRANSITION_PLAYER_VSPEED	= 0.5f;		// Pixels per tick
		
		private int timer;
		private int distance;
		private int direction;
		private int maxDistance;
		private float playerSpeed;
		private bool isWaitingForView;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public RoomTransitionPush(int direction) {
			this.direction = direction;
		}
		

		//-----------------------------------------------------------------------------
		// Internal drawing
		//-----------------------------------------------------------------------------

		private void DrawRooms(Graphics2D g) {
			// Determine room draw positions.
			Point2I panOld = Directions.ToPoint(direction) * (-distance);
			Point2I panNew = Directions.ToPoint(direction) * (GameSettings.VIEW_SIZE - distance);

			// Draw the old and new rooms.
			OldRoomControl.DrawRoom(g, new Vector2F(0, 16) + panOld);
			NewRoomControl.DrawRoom(g, new Vector2F(0, 16) + panNew);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			if (IsChangingPalette) {
				GameData.PaletteShader.LerpTilePalette = NewRoomControl.Zone.Palette;
			}

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

			// Wait for the view to pan to the player.
			if (isWaitingForView) {
				OldRoomControl.ViewControl.PanTo(
					Player.Center + Player.ViewFocusOffset);

				if (OldRoomControl.ViewControl.IsCenteredOnPosition(
					Player.Center + Player.ViewFocusOffset))
				{
					// Convert the player's position from the old room to the
					// new room.
					Vector2F playerPosInNewRoom = Player.Position -
						(Directions.ToPoint(direction) *
						NewRoomControl.RoomBounds.Size);

					// Setup the new room while pretending the player is in his
					// final position after transitioning.
					Vector2F totalMovement = Directions.ToVector(direction) *
						(playerSpeed * ((float) maxDistance / (float) TRANSITION_SPEED));
					Player.Position = playerPosInNewRoom + totalMovement;
					SetupNewRoom();

					// Move the player back a bit so we can smoothly transition
					// them between the room border.
					Player.Position -= totalMovement;
					isWaitingForView = false;
					return;
				}
				return;
			}

			// Update HUD.
			GameControl.HUD.Update();

			// Update screen panning.
			if (timer > TRANSITION_DELAY) {
				distance += TRANSITION_SPEED;
				Player.Position += (Vector2F) Directions.ToPoint(direction) * playerSpeed;

				if (IsChangingPalette)
					GameData.PaletteShader.TileRatio = (float) distance / maxDistance;

				// Check if we are done panning.
				if (distance >= maxDistance) {
					DestroyOldRoom();
					EndTransition();
					NewRoomControl.DisableVisualEffect = false;
				}
			}
		}

		public override void Draw(Graphics2D g) {
			Zone zoneOld = OldRoomControl.Room.Zone;
			Zone zoneNew = NewRoomControl.Room.Zone;

			if (zoneOld == zoneNew) {
				// Draw the rooms normally.
				DrawRooms(g);
			}
			else {
				// Fade between different zones.

				// Switch to the temp render target to draw the new zone.
				g.End();
				g.SetRenderTarget(GameData.RenderTargetGameTemp);
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				OldRoomControl.Room.Zone = zoneNew;
				DrawRooms(g);
				OldRoomControl.Room.Zone = zoneOld;

				// Switch to main render target to draw the old zone.
				g.End();
				g.SetRenderTarget(GameData.RenderTargetGame);
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				NewRoomControl.Room.Zone = zoneOld;
				DrawRooms(g);
				NewRoomControl.Room.Zone = zoneNew;

				// Draw the temp render target (with the new zone) at an opacity.
				float opacity = (float) distance / (float) maxDistance;
				Color color = Color.White * opacity;
				g.DrawImage(GameData.RenderTargetGameTemp, Vector2F.Zero,
					Vector2F.Zero, Vector2F.One, 0.0, color);
			}
			
			// Draw the HUD.
			GameControl.HUD.Draw(g, false);
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		private bool IsChangingPalette {
			get { return OldRoomControl.Zone.PaletteID != NewRoomControl.Zone.PaletteID; }
		}
	}
}
