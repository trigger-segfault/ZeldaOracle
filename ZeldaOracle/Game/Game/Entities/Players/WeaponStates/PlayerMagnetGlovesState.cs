using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMagnetGlovesState : PlayerState {

		//-----------------------------------------------------------------------------
		// Internal Types
		//-----------------------------------------------------------------------------
		
		/// <summary>Player condition state to make him hover for a short delay, so he
		/// doesn't instantly fall into a hole when reversing polarity from a Magnetic
		/// Spinner Tile.</summary>
		private class HoverState : PlayerState {
			private int timer;

			public HoverState() {
				StateParameters.DisableSurfaceContact = true;
			}
			public override void OnBegin(PlayerState previousState) {
				timer = 0;
			}
			public override void Update() {
				timer++;
				if (timer++ > 8)
					End();
			}
			public void Refresh() {
				timer = 0;
			}
		}

		private enum MagnetState {
			Idle,
			PullingBall,
			PullingTile,
			AttachedToSpinner,
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private GenericStateMachine<MagnetState> subStateMachine;
		private ItemMagnetGloves weapon;
		private object magneticObject;
		private Rectangle2F alignBox;
		private HoverState hoverDelayPlayerState;
		private Entity magnetEffect;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMagnetGlovesState() {
			StateParameters.ProhibitJumping	= true;
			StateParameters.EnableStrafing	= true;
			PlayerAnimations.Default		= GameData.ANIM_PLAYER_AIM_WALK;

			alignBox = new Rectangle2F(-10, -12, 20, 19);
			
			// Create player condition state
			hoverDelayPlayerState = new HoverState();

			// Setup magnet effect animation entity
			magnetEffect = new Entity();
			magnetEffect.Graphics.DepthLayer = DepthLayer.EffectMagnetGloves;

			// Setup sub-state machine
			subStateMachine = new GenericStateMachine<MagnetState>();
			subStateMachine.AddState(MagnetState.Idle)
				.OnUpdate(OnUpdateIdleState);
			subStateMachine.AddState(MagnetState.PullingBall)
				.OnBegin(OnBeginPullBallState)
				.OnEnd(OnEndPullBallState)
				.OnUpdate(OnUpdatePullBallState);
			subStateMachine.AddState(MagnetState.PullingTile)
				.OnBegin(OnBeginPullTileState)
				.OnEnd(OnEndPullTileState)
				.OnUpdate(OnUpdatePullTileState);
			subStateMachine.AddState(MagnetState.AttachedToSpinner)
				.OnBegin(OnBeginAttachedToSpinnerState)
				.OnEnd(OnEndAttachedToSpinnerState)
				.OnUpdate(OnUpdateAttachedToSpinnerState);
		}
		

		//-----------------------------------------------------------------------------
		// Magnet Spinner Interaction
		//-----------------------------------------------------------------------------

		/// <summary>Called by the Magnet Spinner tile when its rotation is complete.
		/// </summary>
		public void DetachFromSpinner() {
			subStateMachine.BeginState(MagnetState.Idle);
			magneticObject = null;
		}
		

		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnUpdateIdleState() {
			CheckMagneticObject();
		}

		private void OnBeginPullTileState() {
			StateParameters.DisableSurfaceContact			= true;
			StateParameters.ProhibitMovementControlOnGround	= true;
			StateParameters.ProhibitMovementControlInAir	= RoomControl.IsSideScrolling;
			StateParameters.DisableGravity					= RoomControl.IsSideScrolling;
			StateParameters.EnableGroundOverride			= RoomControl.IsSideScrolling;

			// Determine the magnetic pull direction
			Tile magneticTile = (Tile) magneticObject;
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

		private void OnEndPullTileState() {
			StateParameters.DisableSurfaceContact			= false;
			StateParameters.ProhibitMovementControlOnGround	= false;
			StateParameters.ProhibitMovementControlInAir	= false;
			StateParameters.DisableGravity					= false;
			StateParameters.EnableGroundOverride			= false;
		}

		private void OnUpdatePullTileState() {
			// Check if we stopped pulling this tile
			if (CheckMagneticObject())
				return;

			int axis = Directions.ToAxis(player.Direction);
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
				velocity = Directions.ToVector(player.Direction) *
					GameSettings.PLAYER_MAGNET_GLOVE_MOVE_SPEED;
			else {
				velocity = (magneticTile.Center - player.Center).Normalized *
					GameSettings.PLAYER_MAGNET_GLOVE_MOVE_SPEED;
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

			// Check if the tile is a Magnet Spinner and if we can attach to it
			if (magneticTile.Polarity != Polarity && distance <= 14 &&
				magneticTile is TileMagnetSpinner)
			{
				if (!hoverDelayPlayerState.IsActive)
					player.BeginConditionState(hoverDelayPlayerState);
				else
					hoverDelayPlayerState.Refresh();

				if (!((TileMagnetSpinner) magneticTile).IsRotating) {
					subStateMachine.EndCurrentState();
					player.SetPositionByCenter(magneticTile.Center -
						Directions.ToVector(player.Direction) *
						GameSettings.TILE_SIZE);
					subStateMachine.BeginState(MagnetState.AttachedToSpinner);
				}
			}
		}

		private void OnBeginAttachedToSpinnerState() {
			player.Physics.Velocity = Vector2F.Zero;

			StateParameters.DisableSurfaceContact			= true;
			StateParameters.ProhibitMovementControlOnGround	= true;
			StateParameters.ProhibitMovementControlInAir	= RoomControl.IsSideScrolling;
			StateParameters.DisableGravity					= RoomControl.IsSideScrolling;
			StateParameters.EnableGroundOverride			= RoomControl.IsSideScrolling;
			StateParameters.DisableSolidCollisions			= true;

			TileMagnetSpinner spinner = (TileMagnetSpinner) magneticObject;
			spinner.OnPlayerAttach();
		}

		private void OnEndAttachedToSpinnerState() {
			TileMagnetSpinner spinner = (TileMagnetSpinner) magneticObject;
			spinner.OnPlayerDetach();

			StateParameters.DisableSurfaceContact			= false;
			StateParameters.ProhibitMovementControlOnGround	= false;
			StateParameters.ProhibitMovementControlInAir	= false;
			StateParameters.DisableGravity					= false;
			StateParameters.EnableGroundOverride			= false;
			StateParameters.DisableSolidCollisions			= false;
		}

		private void OnUpdateAttachedToSpinnerState() {
			player.Physics.Velocity = Vector2F.Zero;

			// The Magnet Spinner tile will detach the player when its next rotation
			// is complete

			hoverDelayPlayerState.Refresh();
		}
		
		private void OnBeginPullBallState() {
			MagnetBall ball = (MagnetBall) magneticObject;
			ball.IsMoving = true;
			ball.Direction = player.Direction;
			if (ball.Polarity != Polarity)
				ball.Direction = Directions.Reverse(ball.Direction);
		}

		private void OnEndPullBallState() {
			MagnetBall ball = (MagnetBall) magneticObject;
			ball.IsMoving = false;
		}

		private void OnUpdatePullBallState() {
			// Check if we stopped pulling this magnet ball
			if (CheckMagneticObject())
				return;

			MagnetBall ball = (MagnetBall) magneticObject;
			int axis = Directions.ToAxis(player.Direction);
			int lateralAxis = Axes.GetOpposite(axis);
			float distance = GMath.Abs(ball.Center[axis] - player.Center[axis]);

			// Determine the magnet ball's velocity
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

			// Always move laterally toward the player
			velocity[lateralAxis] = GMath.Clamp(
				player.Center[lateralAxis] - ball.Center[lateralAxis] +
					player.Physics.Velocity[lateralAxis],
				-GameSettings.MAGNET_BALL_LATERAL_MOVE_SPEED,
				GameSettings.MAGNET_BALL_LATERAL_MOVE_SPEED);

			ball.Physics.Velocity = velocity;
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Reverse the polarity of the magnetic gloves weapon.</summary>
		private void ReversePolarity() {
			if (weapon.Polarity == Polarity.North)
				weapon.Polarity = Polarity.South;
			else
				weapon.Polarity = Polarity.North;
		}

		/// <summary>Check for the magnetic object that is in front of (and closest to)
		/// the player. If this object is different from the current one, then
		/// transition to the sub-state for the new object.</summary>
		private bool CheckMagneticObject() {
			object newMagneticObject = GetMagneticObject();

			if (newMagneticObject != magneticObject) {

				// End pulling the previous magnetic object
				subStateMachine.EndCurrentState();

				magneticObject = newMagneticObject;

				// Begin pulling the new magnetic object
				if (magneticObject is Tile)
					subStateMachine.BeginState(MagnetState.PullingTile);
				else if (magneticObject is MagnetBall)
					subStateMachine.BeginState(MagnetState.PullingBall);
				else
					subStateMachine.BeginState(MagnetState.Idle);
				return true;
			}

			return false;
		}

		/// <summary>Get the nearest magnetic object in front of and aligned with the 
		/// player, or null if none was found.</summary>
		private object GetMagneticObject() {
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
		
		/// <summary>Get the nearest magnetic tile in front of and aligned with the 
		/// player, or null if none was found.</summary>
		private Tile GetMagnetTile() {
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
					float distance = GMath.Abs(
						tile.Center[axis] - player.Center[axis]);
					float lateralDistance = GMath.Abs(
						tile.Center[lateralAxis] - player.Center[lateralAxis]);
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

			// Attach a magnetic effect to the player
			Animation animation;
			if (Polarity == Polarity.North)
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_NORTH;
			else
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_SOUTH;
			magnetEffect.Graphics.PlayAnimation(animation);
			magnetEffect.Graphics.SubStripIndex = player.Direction;
			player.AttachEntity(magnetEffect);

			// Begin the idle sub-state
			magneticObject = null;
			subStateMachine.InitializeOnState(MagnetState.Idle);
		}

		public override void OnEnd(PlayerState newState) {
			// End pulling the magnetic object
			subStateMachine.EndCurrentState();
			magnetEffect.Destroy();
			AudioSystem.PlaySound(GameData.SOUND_MAGNET_GLOVES_STOP);
		}

		public override void Update() {
			// Update the magnetic object state
			subStateMachine.Update();

			// Udpate the magnet effect animation
			magnetEffect.Graphics.SubStripIndex = player.Direction;

			// Loop the magnet gloves sound while this update method is called
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_MAGNET_GLOVES_LOOP);

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
			//effectAnimation.SubStripIndex = player.Direction;
			//g.DrawAnimationPlayer(effectAnimation, player.Position -
				//new Vector2F(0, player.ZPosition),
				//DepthLayer.EffectMagnetGloves);
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
