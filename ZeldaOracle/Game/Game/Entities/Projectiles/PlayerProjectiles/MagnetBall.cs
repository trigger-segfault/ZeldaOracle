using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class MagnetBall : Entity {

		private Polarity polarity;
		private int direction;
		private bool isMoving;
		private bool isLedgeJumping;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MagnetBall() {
			// Graphics
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;
			Graphics.DepthLayer				= DepthLayer.ProjectileBomb;
			Graphics.DrawOffset				= new Point2I(-8, -8);
			centerOffset					= Point2I.Zero;

			// Physics
			Physics.Enable(
				PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.CollideRoomEdge |
				PhysicsFlags.DestroyedInHoles |
				PhysicsFlags.MoveWithConveyors |
				PhysicsFlags.EdgeClipping);
			Physics.EdgeClipAmount	= 3;
			Physics.CollisionBox	= new Rectangle2F(-8, -8, 16, 16);
			soundBounce				= GameData.SOUND_KEY_BOUNCE;
			
			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox		= new Rectangle2F(-8, -8, 16, 16);
			Interactions.InteractionType	= InteractionType.MagnetBall;

			// Magnet Ball
			polarity = Polarity.North;

		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begin falling off of a ledge.</summary>
		private void BeginLedgeJump(int ledgeDirection, Tile ledgeTile) {
			isLedgeJumping = true;
			physics.ZVelocity = 1.8f;
			physics.Velocity = Directions.ToVector(ledgeDirection) * 1.0f;
			physics.CollideWithWorld = false;
			Interactions.Disable();
		}

		/// <summary>End falling off of a ledge.</summary>
		private void EndLedgeJump() {
			isLedgeJumping = false;
			physics.CollideWithWorld = true;
			Interactions.Disable();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (polarity == Polarity.North)
				Graphics.PlayAnimation(GameData.SPR_MAGNET_BALL_NORTH);
			else
				Graphics.PlayAnimation(GameData.SPR_MAGNET_BALL_SOUTH);

			isLedgeJumping	= false;
			isMoving		= false;
			direction		= Directions.Right;
		}

		public override void Update() {

			if (isLedgeJumping) {
				if (IsOnGround)
					EndLedgeJump();
			}
			else {
				// Collide with player
				Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
				float distanceToPlayer = vectorToPlayer.Length;
				if (physics.PositionedCollisionBox.Intersects(
					RoomControl.Player.Physics.PositionedCollisionBox))
				{
					RoomControl.Player.Position += vectorToPlayer.Normalized * 3.0f;
				}

				// Slow down
				if (!isMoving) {
					int axis = Directions.ToAxis(direction);
					Vector2F directionVector = Directions.ToVector(direction);
					if (physics.Velocity.Dot(directionVector) > 0.0f) 
						physics.Velocity -= directionVector *
							GameSettings.MAGNET_BALL_DECELERATION;
					else
						physics.Velocity = Vector2F.Zero;
				}
			
				// Check for ledge jumping
				foreach (Collision collision in Physics.Collisions) {
					if (collision.IsTile && !collision.IsDodged) {
						Tile tile = collision.Tile;
						if (Directions.ToVector(tile.LedgeDirection)
								.Dot(Center - tile.Center) < 0.0f &&
							tile.LedgeDirection == collision.Direction &&
							tile.IsLedge && !tile.IsInMotion)
							BeginLedgeJump(tile.LedgeDirection, tile);
					}
				}

				// Only trigger interactions if moving
				float speed = physics.Velocity.Length;
				if (speed >= 1.0f)
					Interactions.InteractionType = InteractionType.MagnetBall;
				else
					Interactions.InteractionType = InteractionType.None;
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The ball's magnetic polarity (North or South).</summary>
		public Polarity Polarity {
			get { return polarity; }
		}
		
		/// <summary>True if being moved by the magnet gloves.</summary>
		public bool IsMoving {
			get { return isMoving; }
			set { isMoving = value; }
		}
		
		/// <summary>The last-known movement direction caused by the magnet gloves.
		/// </summary>
		public int Direction {
			get { return direction; }
			set { direction = value; }
		}
		
		/// <summary>True if the magnet ball is currently falling off of a ledge.
		/// </summary>
		public bool IsLedgeJumping {
			get { return isLedgeJumping; }
		}
	}
}
