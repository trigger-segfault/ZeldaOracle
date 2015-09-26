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
		// The player doesn't need to be moving to transition.
		private bool autoRoomTransition;
		// The position the player was at when he entered the room.
		private Vector2F roomEnterPosition;
		// The current player state.
		private PlayerState state;
		// The previous player state.
		private PlayerState previousState;
		// The movement component for the player.
		private PlayerMoveComponent movement;


		private PlayerNormalState		stateNormal;
		private PlayerBusyState			stateBusy;
		private PlayerSwimState			stateSwim;
		private PlayerLedgeJumpState	stateLedgeJump;
		private PlayerLadderState		stateLadder;
		private PlayerSwingState		stateSwing;
		private PlayerHoldSwordState	stateHoldSword;
		private PlayerSpinSwordState	stateSpinSword;

		// TEMPORARY: Change tool drawing to something else
		public AnimationPlayer toolAnimation;

		private bool isItemButtonPressDisabled;

		private PlayerSwimmingSkills swimmingSkills;

		private PlayerTunics tunic;

		private int invincibleTimer;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int InvincibleDuration = 25;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			direction			= Directions.Down;
			useDirection		= 0;
			useAngle			= 0;
			autoRoomTransition	= false;
			syncAnimationWithDirection	= true;
			isItemButtonPressDisabled	= false;
			movement = new PlayerMoveComponent(this);

			// Unit properties.
			originOffset	= new Point2I(0, -2);
			centerOffset	= new Point2I(0, -8);
			Health			= 4 * 3;
			MaxHealth		= 4 * 3;
			swimmingSkills	= PlayerSwimmingSkills.CantSwim;
			invincibleTimer	= 0;
			tunic			= PlayerTunics.GreenTunic;

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;
			Physics.CollideWithRoomEdge = true;

			// Graphics.
			Graphics.ShadowDrawOffset = originOffset;
			Graphics.DrawOffset = new Point2I(-8, -16);

			// Create the basic player states.
			state			= null;
			stateNormal		= new PlayerNormalState();
			stateBusy		= new PlayerBusyState();
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

		// Begin the given player state.
		public void BeginState(PlayerState newState) {
			if (state != newState) {
				if (state != null) {
					state.End(newState);
				}
				previousState = state;
				state = newState;
				newState.Begin(this, previousState);
			}
		}

		// Return the player state that the player wants to be in
		// based on his current position.
		public PlayerState GetDesiredNaturalState() {
			if (physics.IsInWater)
				return stateSwim;
			else if (physics.IsOnLadder)
				return stateLadder;
			else
				return stateNormal;
		}

		// Begin the busy state with the specified duration.
		public void BeginBusyState(int duration) {
			stateBusy.Duration = duration;
			BeginState(stateBusy);
		}

		// Begin the desired natural state.
		public void BeginNormalState() {
			BeginState(GetDesiredNaturalState());
		}


		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------

		public void Hurt(int damage) {
			health = GMath.Max(0, health - damage);
			invincibleTimer = InvincibleDuration;
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		// Update items by checking if their buttons are pressed.
		public void UpdateEquippedItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				ItemWeapon item = EquippedUsableItems[i];
				if (item != null) {
					item.Player = this;

					if (!isItemButtonPressDisabled && item.IsUsable()) {
						if (Inventory.GetSlotButton(i).IsPressed())
							item.OnButtonPress();
						if (Inventory.GetSlotButton(i).IsDown())
							item.OnButtonDown();
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
			// Arrow priorities: Left > Right, Up > Down
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
			previousState = stateNormal;
		}

		public override void OnEnterRoom() {
			state.OnEnterRoom();
		}

		public override void OnLeaveRoom() {
			state.OnLeaveRoom();
		}

		public override void Update() {
			isItemButtonPressDisabled = false;

			if (invincibleTimer > 0)
				invincibleTimer--;

			movement.Update();
			UpdateUseDirections();
			
			// Check for tile press interactions.
			if (IsOnGround) {
				Tile actionTile = physics.GetMeetingSolidTile(position, direction);
				if (actionTile != null && Controls.A.IsPressed()) {
					if (actionTile.OnAction(direction))
						isItemButtonPressDisabled = true;
					// TODO: player stops pushing when reading text.
				}
			}

			// Update the current player state.
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			if (state != desiredNaturalState && state.RequestStateChange(desiredNaturalState))
				BeginState(desiredNaturalState);
			state.Update();
			UpdateEquippedItems();

			Rectangle2I tiles = RoomControl.GetTileAreaFromRect(physics.PositionedCollisionBox);
			for (int x = tiles.Left; x < tiles.Right; x++) {
				for (int y = tiles.Top; y < tiles.Bottom; y++) {
					for (int layer = 0; layer < RoomControl.Room.LayerCount; layer++) {
						Tile tile = RoomControl.GetTile(new Point2I(x, y), layer);
						if (tile != null)
							tile.OnTouch();
					}
				}
			}

			tiles = RoomControl.GetTileAreaFromRect(physics.PositionedCollisionBox, 1);
			for (int x = tiles.Left; x < tiles.Right; x++) {
				for (int y = tiles.Top; y < tiles.Bottom; y++) {
					for (int layer = 0; layer < RoomControl.Room.LayerCount; layer++) {
						Tile tile = RoomControl.GetTile(new Point2I(x, y), layer);
						if (tile != null && tile.CollisionModel != null) {
							for (int i = 0; i < tile.CollisionModel.BoxCount; i++) {
								if (((Rectangle2F)tile.CollisionModel[i] + tile.Position).Colliding(physics.PositionedCollisionBox.Inflated(1, 1))) {
									tile.OnCollide();
									break;
								}
							}
						}
					}
				}
			}

			// TEMPORARY: Change tool drawing to something else
			toolAnimation.Update();

			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
			
			// Set image variant based on tunic and hurt timer.
			if (invincibleTimer != 0 && (invincibleTimer + 1) % 8 < 4) {
				Graphics.ImageVariant = GameData.VARIANT_HURT;
			}
			else {
				switch (tunic) {
				case PlayerTunics.GreenTunic:	Graphics.ImageVariant = GameData.VARIANT_GREEN;	break;
				case PlayerTunics.RedTunic:		Graphics.ImageVariant = GameData.VARIANT_RED;	break;
				case PlayerTunics.BlueTunic:	Graphics.ImageVariant = GameData.VARIANT_BLUE;	break;
				}
			}

			// Update superclass.
			base.Update();
		}

		public override void Draw(Graphics2D g) {
			// TEMPORARY: Change tool drawing to something else
			if (toolAnimation.Animation != null)
				g.DrawAnimation(toolAnimation, position - new Vector2F(8, 16 + ZPosition), 0.6f);

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

		public PlayerSwimmingSkills SwimmingSkills {
			get { return swimmingSkills; }
			set { swimmingSkills = value; }
		}

		public PlayerTunics Tunic {
			get { return tunic; }
			set { tunic = value; }
		}
		
		// Player states

		public PlayerState CurrentState {
			get { return state; }
		}

		public PlayerState PreviousState {
			get { return previousState; }
		}

		public PlayerNormalState NormalState {
			get { return stateNormal; }
		}

		public PlayerBusyState BusyState {
			get { return stateBusy; }
		}

		public PlayerSwimState SwimState {
			get { return stateSwim; }
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
