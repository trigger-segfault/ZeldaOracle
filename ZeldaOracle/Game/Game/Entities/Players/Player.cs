using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Players.States.SwingStates;
using ZeldaOracle.Game.Entities.Players.Tools;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players {
	
	// TODO: use this struct
	public struct PlayerAnimationSet {
		public Animation Swing;
		public Animation Spin;
		public Animation Stab;
		public Animation Aim;
		public Animation Throw;
		public Animation Default;
	}

	public class Player : Unit {

		/// <summary>The current direction that the player wants to face to use items.</summary>
		private int useDirection;
		/// <summary>The current angle that the player wants face to use items.</summary>
		private int useAngle;
		/// <summary>The player doesn't need to be moving to transition.</summary>
		private bool autoRoomTransition;
		/// <summary>The position the player was at when he entered the room.</summary>
		private Vector2F respawnPosition;
		private int respawnDirection;
		/// <summary>The movement component for the player.</summary>
		private PlayerMoveComponent movement;

		private PlayerStateMachine environmentStateMachine;
		private PlayerStateMachine controlStateMachine;
		private PlayerStateMachine weaponStateMachine;
		private List<PlayerStateMachine> conditionStateMachines;
		private PlayerStateParameters stateParameters;

		private bool isFrozen;

		private Animation moveAnimation;

		private bool isStateControlled; // Is the player fully being controlled by its current state?

		private Vector2F viewFocusOffset;

		public delegate void PlayerDelegate(Player player);

		private event PlayerDelegate eventJump;
		private event PlayerDelegate eventLand;

		private PlayerSwimmingSkills	swimmingSkills;
		private PlayerTunics			tunic;

		// Player Tools
		private PlayerToolShield	toolShield;
		private PlayerToolSword		toolSword;
		private PlayerToolVisual	toolVisual;

		// Player States
		private PlayerBusyState				stateBusy;
		private PlayerLedgeJumpState		stateLedgeJump;
		private PlayerLeapLedgeJumpState	stateLeapLedgeJump;
		private PlayerSwingSwordState		stateSwingSword;
		private PlayerSwingBigSwordState	stateSwingBigSword;
		private PlayerSwingCaneState		stateSwingCane;
		private PlayerSwingMagicRodState	stateSwingMagicRod;
		private PlayerHoldSwordState		stateHoldSword;
		private PlayerSwordStabState		stateSwordStab;
		private PlayerSpinSwordState		stateSpinSword;
		private PlayerSeedShooterState		stateSeedShooter;
		private PlayerSwitchHookState		stateSwitchHook;
		private PlayerSwitchHookSwitchState	stateSwitchHookSwitch;
		private PlayerMagicBoomerangState	stateMagicBoomerang;
		private PlayerGrabState				stateGrab;
		private PlayerCarryState			stateCarry;
		private PlayerPullHandleState		statePullHandle;
		private PlayerRespawnDeathState		stateRespawnDeath;
		private PlayerMinecartState			stateMinecart;
		private PlayerJumpToState			stateJumpTo;

		private PlayerGrassEnvironmentState					environmentStateGrass;
		private PlayerStairsEnvironmentState				environmentStateStairs;
		private PlayerJumpEnvironmentState					environmentStateJump;
		private PlayerIceEnvironmentState					environmentStateIce; 
		private PlayerLadderEnvironmentState				environmentStateLadder;
		private PlayerSideScrollLadderEnvironmentState		environmentStateSideScrollLadder;
		private PlayerSwimEnvironmentState					environmentStateSwim;
		private PlayerUnderwaterEnvironmentState			environmentStateUnderwater;
		private PlayerSideScrollSwimEnvironmentState		environmentStateSideScrollSwim;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int	InvincibleDuration					= 25;
		private const int	InvincibleControlRestoreDuration	= 8;
		private const int	KnockbackSnapCount					= 16;
		private const float	KnockbackSpeed						= 1.3f;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			// Unit properties
			centerOffset			= new Point2I(0, -5);
			MaxHealth               = 4 * 3;
			Health					= 4 * 3;
			swimmingSkills			= PlayerSwimmingSkills.CannotSwim;
			tunic					= PlayerTunics.GreenTunic;
			moveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
			knockbackSpeed			= GameSettings.PLAYER_KNOCKBACK_SPEED;
			hurtKnockbackDuration	= GameSettings.PLAYER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration	= GameSettings.PLAYER_BUMP_KNOCKBACK_DURATION;

			// Physics
			Physics.CollisionBox			= new Rectangle2F(-4, -10 + 3, 8, 9);
			Physics.SoftCollisionBox		= new Rectangle2F(-6, -14 + 3, 12, 13);
			Physics.CollideWithWorld		= true;
			Physics.CollideWithEntities		= true;
			Physics.CollideWithRoomEdge		= true;
			Physics.CheckRadialCollisions	= true;
			Physics.RoomEdgeCollisionBoxType = CollisionBoxType.Soft;
			Physics.HasGravity				= true;
			Physics.AutoDodges				= true;
			Physics.MovesWithConveyors		= true;
			Physics.MovesWithPlatforms		= true;
			Physics.AllowEdgeClipping		= true;
			Physics.IsCrushable				= true;
			Physics.EdgeClipAmount			= 1;
			Physics.CrushMaxGapSize			= 4;
			Physics.CustomTileIsNotSolidCondition = delegate(Tile tile) {
				if (movement.IsOnColorBarrier && (tile is TileColorBarrier)) {
					return false;
				}
				return true;
			};

			// Graphics
			Graphics.DepthLayer			= DepthLayer.PlayerAndNPCs;
			Graphics.DepthLayerInAir	= DepthLayer.InAirPlayer;
			Graphics.DrawOffset			= new Point2I(-8, -13);

			// Tools
			toolShield	= new PlayerToolShield();
			toolSword	= new PlayerToolSword();
			toolVisual	= new PlayerToolVisual();
			
			// Movement
			movement = new PlayerMoveComponent(this);

			// State machines
			stateParameters			= new PlayerStateParameters();
			environmentStateMachine	= new PlayerStateMachine(this);
			controlStateMachine		= new PlayerStateMachine(this);
			weaponStateMachine		= new PlayerStateMachine(this);
			conditionStateMachines	= new List<PlayerStateMachine>();
			
			// Construct the basic player states
			stateBusy			= new PlayerBusyState();
			stateMagicBoomerang	= new PlayerMagicBoomerangState();

			// Weapon states
			stateSwingSword		= new PlayerSwingSwordState();
			stateSwingBigSword	= new PlayerSwingBigSwordState();
			stateSwingMagicRod	= new PlayerSwingMagicRodState();
			stateSwingCane		= new PlayerSwingCaneState();
			stateHoldSword		= new PlayerHoldSwordState();
			stateSwordStab		= new PlayerSwordStabState();
			stateSpinSword		= new PlayerSpinSwordState();
			stateSeedShooter	= new PlayerSeedShooterState();
			stateSwitchHook		= new PlayerSwitchHookState();
			stateGrab			= new PlayerGrabState();
			stateCarry			= new PlayerCarryState();

			// Control states
			stateLedgeJump			= new PlayerLedgeJumpState();
			stateLeapLedgeJump		= new PlayerLeapLedgeJumpState();
			statePullHandle			= new PlayerPullHandleState();
			stateRespawnDeath		= new PlayerRespawnDeathState();
			stateMinecart			= new PlayerMinecartState();
			stateJumpTo				= new PlayerJumpToState();
			stateSwitchHookSwitch	= new PlayerSwitchHookSwitchState();

			// Environment states
			environmentStateGrass				= new PlayerGrassEnvironmentState();
			environmentStateStairs				= new PlayerStairsEnvironmentState();
			environmentStateJump				= new PlayerJumpEnvironmentState();
			environmentStateIce					= new PlayerIceEnvironmentState();
			environmentStateSwim				= new PlayerSwimEnvironmentState();
			environmentStateUnderwater			= new PlayerUnderwaterEnvironmentState();
			environmentStateSideScrollSwim		= new PlayerSideScrollSwimEnvironmentState();
			environmentStateLadder				= new PlayerLadderEnvironmentState();
			environmentStateSideScrollLadder	= new PlayerSideScrollLadderEnvironmentState();
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		public void BeginConditionState(PlayerState state) {
			PlayerStateMachine stateMachine = new PlayerStateMachine(this);
			conditionStateMachines.Add(stateMachine);
			stateMachine.BeginState(state);
		}

		public void BeginWeaponState(PlayerState weaponState) {
			weaponStateMachine.BeginState(weaponState);
		}

		public void BeginControlState(PlayerState state) {
			controlStateMachine.BeginState(state);
		}

		public void BeginEnvironmentState(PlayerState state) {
			environmentStateMachine.BeginState(state);
		}

		// Return the player state that the player wants to be in based on his current position.
		public PlayerState GetDesiredNaturalState() {
			
			// Find the surface tile underneath the entity
			Tile feetTile = RoomControl.TileManager.GetSurfaceTileAtPosition(
				position, true);

			if (RoomControl.IsSideScrolling) {
				if (physics.IsInWater || RoomControl.IsUnderwater)
					return environmentStateSideScrollSwim;
				else if (physics.IsInLava)
					return environmentStateSwim; // TODO: this is here to trigger player death
				else if (physics.IsInAir)
					return environmentStateJump;
				else if (physics.IsInGrass)
					return environmentStateGrass;
				else if (physics.IsOnStairs)
					return environmentStateStairs;
				else if (physics.IsOnIce)
					return environmentStateIce;
				else 
					return null;
			}
			else {
				//if (RoomControl.IsUnderwater)
					//return stateUnderwater;
				if (physics.IsInAir)
					return environmentStateJump;
				else if (physics.IsInWater || physics.IsInLava)
					return environmentStateSwim;
				else if (physics.IsInGrass)
					return environmentStateGrass;
				else if (physics.IsOnStairs)
					return environmentStateStairs;
				else if (physics.IsOnLadder)
					return environmentStateLadder;
				else if (physics.IsOnIce)
					return environmentStateIce;
				else
					return null;
			}
		}

		// Begin the busy state with the specified duration.
		public void BeginBusyState(int duration) {
			BeginConditionState(new PlayerBusyState() {
				Duration			= duration,
				Animation			= Graphics.Animation,
				AnimationInMinecart	= Graphics.Animation,
			});
		}
		
		// Begin the busy state with the specified duration and animation(s).
		public void BeginBusyState(int duration, Animation animation, Animation animationInMinecart = null) {
			if (IsInMinecart && animationInMinecart != null)
				Graphics.PlayAnimation(animationInMinecart);
			else
				Graphics.PlayAnimation(animation);
			
			BeginConditionState(new PlayerBusyState() {
				Duration			= duration,
				Animation			= animation,
				AnimationInMinecart	= animationInMinecart,
			});
		}

		// Jump to the given position using the special jump state.
		public void JumpToPosition(Vector2F destinationPosition,
			float destinationZPosition, int duration,
			Action<PlayerJumpToState> endAction)
		{
			stateJumpTo.JumpDuration			= duration;
			stateJumpTo.DestinationPosition		= destinationPosition;
			stateJumpTo.DestinationZPosition	= destinationZPosition;
			stateJumpTo.EndAction				= endAction;
			BeginControlState(stateJumpTo);
		}

		// Hop into a minecart.
		public void JumpIntoMinecart(Minecart minecart) {
			JumpToPosition(minecart.Center, 4.0f, 26, delegate(PlayerJumpToState state) {
				stateMinecart.Minecart = minecart;
				BeginControlState(stateMinecart);
			});
		}

		// Hop out of a minecart and land at the given tile location.
		public void JumpOutOfMinecart(Point2I landingTileLocation) {
			Vector2F landingPoint = (landingTileLocation +
				new Vector2F(0.5f, 0.5f)) * GameSettings.TILE_SIZE;
			landingPoint -= centerOffset + new Vector2F(0, 2.0f);
			JumpToPosition(landingPoint, 0.0f, 26, null);
		}

		public void OnJump() {
			if (eventJump != null)
				eventJump(this);
		}


		//-----------------------------------------------------------------------------
		// Interactions
		//-----------------------------------------------------------------------------
		
		// Land on the given surface, breaking any obstructions.
		// This is used for when the player begins colliding with the world again in a new position.
		public void LandOnSurface() {
			// Break tiles in the way, not if on a color barrier.
			foreach (Tile tile in Physics.GetTilesMeeting(position, CollisionBoxType.Hard)) {
				if (tile.IsSolid) {
					if (tile.IsBreakable)
						tile.Break(false);
					else if (tile is TileColorBarrier)
						movement.IsOnColorBarrier = true;
				}
			}
		}

		// For when the player needs to stop pushing, such as when reading text or opening a chest.
		public void StopPushing() {
			movement.IsMoving = false;
			movement.StopMotion();
			movement.ChooseAnimation();
		}

		public void Freeze() {
			isFrozen = true;
			Movement.StopMotion();
			Graphics.PauseAnimation();
		}

		public void Unfreeze() {
			isFrozen = false;
			Graphics.ResumeAnimation();
		}

		public void RespawnDeath() {
			stateRespawnDeath.WaitForAnimation = true;
			BeginControlState(stateRespawnDeath);
		}

		public void RespawnDeathInstantaneous() {
			stateRespawnDeath.WaitForAnimation = false;
			BeginControlState(stateRespawnDeath);
		}

		// Mark the player's current position/direction as where he should respawn.
		public void MarkRespawn() {
			respawnPosition		= position;
			respawnDirection	= direction;
		}

		// Move the player to his marked spawn position in the room.
		public void Respawn() {
			position	= respawnPosition;
			direction	= respawnDirection;
			LandOnSurface(); // Break any breakable blocks the player respawns and collides with.

			// If colliding with a door, then move forward one tile.
			foreach (Tile tile in Physics.GetTilesMeeting(position, CollisionBoxType.Hard)) {
				if ((tile is TileDoor) && tile.IsSolid) {
					SetPositionByCenter(tile.Center + Directions.ToVector(((TileDoor) tile).Direction) * GameSettings.TILE_SIZE);
					break;
				}
			}

			RoomControl.OnPlayerRespawn();
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		public void InterruptWeapons() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				ItemWeapon item = EquippedUsableItems[i];
				if (item != null) {
					item.Interrupt();
					if (item.IsTwoHanded)
						break;
				}
			}
			if (weaponStateMachine.IsActive)
				weaponStateMachine.CurrentState.End();
		}

		// Update items by checking if their buttons are pressed.
		private void UpdateEquippedItems() {
			for (int i = 0; i < EquippedUsableItems.Length; i++) {
				ItemWeapon item = EquippedUsableItems[i];
				if (item != null && item.IsUsable()) {
					if (Inventory.GetSlotButton(i).IsPressed())
						item.OnButtonPress();
					if (Inventory.GetSlotButton(i).IsDown())
						item.OnButtonDown();
					if (!item.IsTwoHanded || i == 1)
						item.Update();
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
			if (movement.IsMoving && !StateParameters.EnableStrafing) {
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
			if (AutoRoomTransition)
				return true;
			if (movement.MoveCondition != PlayerMoveCondition.FreeMovement || !movement.IsMoving)
				return false;
			return (Controls.GetArrowControl(transitionDirection).IsDown() || Controls.GetAnalogDirection(transitionDirection));
		}
		
		private bool IsOnHazardTile() {
			return	physics.IsInHole ||
					(physics.IsInWater && !swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInWater)) ||
					(physics.IsInOcean && !swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInOcean)) ||
					(physics.IsInLava  && !swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInLava));
		}

		// Custom collision function for colliding with room edges.
		public void CheckRoomTransitions() {
			if (StateParameters.ProhibitRoomTransitions || IsOnHazardTile())
				return;

			// Check for room edge collisions.
			int transitionDirection = -1;
			foreach (CollisionInfo info in Physics.GetCollisions()) {
				if (info.Type == CollisionType.RoomEdge && CanRoomTransition(info.Direction)) {
					transitionDirection = info.Direction;
					break;
				}
			}
			// Request a transition on the room edge.
			if (transitionDirection >= 0) {
				physics.Velocity = Vector2F.Zero;
				RoomControl.RequestRoomTransition(transitionDirection);
			}
		}

		// Check for tile & entity press interactions.
		private void CheckPressInteractions() {
			// TODO: interactions not allowed when:
			//  - In PlayerGrabState or PlayerCarryState
			//  - In carry state

			if (IsOnGround && Controls.A.IsPressed()) {
				// TEMPORARY: Can't do actions while grabbing or carring.
				if (WeaponState == stateGrab || WeaponState == stateCarry)
					return;

				// First check entity interactions.
				for (int i = 0; i < RoomControl.EntityCount; i++) {
					Entity e = RoomControl.Entities[i];
					if (e != this && !e.IsDestroyed && e.Physics.IsSolid && Physics.IsSoftMeetingEntity(e) &&
						Entity.AreEntitiesAligned(this, e, direction, e.ActionAlignDistance) &&
						e.OnPlayerAction(direction))
					{
						Controls.A.Disable(true);
						StopPushing();
					}
				}

				// Then check tile interactions.
				Tile actionTile = physics.GetFacingSolidTile(direction);
				if (actionTile != null && actionTile.OnAction(direction)) {
					Controls.A.Disable(true);
					StopPushing();
				}
			}
		}

		// Try to switch to a natural state.
		public void RequestNaturalState() {
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			environmentStateMachine.BeginState(desiredNaturalState);
			IntegrateStateParameters();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			
			viewFocusOffset		= Vector2F.Zero;
			direction			= Directions.Down;
			useDirection		= 0;
			useAngle			= 0;
			autoRoomTransition	= false;
			isStateControlled	= false;
			syncAnimationWithDirection = true;
			isFrozen			= false;

			// Initialize tools
			toolShield.Initialize(this);
			toolSword.Initialize(this);
			toolVisual.Initialize(this);
			
			stateParameters = new PlayerStateParameters();
		}

		public override void OnEnterRoom() {
			// Update information about the tile we are standing on
			Physics.TopTile = RoomControl.TileManager
				.GetSurfaceTileAtPosition(position, Physics.MovesWithPlatforms);

			// Notify the state
			foreach (PlayerState state in ActiveStates)
				state.OnEnterRoom();

			// In side-scroll mode, the player samples the top-tile from his center
			// postion opposed to his origin position
			if (RoomControl.IsSideScrolling)
				Physics.TopTilePointOffset = CenterOffset;
			else
				Physics.TopTilePointOffset = Point2I.Zero;
		}

		public override void OnLeaveRoom() {
			// Notify the state
			foreach (PlayerState state in ActiveStates)
				state.OnLeaveRoom();

			// Clear events
			eventJump = null;
			eventLand = null;
		}

		public override void Knockback(int duration, float speed, Vector2F sourcePosition) {
			// Don't apply knockback when doomed to fall in a hole
			if (!movement.IsDoomedToFallInHole)
				base.Knockback(duration, speed, sourcePosition);
		}

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_HURT);
			foreach (PlayerState state in ActiveStates)
				state.OnHurt(damage);
		}

		public override void OnCrush(bool horizontal) {
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
			RespawnDeath();
			if (horizontal)
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_HORIZONTAL);
			else
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_VERTICAL);
		}

		public override void Die() {
			// Don't actually die.
		}

		public override void OnBeginFalling() {
			base.OnBeginFalling();

			if (RoomControl.IsSideScrolling) {
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			}
		}

		public override void OnLand() {
			base.OnLand();

			// Notify the tile we landed on.
			Tile tile = RoomControl.GetTopTile(RoomControl.GetTileLocation(Position));
			if (tile != null)
				tile.OnLand(movement.JumpStartTile);
			movement.JumpStartTile = -Point2I.One;

			eventLand?.Invoke(this);

			Physics.Gravity = GameSettings.DEFAULT_GRAVITY;

			if (RoomControl.DeathOutOfBounds && Position.Y + 3 >= RoomControl.RoomBounds.Bottom) {
				RespawnDeathInstantaneous();
			}
			else {
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_LAND);
			}
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();
			
			// Sync the graphics color with the current tunic.
			switch (tunic) {
			case PlayerTunics.GreenTunic:
				Graphics.ColorDefinitions.SetAll("green");
				break;
			case PlayerTunics.RedTunic:
				Graphics.ColorDefinitions.SetAll("red");
				break;
			case PlayerTunics.BlueTunic:
				Graphics.ColorDefinitions.SetAll("blue");
				break;
			}
		}

		public void IntegrateStateParameters() {
			// Combine all state parameters from each active state
			stateParameters = new PlayerStateParameters();
			foreach (PlayerState state in ActiveStates)
				stateParameters |= state.StateParameters;

			// Integrate the combined state parameters

			if (stateParameters.ProhibitMovementControlInAir &&
				stateParameters.ProhibitMovementControlOnGround)
				movement.MoveCondition = PlayerMoveCondition.NoControl;
			else if (stateParameters.ProhibitMovementControlOnGround)
				movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			else
				movement.MoveCondition = PlayerMoveCondition.FreeMovement;

			movement.CanJump				= !stateParameters.ProhibitJumping;
			movement.CanLedgeJump			= !stateParameters.ProhibitLedgeJumping;
			movement.CanPush				= !stateParameters.ProhibitPushing;
			autoRoomTransition				= stateParameters.EnableAutomaticRoomTransitions;

			// TODO: Check if movement is disabled

			physics.HasGravity				= !stateParameters.DisableGravity;
			physics.OnGroundOverride		= stateParameters.EnableGroundOverride;
			physics.CollideWithWorld		= !stateParameters.DisableSolidCollisions;
			physics.CollideWithEntities		= !stateParameters.DisableSolidCollisions;

			IsPassable						= stateParameters.DisableInteractionCollisions;
		}

		private void UpdateStates() {
			IntegrateStateParameters();

			// Update depricated state
			// TODO: remove when ready
			//state.Update();

			// Update depricated special state (minecart state)
			// TODO: remove when ready
			//if (specialState != null && specialState.IsActive)
				//specialState.Update();
				
			if (IsOnGround &&
				movement.MoveCondition == PlayerMoveCondition.FreeMovement)
			{
				Graphics.SetAnimation(MoveAnimation);
			}

			// Update the weapon state
			weaponStateMachine.Update();

			// Update the environment state
			environmentStateMachine.Update();
			RequestNaturalState();

			// Update the control state
			controlStateMachine.Update();

			// Update the condition states
			for (int i = 0; i < conditionStateMachines.Count; i++) {
				conditionStateMachines[i].Update();
				if (!conditionStateMachines[i].IsActive)
					conditionStateMachines.RemoveAt(i--);
			}

			IntegrateStateParameters();
		}

		public override void Update() {
			if (!isFrozen) {

				// Pre-state update.
				if (!isStateControlled) {
					RequestNaturalState();
					movement.Update();
					UpdateUseDirections();
					CheckPressInteractions();
					RequestNaturalState();
				}
			
				// Update the current player states.
				UpdateStates();

				// Post-state update.
				if (!isStateControlled) {
					UpdateEquippedItems();
				}

				// Handle SHIELD holding.
				if (Graphics.Animation == GameData.ANIM_PLAYER_SHIELD_BLOCK ||
					Graphics.Animation == GameData.ANIM_PLAYER_SHIELD_LARGE_BLOCK)
				{
					EquipTool(toolShield);
				}
				else if (toolShield.IsEquipped) {
					UnequipTool(toolShield);
				}

				// Update superclass.
				base.Update();
			}
		}

		public override void Draw(RoomGraphics g) {
			foreach (PlayerState state in ActiveStates)
				state.DrawUnder(g);

			base.Draw(g);

			foreach (PlayerState state in ActiveStates)
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

		public bool IsStateControlled {
			get { return isStateControlled; }
			set { isStateControlled = value; }
		}

		public float PushSpeed {
			get {
				if (Inventory.IsItemObtained("item_bracelet")) {
					int braceletLevel = Inventory.GetItem("item_bracelet").Level;
					return GameSettings.BRACELET_PUSH_SPEEDS[braceletLevel];
				}
				return GameSettings.PLAYER_DEFAULT_PUSH_SPEED;
			}
		}

		public Vector2F ViewFocusOffset {
			get { return viewFocusOffset; }
			set { viewFocusOffset = value; }
		}

		public bool IsInMinecart {
			get { return (stateMinecart.IsActive); }
		}

		public bool IsUnderwater {
			get { return RoomControl.IsUnderwater; }
		}

		// Events.

		public event PlayerDelegate EventJump {
			add { eventJump += value; }
			remove { eventJump -= value; }
		}

		public event PlayerDelegate EventLand {
			add { eventLand += value; }
			remove { eventLand -= value; }
		}
		
		// Player states

		public PlayerStateParameters StateParameters {
			get { return stateParameters; }
		}

		public IEnumerable<PlayerState> ActiveStates {
			get {
				if (controlStateMachine.IsActive)
					yield return controlStateMachine.CurrentState;
				if (weaponStateMachine.IsActive)
					yield return weaponStateMachine.CurrentState;
				if (environmentStateMachine.IsActive)
					yield return environmentStateMachine.CurrentState;
				foreach (PlayerStateMachine stateMachine in conditionStateMachines) {
					if (stateMachine.IsActive)
						yield return stateMachine.CurrentState;
				}
			}
		}

		public PlayerEnvironmentState EnvironmentState {
			get { return environmentStateMachine.CurrentState as PlayerEnvironmentState; }
		}

		public PlayerState ControlState {
			get { return controlStateMachine.CurrentState; }
		}

		public PlayerState WeaponState {
			get { return weaponStateMachine.CurrentState; }
		}

		public IEnumerable<PlayerState> ConditionStates {
			get {
				foreach (PlayerStateMachine stateMachine in conditionStateMachines) {
					if (stateMachine.IsActive)
						yield return stateMachine.CurrentState;
				}
			}
		}

		public PlayerBusyState BusyState {
			get { return stateBusy; }
		}

		public PlayerSwimEnvironmentState SwimState {
			get { return environmentStateSwim; }
		}

		public PlayerSideScrollSwimEnvironmentState SideScrollSwimState {
			get { return environmentStateSideScrollSwim; }
		}

		public PlayerUnderwaterEnvironmentState UnderwaterState {
			get { return environmentStateUnderwater; }
		}

		public PlayerLedgeJumpState LedgeJumpState {
			get { return stateLedgeJump; }
		}

		public PlayerLeapLedgeJumpState LeapLedgeJumpState {
			get { return stateLeapLedgeJump; }
		}

		public PlayerLadderEnvironmentState LadderState {
			get { return environmentStateLadder; }
		}

		public PlayerSideScrollLadderEnvironmentState SideScrollLadderState {
			get { return environmentStateSideScrollLadder; }
		}

		public bool IsOnSideScrollLadder {
			get { return (EnvironmentState == environmentStateSideScrollLadder); }
		}

		public PlayerSwingSwordState SwingSwordState {
			get { return stateSwingSword; }
		}

		public PlayerSwingBigSwordState SwingBigSwordState {
			get { return stateSwingBigSword; }
		}

		public PlayerSwingCaneState SwingCaneState {
			get { return stateSwingCane; }
		}

		public PlayerSwingMagicRodState SwingMagicRodState {
			get { return stateSwingMagicRod; }
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

		public PlayerSeedShooterState SeedShooterState {
			get { return stateSeedShooter; }
		}

		public PlayerSwitchHookState SwitchHookState {
			get { return stateSwitchHook; }
		}

		public PlayerSwitchHookSwitchState SwitchHookSwitchState {
			get { return stateSwitchHookSwitch; }
		}

		public PlayerMagicBoomerangState MagicBoomerangState {
			get { return stateMagicBoomerang; }
		}

		public PlayerCarryState CarryState {
			get { return stateCarry; }
		}

		public PlayerGrabState GrabState {
			get { return stateGrab; }
		}

		public PlayerPullHandleState PullHandleState {
			get { return statePullHandle; }
		}

		public PlayerRespawnDeathState RespawnDeathState {
			get { return stateRespawnDeath; }
		}

		public PlayerMinecartState MinecartState {
			get { return stateMinecart; }
		}

		public PlayerJumpToState JumpToState {
			get { return stateJumpTo; }
		}

		public PlayerJumpEnvironmentState JumpState {
			get { return environmentStateJump; }
		}

		public bool IsSwimmingUnderwater {
			get {
				return (EnvironmentState == environmentStateUnderwater |
					EnvironmentState == environmentStateSideScrollSwim);
			}
		}

		public bool IsSwimming {
			get {
				return (EnvironmentState == environmentStateSwim ||
					EnvironmentState == environmentStateUnderwater ||
					EnvironmentState == environmentStateSideScrollSwim);
			}
		}

		// Tools

		public PlayerToolSword ToolSword {
			get { return toolSword; }
		}
		
		public PlayerToolShield ToolShield {
			get { return toolShield; }
		}
		
		public PlayerToolVisual ToolVisual {
			get { return toolVisual; }
		}

		public bool IsFrozen {
			get { return isFrozen; }
			set { isFrozen = value; }
		}
	}
}
