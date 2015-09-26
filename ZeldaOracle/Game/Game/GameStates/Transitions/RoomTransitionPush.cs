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

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransitionPush(RoomControl roomOld, RoomControl roomNew, int direction) :
			base(roomOld, roomNew)
		{
			this.direction = direction;
		}
		

		//-----------------------------------------------------------------------------
		// Internal drawing
		//-----------------------------------------------------------------------------

		private void DrawRooms(Graphics2D g) {
			Point2I panNew = -(Directions.ToPoint(direction) * distance);
			Point2I panOld = Directions.ToPoint(direction) * GameSettings.VIEW_SIZE;

			// Draw the old room.
			g.ResetTranslation();
			g.Translate(panNew);
			roomOld.Draw(g);
			
			// Draw the new room.
			g.Translate(panNew + panOld);
			roomNew.Draw(g);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			timer		= 0;
			distance	= 0;
			maxDistance = GameSettings.VIEW_SIZE[direction % 2];
			playerSpeed	= TRANSITION_PLAYER_HSPEED;
			if (Directions.IsVertical(direction))
				playerSpeed = TRANSITION_PLAYER_VSPEED;
		}

		public override void Update() {
			timer++;

			// Update screen panning.
			if (timer > TRANSITION_DELAY) {
				distance += TRANSITION_SPEED;
				player.Position += (Vector2F) Directions.ToPoint(direction) * playerSpeed;

				// Check if we are done panning.
				if (distance >= maxDistance) {
					player.RoomEnterPosition = player.Position;
					gameManager.PopGameState();
					gameManager.PushGameState(roomNew);
					player.OnEnterRoom();
					roomOld.DestroyRoom();
				}
			}
		}

		public override void Draw(Graphics2D g) {
			Zone zoneOld = roomOld.Room.Zone;
			Zone zoneNew = roomNew.Room.Zone;

			if (zoneOld == zoneNew) {
				// Draw the rooms normally.
				DrawRooms(g);
			}
			else {
				// Fade between zones.

				// Switch to the temp render target to draw the new zone.
				g.End();
				g.SetRenderTarget(GameData.RenderTargetGameTemp);
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				roomOld.Room.Zone = zoneNew;
				DrawRooms(g);
				roomOld.Room.Zone = zoneOld;

				// Switch to main render target to draw the old zone.
				g.End();
				g.SetRenderTarget(GameData.RenderTargetGame);
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				roomNew.Room.Zone = zoneOld;
				DrawRooms(g);
				roomNew.Room.Zone = zoneNew;

				// Draw the temp render target (with the new zone) at an opacity.
				float opacity = (float) distance / (float) maxDistance;
				Color color = Color.White * opacity;
				g.DrawImage(GameData.RenderTargetGameTemp, Vector2F.Zero, Vector2F.Zero, Vector2F.One, 0.0, color);
			}
		}

	}
}
