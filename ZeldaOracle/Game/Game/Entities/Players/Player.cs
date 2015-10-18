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
using ZeldaOracle.Common.Audio;


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
		private Vector2F respawnPosition;
		private int respawnDirection;
		// The current player state.
		private PlayerState state;
		// The previous player state.
		private PlayerState previousState;
		// The movement component for the player.
		private PlayerMoveComponent movement;

		private Animation moveAnimation;

		private bool isStateControlled; // Is the player fully being controlled by its current state?


		private PlayerNormalState		stateNormal;
		private PlayerBusyState			stateBusy;
		private PlayerSwimState			stateSwim;
		private PlayerLedgeJumpState	stateLedgeJump;
		private PlayerLadderState		stateLadder;
		private PlayerSwingState		stateSwing;
		private PlayerHoldSwordState	stateHoldSword;
		private PlayerSwordStabState	stateSwordStab;
		private PlayerSpinSwordState	stateSpinSword;
		private PlayerRespawnDeathState stateRespawnDeath;

		// TEMPORARY: Change tool drawing to something else
		public AnimationPlayer toolAnimation;

		private PlayerSwimmingSkills swimmingSkills;

		private PlayerTunics tunic;

		private int invincibleTimer;
		private float knockbackDirection;
		private bool useKnockback;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int InvincibleDuration = 25;
		private const int InvincibleControlRestoreDuration = 8;

		private const int KnockbackSnapCount = 16;
		private const float KnockbackSpeed = 1.3f;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			direction			= Directions.Down;
			useDirection		= 0;
			useAngle			= 0;
			autoRoomTransition	= false;
			isStateControlled	= false;
			syncAnimationWithDirection = true;
			movement = new PlayerMoveComponent(this);

			// Unit properties.
			originOffset	= new Point2I(0, -3);
			centerOffset	= new Point2I(0, -8);
			Health			= 4 * 3;
			MaxHealth		= 4 * 3;
			swimmingSkills	= PlayerSwimmingSkills.CantSwim;
			invincibleTimer	= 0;
			knockbackDirection	= 0f;
			useKnockback	= false;
			tunic			= PlayerTunics.GreenTunic;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-4, -10, 8, 9);
			Physics.SoftCollisionBox	= new Rectangle2F(-6, -14, 12, 13);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithEntities	= true;
			//Physics.CollideWithRoomEdge	= true;
			Physics.HasGravity			= true;

			// Graphics.
			Graphics.ShadowDrawOffset = originOffset;
			Graphics.DrawOffset = new Point2I(-8, -16);

			// Create the basic player states.
			state				= null;
			stateNormal			= new PlayerNormalState();
			stateBusy			= new PlayerBusyState();
			stateSwim			= new PlayerSwimState();
			stateLadder			= new PlayerLadderState();
			stateLedgeJump		= new PlayerLedgeJumpState();
			stateSwing			= new PlayerSwingState();
			stateHoldSword		= new PlayerHoldSwordState();
			stateSwordStab		= new PlayerSwordStabState();
			stateSpinSword		= new PlayerSpinSwordState();
			stateRespawnDeath	= new PlayerRespawnDeathState();

			toolAnimation	= new AnimationPlayer();

			moveAnimation	= GameData.ANIM_PLAYER_DEFAULT;

			Physics.CustomCollisionFunction = CheckRoomEdgeCollisions;
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		// Mark the player's current position/direction as where he should respawn.
		public void MarkRespawn() {
			respawnPosition		= position;
			respawnDirection	= direction;
		}

		public void Respawn() {
			position	= respawnPosition;
			direction	= respawnDirection;
			RoomControl.OnPlayerRespawn();
			
			// TODO: Break any breakable blocks the player respawns and collides with.
		}

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
			if (physics.IsInWater || physics.IsInOcean || physics.IsInLava)
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

		// For when the player needs to stop pushing, such as when reading text or opening a chest.
		public void StopPushing() {
			if (state == stateNormal)
				stateNormal.StopPushing();
			movement.IsMoving = false;
			movement.StopMotion();
			movement.ChooseAnimation();
		}

		public override void Hurt(int damage) {
			health = GMath.Max(0, health - damage);
			graphics.IsHurting = true;
			invincibleTimer = InvincibleDuration;
			useKnockback = false;
			//movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
		}

		public override void Hurt(int damage, float radians) {
			health = GMath.Max(0, health - damage);
			graphics.IsHurting = true;
			invincibleTimer = InvincibleDuration;
			knockbackDirection = radians;
			useKnockback = true;
			//movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
		}

		public override void RespawnDeath() {
			BeginState(stateRespawnDeath);
		}

		public override void Death() {
			base.Death();
		}

		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		public void InterruptItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				ItemWeapon item = EquippedUsableItems[i];
				if (item != null) {
					item.Interrupt();
					if (item.IsTwoHanded)
						break;
				}
			}
		}

		// Update items by checking if their buttons are pressed.
		public void UpdateEquippedItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				ItemWeapon item = EquippedUsableItems[i];
				if (item != null) {
					if (item.IsUsable()) {
						if (Inventory.GetSlotButton(i).IsPressed())
							item.OnButtonPress();
						if (Inventory.GetSlotButton(i).IsDown())
							item.OnButtonDown();
						item.Update();
					}
					if (item.IsTwoHanded)
						break;
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

		// Check if the player can room-transition in the given direction.
		private bool CanRoomTransition(int transitionDirection) {
			if (!AllowRoomTransition)
				return false;
			if (AutoRoomTransition)
				return true;
			if (movement.MoveCondition != PlayerMoveCondition.FreeMovement || !movement.IsMoving)
				return false;
			return (Controls.GetArrowControl(transitionDirection).IsDown() || Controls.GetAnalogDirection(transitionDirection));
		}
		
		// Custom collision function for colliding with room edges.
		private void CheckRoomEdgeCollisions() {
			Rectangle2F roomBounds = RoomControl.RoomBounds;
			Rectangle2F myBox = Physics.SoftCollisionBox;
			myBox.Point += position + Physics.Velocity;
			int transitionDirection = -1;

			// Collide with room edges.
			if (myBox.Left < roomBounds.Left) {
				position.X = roomBounds.Left - physics.SoftCollisionBox.Left;
				physics.VelocityX = 0;
				if (CanRoomTransition(Directions.Left))
					transitionDirection = Directions.Left;
			}
			else if (myBox.Right > roomBounds.Right) {
				position.X = roomBounds.Right - physics.SoftCollisionBox.Right;
				physics.VelocityX = 0;
				if (CanRoomTransition(Directions.Right))
					transitionDirection = Directions.Right;
			}
			if (myBox.Top < roomBounds.Top) {
				position.Y = roomBounds.Top - physics.SoftCollisionBox.Top;
				physics.VelocityY = 0;
				if (transitionDirection < 0 && CanRoomTransition(Directions.Up))
					transitionDirection = Directions.Up;
			}
			else if (myBox.Bottom > roomBounds.Bottom) {
				position.Y = roomBounds.Bottom - physics.SoftCollisionBox.Bottom;
				physics.VelocityY = 0;
				if (transitionDirection < 0 && CanRoomTransition(Directions.Down))
					transitionDirection = Directions.Down;
			}

			// Request a transition on the room edge.
			if (transitionDirection >= 0) {
				physics.Velocity = Vector2F.Zero;
				RoomControl.RequestRoomTransition(transitionDirection);
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
			bool performedAction = false;

			if (!isStateControlled) {
				// Update hurting.
				if (invincibleTimer > 0) {
					invincibleTimer--;
					if (invincibleTimer == 0)
						graphics.IsHurting = false;
				}

				movement.Update();
				UpdateUseDirections();
			
				// Check for tile & entity press interactions.
				if (IsOnGround && Controls.A.IsPressed()) {
					Entity actionEntity = null;
					for (int i = 0; i < RoomControl.Entities.Count; i++) {
						Entity e = RoomControl.Entities[i];
						if (e != this && !e.IsDestroyed && e.Physics.IsSolid && Physics.IsSoftMeetingEntity(e) &&
							Entity.AreEntitiesAligned(this, e, direction, e.ActionAlignDistance) &&
							e.OnPlayerAction(direction))
						{
							actionEntity = e;
							Controls.A.Disable(true);
							performedAction = true;
							break;
						}
					}
					if (actionEntity == null) {
						Tile actionTile = physics.GetMeetingSolidTile(position, direction);
						if (actionTile != null && actionTile.OnAction(direction)) {
							Controls.A.Disable(true);
							performedAction = true;
						}
					}
				}

				if (IsOnGround && movement.JumpStartTile != -Point2I.One) {
					RoomControl.GetTopTile(RoomControl.GetTileLocation(Origin)).OnLand(movement.JumpStartTile);
					movement.JumpStartTile = -Point2I.One;
				}

				// Try to switch to a natural state.
				PlayerState desiredNaturalState = GetDesiredNaturalState();
				if (state != desiredNaturalState && state.RequestStateChange(desiredNaturalState))
					BeginState(desiredNaturalState);
			}
			
			// Update the current player state.
			state.Update();
			
			if (!isStateControlled) {

				if (invincibleTimer > InvincibleControlRestoreDuration && useKnockback) {
					Vector2F motion = new Vector2F(KnockbackSpeed, knockbackDirection, true);
					// Snap velocity direction.
					float snapInterval = ((float)GMath.Pi * 2.0f) / KnockbackSnapCount;
					float theta = (float)Math.Atan2(-motion.Y, motion.X);
					if (theta < 0)
						theta += (float)Math.PI * 2.0f;
					int angle = (int)((theta / snapInterval) + 0.5f);
					Physics.Velocity = new Vector2F(
						(float)Math.Cos(angle * snapInterval) * motion.Length,
						(float)-Math.Sin(angle * snapInterval) * motion.Length);
				}
				else if (invincibleTimer == InvincibleControlRestoreDuration) {
					//movement.MoveCondition = PlayerMoveCondition.FreeMovement;
				}

				UpdateEquippedItems();

				if (performedAction)
					StopPushing();

				// Notify for touching tiles.
				// TODO: move this somewhere else.
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

				// Notify colliding tiles.
				// TODO: move this somewhere else.
				Rectangle2F myBox = physics.PositionedCollisionBox.Inflated(1, 1);
				tiles = RoomControl.GetTileAreaFromRect(myBox);
				for (int x = tiles.Left; x < tiles.Right; x++) {
					for (int y = tiles.Top; y < tiles.Bottom; y++) {
						for (int layer = 0; layer < RoomControl.Room.LayerCount; layer++) {
							Tile tile = RoomControl.GetTile(new Point2I(x, y), layer);
							if (tile != null && tile.CollisionModel != null) {
								for (int i = 0; i < tile.CollisionModel.BoxCount; i++) {
									if (((Rectangle2F)tile.CollisionModel[i] + tile.Position).Colliding(myBox)) {
										tile.OnCollide();
										break;
									}
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
			
			switch (tunic) {
			case PlayerTunics.GreenTunic:	Graphics.ImageVariant = GameData.VARIANT_GREEN;	break;
			case PlayerTunics.RedTunic:		Graphics.ImageVariant = GameData.VARIANT_RED;	break;
			case PlayerTunics.BlueTunic:	Graphics.ImageVariant = GameData.VARIANT_BLUE;	break;
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
			set {
				direction = value;
				if (syncAnimationWithDirection)
					graphics.SubStripIndex = direction;
			}
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

		public PlayerSwimmingSkills SwimmingSkills {
			get { return swimmingSkills; }
			set { swimmingSkills = value; }
		}

		public PlayerTunics Tunic {
			get { return tunic; }
			set { tunic = value; }
		}

		public bool CanUseWarpPoint {
			get { return movement.CanUseWarpPoint; }
			set { movement.CanUseWarpPoint = value; }
		}

		public Animation MoveAnimation {
			get { return moveAnimation; }
			set { moveAnimation = value; }
		}

		public int InvincibleTimer {
			get { return invincibleTimer; }
			set { invincibleTimer = value; }
		}

		public bool IsBeingKnockedBack {
			get { return (invincibleTimer > InvincibleControlRestoreDuration && useKnockback); }
		}

		public bool IsStateControlled {
			get { return isStateControlled; }
			set { isStateControlled = value; }
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

		public PlayerSwordStabState SwordStabState {
			get { return stateSwordStab; }
		}

		public PlayerSpinSwordState SpinSwordState {
			get { return stateSpinSword; }
		}

		public PlayerRespawnDeathState RespawnDeathState {
			get { return stateRespawnDeath; }
		}
	}
}
