using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Control {

	// UNUSED Class. I may use it at some point.

	public class RoomPhysics {

		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomPhysics(RoomControl roomControl) {
			this.roomControl = roomControl;
		}


		//-----------------------------------------------------------------------------
		// Physics Update
		//-----------------------------------------------------------------------------

		public void ProcessPhysics() {

			// Create list of collisions.
			for (int i = 0; i < roomControl.Entities.Count; i++) {
				Entity entity = roomControl.Entities[i];
				if (entity.Physics != null && entity.Physics.IsEnabled)
					ProcessEntity(entity);
			}
		}
		
		public void ProcessEntity(Entity entity) {
			// Reset collision states
			// Update Z dynamics
			// Collide with tiles & entities
			// Collide with room edge.
			// Integrate velocity.
			// Check ledges
			// Top tile - conveyor effect
			// Destroy outside of room
			// Hazard tiles
			// OnLand()

			UpdateEntityZPosition(entity);
			UpdateEntityTopTile(entity);
		}

		public void UpdateEntityTopTile(Entity entity) {
			Tile topTile = null;

			foreach (Tile tile in roomControl.GetTiles()) {
				Rectangle2F tileRect = new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
				if (!tile.IsSolid && tileRect.Contains(entity.Position) &&
					(topTile == null || tile.Layer > topTile.Layer))
				{
					topTile = tile;
				}
			}

			if (topTile != null) {
				// TODO: Integrate the surface tile's velocity into our
				// velocity rather than just moving position.
				entity.Position += topTile.Velocity;
				if (entity.Physics.MovesWithConveyors && entity.Physics.IsOnGround)
					entity.Position += topTile.ConveyorVelocity;
			}

			entity.Physics.TopTile = topTile;
		}

		public void UpdateEntityZPosition(Entity entity) {
			if (entity.ZPosition > 0.0f || entity.Physics.ZVelocity != 0.0f) {
				// Apply gravity.
				if (entity.Physics.HasFlags(PhysicsFlags.HasGravity)) {
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					if (entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed && entity.Physics.MaxFallSpeed >= 0)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
				}

				// Apply z-velocity.
				entity.ZPosition += entity.Physics.ZVelocity;

				// Check if landed on the ground.
				if (entity.ZPosition <= 0.0f) {
					//hasLanded = true;
					entity.ZPosition = 0.0f;

					if (entity.Physics.HasFlags(PhysicsFlags.Bounces)) {
						BounceEntity(entity);
					}
					else {
						entity.ZPosition = 0.0f;
						entity.Physics.ZVelocity = 0.0f;
					}
				}
			}
			else
				entity.Physics.ZVelocity = 0.0f;
		}

		public void BounceEntity(Entity entity) {
			if (entity.Physics.ZVelocity < -1.0f) {
				// Bounce back into the air.
				//hasLanded = false;
				entity.ZPosition = 0.1f;
				entity.Physics.ZVelocity = -entity.Physics.ZVelocity * 0.5f;
			}
			else {
				// Stay on the ground.
				entity.ZPosition = 0.0f;
				entity.Physics.ZVelocity = 0;
				entity.Physics.Velocity = Vector2F.Zero;
			}

			if (entity.Physics.Velocity.Length > 0.25)
				entity.Physics.Velocity *= 0.5f;
			else
				entity.Physics.Velocity = Vector2F.Zero;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}
	}
}
