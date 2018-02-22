using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMagnetGlovesState : PlayerState {

		private enum MagneticObjectType {
			None,
			Ball,
			Tile,
		};

		private MagneticObjectType magneticObjectType;
		private ItemMagnetGloves weapon;
		private AnimationPlayer effectAnimation;
		private object magneticObject;
		private Rectangle2F alignBox;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMagnetGlovesState() {
			StateParameters.ProhibitJumping	= true;
			StateParameters.EnableStrafing	= true;
			PlayerAnimations.Default		= GameData.ANIM_PLAYER_AIM_WALK;
			effectAnimation					= new AnimationPlayer();

			alignBox = new Rectangle2F(-10, -12, 20, 19);
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		private MagneticObjectType GetMagneticObjectType(object obj) {
			if (obj == null)
				return MagneticObjectType.None;
			if (obj is Tile)
				return MagneticObjectType.Tile;
			else if (obj is MagnetBall)
				return MagneticObjectType.Ball;
			return MagneticObjectType.None;
		}

		private void OnPullTileBegin() {
			player.Physics.IsFlying							= true;
			StateParameters.ProhibitMovementControlOnGround	= true;
			StateParameters.ProhibitMovementControlInAir	= RoomControl.IsSideScrolling;
			StateParameters.DisableGravity					= RoomControl.IsSideScrolling;
			StateParameters.EnableGroundOverride			= RoomControl.IsSideScrolling;
			Tile magneticTile = (Tile) magneticObject;

			// Determine the magnetic pull direction
			int moveDirection = player.Direction;
			if (Polarity == magneticTile.Polarity)
				moveDirection = Directions.Reverse(moveDirection);
			int axis = Directions.ToAxis(moveDirection);

			// If the player is moving in the opposite direction of the megetic
			// pull direction, then flip his velocity
			if (player.Physics.Velocity.Dot(
				Directions.ToVector(moveDirection)) < 0.0f)
			{
				Vector2F newVelocity = player.Physics.Velocity;
				newVelocity[axis] = -newVelocity[axis];
				player.Physics.Velocity = newVelocity;
			}

			player.Graphics.PlayAnimation(player.Animations.Throw);
		}

		private void OnPullTileEnd() {
			player.Physics.IsFlying							= false;
			StateParameters.ProhibitMovementControlOnGround	= false;
			StateParameters.ProhibitMovementControlInAir	= false;
			StateParameters.DisableGravity					= false;
			StateParameters.EnableGroundOverride			= false;
		}

		private void OnPullTileUpdate() {
			int axis = Directions.ToAxis(player.Direction);
			float moveSpeed = GameSettings.PLAYER_MAGNET_GLOVE_MOVE_SPEED;
			Tile magneticTile = (Tile) magneticObject;
				
			// Check if the player is jumping and can move while jumping
			bool canMoveDuringJump = true;
			if (player.RoomControl.IsSideScrolling &&
				player.Physics.Velocity.Y <= 0.1f)
				canMoveDuringJump = false;
			else if (!player.RoomControl.IsSideScrolling &&
				player.Physics.ZVelocity >= 0.1f)
				canMoveDuringJump = false;
			if (player.IsInAir && !canMoveDuringJump)
				player.Physics.Velocity = Vector2F.Zero;

			// Direction should be straight it the player is far enough away
			Vector2F velocity;
			float distance = GMath.Abs(
				magneticTile.Center[axis] - player.Center[axis]);
			if (distance > 32.0f)
				velocity = Directions.ToVector(player.Direction) * moveSpeed;
			else {
				velocity = (magneticTile.Center - player.Center).Normalized * moveSpeed;
					//new Vector2F(0, 1.5f)).Normalized * moveSpeed;
				velocity = Vector2F.SnapDirectionByCount(velocity, 16);
			}

			// If polarities are the same, then push away from the magnetic tile
			int lateralAxis = Axes.GetOpposite(axis);
			if (magneticTile.Polarity == Polarity) {
				velocity = -velocity;
				float lateralDistance = player.Center[lateralAxis] -
					magneticTile.Center[lateralAxis];
			}

			player.Position += velocity;
		}
		
		private void OnPullBallBegin() {
			MagnetBall ball = magneticObject as MagnetBall;
			ball.IsMoving = true;
			ball.Direction = player.Direction;
			if (ball.Polarity != Polarity)
				ball.Direction = Directions.Reverse(ball.Direction);
		}

		private void OnPullBallEnd() {
			MagnetBall ball = magneticObject as MagnetBall;
			ball.IsMoving = false;
		}

		private void OnPullBallUpdate() {
			int direction = player.Direction;
			int axis = Directions.ToAxis(player.Direction);
			int lateralAxis = Axes.GetOpposite(axis);
			MagnetBall ball = magneticObject as MagnetBall;
			float distance = GMath.Abs(ball.Center[axis] - player.Center[axis]);

			Vector2F velocity = Vector2F.Zero;

			if (distance > GameSettings.MAGNET_BALL_MIN_DISTANCE ||
				ball.Polarity == weapon.Polarity)
			{
				// Move away/toward the player
				velocity[axis] = ball.Physics.Velocity[axis];
				velocity += Directions.ToVector(ball.Direction) *
					GameSettings.MAGNET_BALL_ACCELERATION;
				velocity[axis] = GMath.Clamp(velocity[axis],
					-GameSettings.MAGNET_BALL_MAX_MOVE_SPEED,
					GameSettings.MAGNET_BALL_MAX_MOVE_SPEED);
			}
			else {
				// If close enough to the player, then move with the player
				velocity[axis] = player.Physics.Velocity[axis];
			}

			velocity[lateralAxis] = GMath.Clamp(
				player.Center[lateralAxis] - ball.Center[lateralAxis] +
				player.Physics.Velocity[lateralAxis], -1.0f, 1.0f);
			ball.Physics.Velocity = velocity;
		}

		/// <summary>Reverse the polarity of the magnetic gloves</summary>
		public void ReversePolarity() {
			if (weapon.Polarity == Polarity.North)
				weapon.Polarity = Polarity.South;
			else
				weapon.Polarity = Polarity.North;
		}

		/// <summary>Get the nearest magnetic object in front of and aligned with the 
		/// player, or null if none was found.</summary>
		public object GetMagneticObject() {
			int axis = Directions.ToAxis(player.Direction);
			int lateralAxis = Axes.GetOpposite(axis);
			RangeF lateralRange = alignBox.GetAxisRange(lateralAxis);
			lateralRange.Min -= GameSettings.EPSILON;
			lateralRange.Max += GameSettings.EPSILON;

			object bestObject = null;
			float bestDistance = 0.0f;

			// First check magnetic tiles
			Tile magneticTile = GetMagnetTile();
			if (magneticTile != null) {
				bestObject = magneticTile;
				bestDistance = GMath.Abs(
					magneticTile.Center[axis] - player.Center[axis]);
			}

			// Second check magnet balls
			foreach (MagnetBall ball in RoomControl.GetEntitiesOfType<MagnetBall>()) {
				float distance = GMath.Abs(ball.Center[axis] - player.Center[axis]);
				Vector2F ballToPlayer = player.Center - ball.Center;

				if (ball.IsOnGround &&
					lateralRange.Contains(ballToPlayer[lateralAxis]) &&
					Directions.NearestFromVector(
						ball.Center - player.Center) == player.Direction &&
					(bestObject == null || distance < bestDistance))
				{
					bestDistance = distance;
					bestObject = ball;
				}
			}

			return bestObject;
		}

		public Tile GetMagnetTile() {
			// Create an area to check for magnet tiles
			int axis = Directions.ToAxis(player.Direction);
			int lateralAxis = Axes.GetOpposite(axis);
			Rectangle2F checkArea = new Rectangle2F(-16, -16, 32, 32);
			checkArea.Point += player.Center;
			checkArea.ExtendEdge(player.Direction, RoomControl.RoomBounds.Size[axis]);
			Rectangle2I tileArea = RoomControl.GetTileAreaFromRect(checkArea);
			RangeF lateralRange = alignBox.GetAxisRange(lateralAxis);
			lateralRange.Min -= GameSettings.EPSILON;
			lateralRange.Max += GameSettings.EPSILON;

			Tile bestTile = null;
			float bestDistance = 0.0f;

			// Iterate tiles looking for tiles with a polarity in front of the player
			foreach (Tile tile in RoomControl.GetTopTilesInArea(tileArea)) {
				if (tile.Polarity != Polarity.None) {
					float distance = GMath.Abs(tile.Center[axis] - player.Center[axis]);
					float lateralDistance = GMath.Abs(tile.Center[lateralAxis] - player.Center[lateralAxis]);
					Vector2F tileToPlayer = player.Center - tile.Center;

					if (lateralRange.Contains(tileToPlayer[lateralAxis]) &&
						Directions.NearestFromVector(
							tile.Center - player.Center) == player.Direction &&
						(bestTile == null || distance < bestDistance))
					{
						bestDistance = distance;
						bestTile = tile;
					}
				}
			}

			return bestTile;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Direction = player.UseDirection;

			// Initialize the magnet effect
			Animation animation;
			if (Polarity == Polarity.North)
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_NORTH;
			else
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_SOUTH;
			effectAnimation.Play(animation);
			effectAnimation.SubStripIndex = player.Direction;

			magneticObjectType = MagneticObjectType.None;
			magneticObject = null;
		}

		public override void OnEnd(PlayerState newState) {
			// End pulling the magnetic object
			if (magneticObjectType == MagneticObjectType.Ball)
				OnPullBallEnd();
			else if (magneticObjectType == MagneticObjectType.Tile)
				OnPullTileEnd();

			player.Physics.IsFlying = false;
		}

		public override void Update() {

			object newMagneticObject = GetMagneticObject();
			MagneticObjectType newMagneticObjectType =
				GetMagneticObjectType(newMagneticObject);

			if (newMagneticObjectType != magneticObjectType) {
				// End pulling the previous magnetic object
				if (magneticObjectType == MagneticObjectType.Ball)
					OnPullBallEnd();
				else if (magneticObjectType == MagneticObjectType.Tile)
					OnPullTileEnd();
				
				magneticObject = newMagneticObject;
				magneticObjectType = newMagneticObjectType;

				// Begin pulling the new magnetic object
				if (magneticObjectType == MagneticObjectType.Ball)
					OnPullBallBegin();
				else if (magneticObjectType == MagneticObjectType.Tile)
					OnPullTileBegin();
			}
			else {
				magneticObject = newMagneticObject;
				magneticObjectType = newMagneticObjectType;
			}
			
			// Update the magnetic object state
			if (magneticObjectType == MagneticObjectType.Ball)
				OnPullBallUpdate();
			else if (magneticObjectType == MagneticObjectType.Tile)
				OnPullTileUpdate();

			// Udpate the magnet effect animation
			effectAnimation.SubStripIndex = player.Direction;
			effectAnimation.Update();

			// Check if the action button was released
			if (!weapon.IsButtonDown()) {
				ReversePolarity();
				End();
			}
			else if (!weapon.IsEquipped)
				End();
		}

		public override void DrawOver(RoomGraphics g) {
			// Draw the magnet effect
			g.DrawAnimationPlayer(effectAnimation, player.Position -
				new Vector2F(0, player.ZPosition),
				DepthLayer.EffectMagnetGloves);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemMagnetGloves Weapon {
			get { return weapon; }
			set { weapon = value; }
		}

		public Polarity Polarity {
			get { return weapon.Polarity; }
		}
	}
}
