using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Players.States;


//States:
//	- None (movement, item control)
//-------------------------------------
//	- Minecart (item control)
//	- Carrying Item (movement, throw)
//	- Swimming (movement, diving)
//-------------------------------------
//	- Ledge Jump
//	- Busy
//	- [SwitchHook]
//	- [SwingingItem]
//	- Die


namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Unit {

		// The current direction that the player wants to face to use items.
		private int useDirection;
		// The current angle that the player wants face to use items.
		private int useAngle;
		// TODO: better name for this.
		private bool syncAnimationWithDirection;
		// Is the player allowed to transition between rooms?
		private bool allowRoomTransition;
		// The player doesn't need to be moving to transition.
		private bool autoRoomTransition;
		// The position the player was at when he entered the room.
		private Vector2F roomEnterPosition;
		// The current player state.
		private PlayerState state;
		// The movement component for the player.
		private PlayerMoveComponent movement;

		private PlayerNormalState		stateNormal;
		private PlayerJumpState			stateJump;
		private PlayerSwimState			stateSwim;
		private PlayerLedgeJumpState	stateLedgeJump;
		private PlayerLadderState		stateLadder;
		private PlayerSwingState		stateSwing;
		private PlayerHoldSwordState	stateHoldSword;
		private PlayerSpinSwordState	stateSpinSword;

		// TEMP: Change tool drawing to something else
		public AnimationPlayer toolAnimation;

		private bool isItemButtonPressDisabled;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			direction			= Directions.Down;
			useDirection		= 0;
			useAngle			= 0;
			autoRoomTransition	= false;
			allowRoomTransition	= true;
			syncAnimationWithDirection	= true;
			isItemButtonPressDisabled	= false;
			movement = new PlayerMoveComponent(this);

			// Unit properties.
			originOffset	= new Point2I(0, -2);
			centerOffset	= new Point2I(0, -8);
			Health			= 4 * 3;
			MaxHealth		= 4 * 3;

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;
			Physics.CollideWithRoomEdge = true;

			// Graphics.
			Graphics.ShadowDrawOffset = originOffset;

			// Create the basic player states.
			state			= null;
			stateNormal		= new PlayerNormalState();
			stateJump		= new PlayerJumpState();
			stateSwim		= new PlayerSwimState();
			stateLadder		= new PlayerLadderState();
			stateLedgeJump	= new PlayerLedgeJumpState();
			stateSwing		= new PlayerSwingState();
			stateHoldSword	= new PlayerHoldSwordState();
			stateSpinSword	= new PlayerSpinSwordState();

			toolAnimation	= new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		public void Jump() {
			if (state is PlayerNormalState) {
				BeginState(stateJump);
			}
		}

		// Begin the given player state.
		public void BeginState(PlayerState newState) {
			if (state != newState) {
				if (state != null)
					state.End();
				state = newState;
				newState.Begin(this);
			}
		}

		// Return the player state that the player wants to be in
		// based on his current position.
		public PlayerState GetDesiredNaturalState() {
			if (IsInAir)
				return stateJump;
			else if (physics.IsInWater)
				return stateSwim;
			else if (physics.IsOnLadder)
				return stateLadder;
			else
				return stateNormal;
		}

		// Begin the desired natural state.
		public void BeginNormalState() {
			BeginState(GetDesiredNaturalState());
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		// Update items by checking if their buttons are pressed.
		public void UpdateEquippedItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				if (EquippedUsableItems[i] != null) {
					EquippedUsableItems[i].Player = this;

					if (!isItemButtonPressDisabled) {
						if (i == 0 && Controls.A.IsPressed())
							EquippedUsableItems[i].OnButtonPress();
						else if (i == 1 && Controls.B.IsPressed())
							EquippedUsableItems[i].OnButtonPress();

						if (i == 0 && Controls.A.IsDown())
							EquippedUsableItems[i].OnButtonDown();
						else if (i == 1 && Controls.B.IsDown())
							EquippedUsableItems[i].OnButtonDown();
					}
					//equippedItems[i].Update();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		private void UpdateUseDirections() {
			int controlDirection = -1;
			int controlAngle = -1;
			
			// Find control direction.
			// Arrow priority: Left > Up > Right > Down
			if (Controls.Left.IsDown())
				controlDirection = Directions.Left;
			else if (Controls.Up.IsDown())
				controlDirection = Directions.Up;
			else if (Controls.Right.IsDown())
				controlDirection = Directions.Right;
			else if (Controls.Down.IsDown())
				controlDirection = Directions.Down;

			// Find control angle.
			// Arrow priority: Left > Right, Up > Down
			if (Controls.Up.IsDown()) {
				if (Controls.Left.IsDown())
					controlAngle = Angles.UpLeft;
				else if (Controls.Right.IsDown())
					controlAngle = Angles.UpRight;
				else
					controlAngle = Angles.Up;
			}
			else if (Controls.Down.IsDown()) {
				if (Controls.Left.IsDown())
					controlAngle = Angles.DownLeft;
				else if (Controls.Right.IsDown())
					controlAngle = Angles.DownRight;
				else
					controlAngle = Angles.Down;
			}
			else if (Controls.Left.IsDown())
				controlAngle = Angles.Left;
			else if (Controls.Right.IsDown())
				controlAngle = Angles.Right;

			// Determine use angle/direction.
			if (movement.IsMoving && !movement.IsStrafing) {
				useAngle		= movement.MoveAngle;
				useDirection	= movement.MoveDirection;
			}
			else if (controlAngle >= 0) {
				useAngle		= controlAngle;
				useDirection	= controlDirection;
			}
			else {
				useAngle		= Directions.ToAngle(direction);
				useDirection	= direction;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			BeginState(stateNormal);
		}

		public override void OnEnterRoom() {
			state.OnEnterRoom();
		}

		public override void OnLeaveRoom() {
			state.OnLeaveRoom();
		}

		public override void Update() {
			isItemButtonPressDisabled = false;

			movement.Update();
			UpdateUseDirections();
			
			// Check for tile press interactions.
			Tile actionTile = physics.GetMeetingSolidTile(position, direction);
			if (actionTile != null && Controls.A.IsPressed()) {
				if (actionTile.OnAction(direction))
					isItemButtonPressDisabled = true;
				// TODO: player stops pushing when reading a sign.
			}

			// Update the current player state.
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			if (state != desiredNaturalState && state.RequestStateChange(desiredNaturalState))
				BeginState(desiredNaturalState);
			state.Update();

			// TEMPORARY: Change tool drawing to something else
			toolAnimation.Update();

			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update();
		}

		public override void Draw(Graphics2D g) {
			// TEMPORARY: Change tool drawing to something else
			if (toolAnimation.Animation != null)
				g.DrawAnimation(toolAnimation, position - new Vector2F(0, ZPosition), 0.6f);

			base.Draw(g);
			state.DrawOver(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Returns the inventory of the player.
		public Inventory Inventory {
			get { return GameControl.Inventory; }
		}

		public ItemWeapon[] EquippedUsableItems {
			get { return Inventory.EquippedWeapons; }
		}

		public int MoveAngle {
			get { return movement.MoveAngle; }
		}

		public int MoveDirection {
			get { return movement.MoveDirection; }
		}
		
		public int Direction {
			get { return direction; }
			set { direction = value; }
		}

		public int UseAngle {
			get { return useAngle; }
		}
		
		public int UseDirection {
			get { return useDirection; }
		}
		
		public PlayerMoveComponent Movement {
			get { return movement; }
			set { movement = value; }
		}
		
		public bool AllowRoomTransition {
			get { return movement.MoveMode.CanRoomChange; }
		}
		
		public bool SyncAnimationWithDirection {
			get { return syncAnimationWithDirection; }
			set { syncAnimationWithDirection = value; }
		}
		
		public bool AutoRoomTransition {
			get { return autoRoomTransition; }
			set { autoRoomTransition = value; }
		}
		
		public Vector2F RoomEnterPosition {
			get { return roomEnterPosition; }
			set { roomEnterPosition = value; }
		}
		
		// Player states

		public PlayerState CurrentState {
			get { return state; }
		}

		public PlayerNormalState NormalState {
			get { return stateNormal; }
		}
		
		public PlayerSwimState SwimState {
			get { return stateSwim; }
		}
		
		public PlayerJumpState JumpState {
			get { return stateJump; }
		}
		
		public PlayerLedgeJumpState LedgeJumpState {
			get { return stateLedgeJump; }
		}

		public PlayerLadderState LadderState {
			get { return stateLadder; }
		}

		public PlayerSwingState SwingState {
			get { return stateSwing; }
		}

		public PlayerHoldSwordState HoldSwordState {
			get { return stateHoldSword; }
		}

		public PlayerSpinSwordState SpinSwordState {
			get { return stateSpinSword; }
		}
	}
}
