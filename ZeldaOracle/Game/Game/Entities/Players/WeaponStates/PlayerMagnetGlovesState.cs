using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMagnetGlovesState : PlayerState {

		private ItemMagnetGloves weapon;
		private AnimationPlayer effectAnimation;
		private Tile magneticTile;
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
		
		private void UpdatePullState() {
			int axis = Directions.ToAxis(player.Direction);
			float moveSpeed = GameSettings.PLAYER_MAGNET_GLOVE_MOVE_SPEED;
				
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

		/// <summary>Reverse the polarity of the magnetic gloves</summary>
		public void ReversePolarity() {
			if (weapon.Polarity == Polarity.North)
				weapon.Polarity = Polarity.South;
			else
				weapon.Polarity = Polarity.North;
		}

		public Tile GetMagnetTile() {
			// Create an area to check for magnet tiles
			int axis = Directions.ToAxis(player.Direction);
			int lateralAxis = Axes.GetOpposite(axis);
			Rectangle2F checkArea = new Rectangle2F(-16, -16, 32, 32);
			checkArea.Point += player.Center;
			checkArea.ExtendEdge(player.Direction, RoomControl.RoomBounds.Size[axis]);
			Rectangle2I tileArea = RoomControl.GetTileAreaFromRect(checkArea);

			Tile bestTile = null;
			float bestDistance = 0.0f;

			// Iterate tiles looking for tiles with a polarity in front of the player
			foreach (Tile tile in RoomControl.GetTopTilesInArea(tileArea)) {
				if (tile.Polarity != Polarity.None) {
					float distance = GMath.Abs(tile.Center[axis] - player.Center[axis]);
					float lateralDistance = GMath.Abs(tile.Center[lateralAxis] - player.Center[lateralAxis]);
					Vector2F tileToPlayer = player.Center - tile.Center;
					RangeF lateralRange = alignBox.GetAxisRange(lateralAxis);
					lateralRange.Min -= GameSettings.EPSILON;
					lateralRange.Max += GameSettings.EPSILON;

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

		private void BeginPulling() {
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


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Direction = player.UseDirection;

			// Initialize the magnet effect
			Animation animation;
			if (Polarity == Polarity.North)
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_RED;
			else
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_BLUE;
			effectAnimation.Play(animation);
			effectAnimation.SubStripIndex = player.Direction;

			magneticTile = null;
		}

		public override void OnEnd(PlayerState newState) {
			player.Physics.IsFlying = false;
		}

		public override void Update() {
			// Check if the magnetic tile has changed
			Tile newMagnetTile = GetMagnetTile();
			if (newMagnetTile != magneticTile) {
				if (magneticTile == null) {
					magneticTile = newMagnetTile;
					BeginPulling();
				}
				else {
					magneticTile = newMagnetTile;
				}
			}

			// Update the pull or idle state
			if (magneticTile != null) {
				player.Physics.IsFlying = true;
				StateParameters.ProhibitMovementControlOnGround = true;
				UpdatePullState();
			}
			else {
				player.Physics.IsFlying = false;
				StateParameters.ProhibitMovementControlOnGround = false;
			}

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
