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
	
		private int				angle;
		//private UsableItem[]	equippedItems; // TODO: move this to somewhere else.
		private bool			syncAnimationWithDirection; // TODO: better name for this.
		private bool			checkGroundTiles;
		private bool			allowRoomTransition; // Is the player allowed to transition between rooms?
		private bool			autoRoomTransition; // The player doesn't need to be moving to transition.
		private Vector2F		roomEnterPosition; // The position the player was at when he entered the room.
		private PlayerMoveComponent		movement;

		private PlayerState				state;
		private PlayerNormalState		stateNormal;
		private PlayerJumpState			stateJump;
		private PlayerSwimState			stateSwim;
		private PlayerLedgeJumpState	stateLedgeJump;
		private PlayerLadderState		stateLadder;

		private Tile[]					frontTiles; // A list of tiles directly in front of the player.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			direction			= Directions.Down;
			angle				= Directions.ToAngle(direction);
			checkGroundTiles	= true;
			autoRoomTransition	= false;
			allowRoomTransition	= true;
			syncAnimationWithDirection = true;

			frontTiles = new Tile[2];

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
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		public void Jump() {
			if (state is PlayerNormalState) {
				BeginState(stateJump);
			}
				//((PlayerNormalState) state).Jump();
		}

		public void BeginState(PlayerState newState) {
			if (state != newState) {
				if (state != null)
					state.End();
				newState.Begin(this);
				state = newState;
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

		public void BeginNormalState() {
			BeginState(GetDesiredNaturalState());
		}

		private void CheckTiles() {
			Vector2F checkPosition = position + Directions.ToVector(direction);
			Rectangle2F myBox = physics.PositionedCollisionBox;
			int index = 0;
			for (int i = 0; i < frontTiles.Length; i++)
				frontTiles[i] = null;

			

			physics.IterateMeetingTiles(checkPosition, physics.CollisionBox, delegate(Tile tile) {
				Rectangle2F tileRect = new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
				if (!physics.IsPlaceMeetingTile(position, tile) &&
					!physics.CanDodgeCollision(tileRect, direction))
				{
					frontTiles[index++] = tile;
					if (index >= frontTiles.Length)
						return true;
				}
				return false;
			});

		}

		public void UpdateEquippedItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				if (EquippedUsableItems[i] != null) {
					EquippedUsableItems[i].Player = this;

					if (i == 0 && Controls.A.IsPressed())
						EquippedUsableItems[i].OnButtonPress();
					else if (i == 1 && Controls.B.IsPressed())
						EquippedUsableItems[i].OnButtonPress();

					if (i == 0 && Controls.A.IsDown())
						EquippedUsableItems[i].OnButtonDown();
					else if (i == 1 && Controls.B.IsDown())
						EquippedUsableItems[i].OnButtonDown();

					//equippedItems[i].Update();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			// Play the default player animation.
			Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);

			BeginState(stateNormal);
			//BeginState(new PlayerSwimState());
		}

		public override void OnEnterRoom() {
			state.OnEnterRoom();
		}

		public override void OnLeaveRoom() {
			state.OnLeaveRoom();
		}

		public override void Update() {

			CheckTiles();

			movement.Update();

			// Update the current player state.
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			if (state.IsNaturalState && state != desiredNaturalState)
				BeginState(desiredNaturalState);
			state.Update();

			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update();
		}

		public override void Draw(Graphics2D g) {
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

		public UsableItem[] EquippedUsableItems {
			get { return Inventory.EquippedUsableItems; }
		}

		public int MoveAngle {
			get { return movement.MoveAngle; }
		}

		public int MoveDirection {
			get { return movement.MoveDirection; }
		}

		public int Angle {
			get { return angle; }
			set { angle = value; }
		}
		
		public int Direction {
			get { return direction; }
			set { direction = value; }
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
		
		public bool CheckGroundTiles {
			get { return checkGroundTiles; }
			set { checkGroundTiles = value; }
		}
		
		public bool AutoRoomTransition {
			get { return autoRoomTransition; }
			set { autoRoomTransition = value; }
		}
		
		public Vector2F RoomEnterPosition {
			get { return roomEnterPosition; }
			set { roomEnterPosition = value; }
		}
		
		public Tile[] FrontTiles {
			get { return frontTiles; }
		}
		
		// Player states

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

		public PlayerState CurrentState {
			get { return state; }
		}
	}
}
