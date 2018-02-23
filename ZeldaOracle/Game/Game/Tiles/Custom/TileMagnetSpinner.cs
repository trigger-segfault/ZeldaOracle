using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TileMagnetSpinner : Tile {

		private enum MagnetSpinnerState {
			Idle,
			Rotating,
		}

		private int timer;
		private bool isPlayerAttached;
		private GenericStateMachine<MagnetSpinnerState> stateMachine;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMagnetSpinner() {
			Graphics.SyncPlaybackWithRoomTicks = false;
			fallsInHoles = false;

			stateMachine = new GenericStateMachine<MagnetSpinnerState>();
			stateMachine.AddState(MagnetSpinnerState.Idle)
				.OnBegin(OnBeginIdleState)
				.SetDuration(30);
			stateMachine.AddState(MagnetSpinnerState.Rotating)
				.OnBegin(OnBeginRotatingState)
				.OnEnd(OnEndRotatingState)
				.OnUpdate(OnUpdateRotatingState);
		}

		
		//-----------------------------------------------------------------------------
		// Magnet Glove Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Called by the player's Magnet Gloves State when he attaches to
		/// this spinner.</summary>
		public void OnPlayerAttach() {
			isPlayerAttached = true;
		}
		
		/// <summary>Called by the player's Magnet Gloves State when he detaches from
		/// this spinner.</summary>
		public void OnPlayerDetach() {
			isPlayerAttached = false;
		}


		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginIdleState() {
			isPlayerAttached = false;

			// Player the stationary animation
			if (Polarity == Polarity.North)
				Graphics.PlayAnimation(
					GameData.ANIM_TILE_MAGNET_SPINNER_NORTH);
			else 
				Graphics.PlayAnimation(
					GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH);
			Graphics.SubStripIndex =
				(RotationDirection == WindingOrder.Clockwise ? 0 : 1);
		}

		private void OnBeginRotatingState() {
			timer = 1;

			// Play the rotate animation
			if (Polarity == Polarity.North)
				Graphics.PlayAnimation(
					GameData.ANIM_TILE_MAGNET_SPINNER_NORTH_ROTATE);
			else 
				Graphics.PlayAnimation(
					GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH_ROTATE);
			Graphics.SubStripIndex =
				(RotationDirection == WindingOrder.Clockwise ? 0 : 1);

			// Move the player with the rotation
			if (isPlayerAttached)
				RotatePlayer();
		}

		private void OnEndRotatingState() {
			// Move the player with the rotation then detach him
			if (isPlayerAttached) {
				RotatePlayer(true);
				isPlayerAttached = false;
				RoomControl.Player.MagnetGlovesState.DetachFromSpinner();
			}
		}

		private void OnUpdateRotatingState() {
			timer++;
				
			// Move the player with the rotation
			if (isPlayerAttached &&  (timer == 15 || timer == 15 + 4))
				RotatePlayer();

			// End the rotation after the animation is complete
			if (timer == 15 + 4 + 3) 
				stateMachine.NextState();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Move the player diagonally with the spinner rotation, and
		/// optionaly rotate his direction as well.</summary>
		public void RotatePlayer(bool rotateDirection = false) {
			Player player = RoomControl.Player;

			// Move the player's position diagonally
			Vector2F moveAmount = Vector2F.Zero;
			moveAmount += Directions.ToVector(player.Direction);
			moveAmount -= Directions.ToVector(
				Directions.Add(player.Direction, 1, RotationDirection));
			player.Position += moveAmount * 4.0f;

			// Rotate the player's direction
			if (rotateDirection)
				player.Direction = Directions.Add(
					player.Direction, 1, RotationDirection);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			stateMachine.BeginState(MagnetSpinnerState.Idle);
		}

		public override void Update() {
			stateMachine.Update();
			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Polarity polarity = args.Properties.GetEnum(
				"polarity", Polarity.South);
			WindingOrder rotationDirection = args.Properties.GetEnum(
				"rotation_direction", WindingOrder.Clockwise);

			Animation animation = null;
			if (polarity == Polarity.North)
				animation = GameData.ANIM_TILE_MAGNET_SPINNER_NORTH_ROTATE_CONTINUOUS;
			else 
				animation = GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH_ROTATE_CONTINUOUS;

			if (rotationDirection == WindingOrder.CounterClockwise)
				animation = animation.GetSubstrip(1);

			g.DrawSprite(
				animation,
				args.SpriteDrawSettings,
				args.Position,
				args.Color);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public WindingOrder RotationDirection {
			get { return Properties.GetEnum("rotation_direction", WindingOrder.Clockwise); }
			set { Properties.Set("rotation_direction", (int) value); }
		}

		public bool IsRotating {
			get { return (stateMachine.CurrentState == MagnetSpinnerState.Rotating); }
		}
	}
}
