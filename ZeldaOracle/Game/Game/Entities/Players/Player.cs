using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Players.States.SwingStates;
using ZeldaOracle.Game.Entities.Players.Tools;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Unit {

		// The current direction that the player wants to face to use items.
		private int useDirection;
		// The current angle that the player wants face to use items.
		private int useAngle;
		// The player doesn't need to be moving to transition.
		private bool autoRoomTransition;
		// The position the player was at when he entered the room.
		private Vector2F respawnPosition;
		private int respawnDirection;
		// The current player state.
		private PlayerState state;
		// The previous player state.
		private PlayerState previousState;
		// The current player state.
		private PlayerState specialState;
		// The previous player state.
		private PlayerState previousSpecialState;
		// The movement component for the player.
		private PlayerMoveComponent movement;

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
		private PlayerNormalState			stateNormal;
		private PlayerBusyState				stateBusy;
		private PlayerSwimState				stateSwim;
		private PlayerLedgeJumpState		stateLedgeJump;
		private PlayerLadderState			stateLadder;
		private PlayerSwingSwordState		stateSwingSword;
		private PlayerSwingBigSwordState	stateSwingBigSword;
		private PlayerSwingCaneState		stateSwingCane;
		private PlayerSwingMagicRodState	stateSwingMagicRod;
		private PlayerHoldSwordState		stateHoldSword;
		private PlayerSwordStabState		stateSwordStab;
		private PlayerSpinSwordState		stateSpinSword;
		private PlayerSeedShooterState		stateSeedShooter;
		private PlayerSwitchHookState		stateSwitchHook;
		private PlayerMagicBoomerangState	stateMagicBoomerang;
		private PlayerGrabState				stateGrab;
		private PlayerCarryState			stateCarry;
		private PlayerPullHandleState		statePullHandle;
		private PlayerRespawnDeathState		stateRespawnDeath;
		private PlayerMinecartState			stateMinecart;
		private PlayerJumpToState			stateJumpTo;


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
			movement = new PlayerMoveComponent(this);

			// Unit properties.
			centerOffset			= new Point2I(0, -5);
			Health					= 4 * 3;
			MaxHealth				= 4 * 3;
			swimmingSkills			= PlayerSwimmingSkills.CantSwim;
			tunic					= PlayerTunics.GreenTunic;
			moveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
			knockbackSpeed			= GameSettings.PLAYER_KNOCKBACK_SPEED;
			hurtKnockbackDuration	= GameSettings.PLAYER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration	= GameSettings.PLAYER_BUMP_KNOCKBACK_DURATION;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-4, -10 + 3, 8, 9);
			Physics.SoftCollisionBox	= new Rectangle2F(-6, -14 + 3, 12, 13);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithEntities	= true;
			Physics.HasGravity			= true;
			Physics.AutoDodges			= true;
			Physics.MovesWithConveyors	= true;
			Physics.MovesWithPlatforms	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.AllowEdgeClipping	= true;
			Physics.IsCrushable			= true;
			Physics.EdgeClipAmount		= 3;
			Physics.CrushMaxGapSize		= 4;
			Physics.RoomEdgeCollisionBoxType = CollisionBoxType.Soft;

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.PlayerAndNPCs;
			Graphics.DepthLayerInAir	= DepthLayer.InAirPlayer;
			Graphics.DrawOffset			= new Point2I(-8, -13);

			// Init tools.
			toolShield	= new PlayerToolShield();
			toolSword	= new PlayerToolSword();
			toolVisual	= new PlayerToolVisual();

			// Create the basic player states.
			stateNormal			= new PlayerNormalState();
			stateBusy			= new PlayerBusyState();
			stateSwim			= new PlayerSwimState();
			stateLadder			= new PlayerLadderState();
			stateLedgeJump		= new PlayerLedgeJumpState();
			stateSwingSword		= new PlayerSwingSwordState();
			stateSwingBigSword	= new PlayerSwingBigSwordState();
			stateSwingMagicRod	= new PlayerSwingMagicRodState();
			stateSwingCane		= new PlayerSwingCaneState();
			stateHoldSword		= new PlayerHoldSwordState();
			stateSwordStab		= new PlayerSwordStabState();
			stateSpinSword		= new PlayerSpinSwordState();
			stateSeedShooter	= new PlayerSeedShooterState();
			stateSwitchHook		= new PlayerSwitchHookState();
			stateMagicBoomerang	= new PlayerMagicBoomerangState();
			stateGrab			= new PlayerGrabState();
			stateCarry			= new PlayerCarryState();
			statePullHandle		= new PlayerPullHandleState();
			stateRespawnDeath	= new PlayerRespawnDeathState();
			stateMinecart		= new PlayerMinecartState();
			stateJumpTo			= new PlayerJumpToState();
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
			
			// Break any breakable blocks the player respawns and collides with.
			foreach (Tile tile in Physics.GetTilesMeeting(CollisionBoxType.Hard)) {
				if (tile.IsSolid && tile.Layer > 0)
					tile.Break(false);
			}
		}

		// Begin the given player state.
		public void BeginState(PlayerState newState) {
			if (state != newState) {
				if (state != null)
					state.End(newState);
				previousState = state;
				state = newState;
				state.Begin(this, previousState);
			}
		}

		// Begin the given player state as a special state.
		public void BeginSpecialState(PlayerState newState) {
			if (specialState != null && !specialState.IsActive)
				specialState = null;
			if (specialState != newState) {
				if (specialState != null)
					specialState.End(newState);
				previousSpecialState = specialState;
				specialState = newState;
				specialState.Begin(this, previousSpecialState);
			}
		}

		// Return the player state that the player wants to be in based on his current position.
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
			stateBusy.Duration				= duration;
			stateBusy.Animation				= Graphics.Animation;
			stateBusy.AnimationInMinecart	= Graphics.Animation;
			BeginState(stateBusy);
		}
		
		// Begin the busy state with the specified duration and animation(s).
		public void BeginBusyState(int duration, Animation animation, Animation animationInMinecart = null) {
			stateBusy.Duration				= duration;
			stateBusy.Animation				= animation;
			stateBusy.AnimationInMinecart	= animationInMinecart;
			if (IsInMinecart && animationInMinecart != null)
				Graphics.PlayAnimation(animationInMinecart);
			else
				Graphics.PlayAnimation(animation);
			BeginState(stateBusy);
		}

		// Begin the desired natural state.
		public void BeginNormalState() {
			BeginState(GetDesiredNaturalState());
		}

		// Jump to the given position using the special jump state.
		public void JumpToPosition(Vector2F destinationPosition, float destinationZPosition,
								   int duration, Action<PlayerJumpToState> endAction)
		{
			stateJumpTo.JumpDuration			= duration;
			stateJumpTo.DestinationPosition		= destinationPosition;
			stateJumpTo.DestinationZPosition	= destinationZPosition;
			stateJumpTo.EndAction				= endAction;
			BeginSpecialState(stateJumpTo);
		}

		// Hop into a minecart.
		public void JumpIntoMinecart(Minecart minecart) {
			JumpToPosition(minecart.Center, 4.0f, 26, delegate(PlayerJumpToState state) {
				stateMinecart.Minecart = minecart;
				BeginSpecialState(stateMinecart);
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

		// For when the player needs to stop pushing, such as when reading text or opening a chest.
		public void StopPushing() {
			if (state == stateNormal)
				stateNormal.StopPushing();
			movement.IsMoving = false;
			movement.StopMotion();
			movement.ChooseAnimation();
		}

		public void RespawnDeath() {
			BeginState(stateRespawnDeath);
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
		private void CheckRoomTransitions() {
			if (!AllowRoomTransition || IsOnHazardTile())
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
				if (state == stateGrab || state == stateCarry)
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
				Tile actionTile = physics.GetMeetingSolidTile(position, direction);
				if (actionTile != null && actionTile.OnAction(direction)) {
					Controls.A.Disable(true);
					StopPushing();
				}
			}
		}

		// Try to switch to a natural state.
		private void RequestNaturalState() {
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			if (state != desiredNaturalState && state.RequestStateChange(desiredNaturalState))
				BeginState(desiredNaturalState);
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
			
			// Initialize tools.
			toolShield.Initialize(this);
			toolSword.Initialize(this);
			toolVisual.Initialize(this);

			// Begin the default player state.
			BeginState(stateNormal);
			previousState	= stateNormal;
			specialState	= null;
			previousSpecialState = null;
		}

		public override void OnEnterRoom() {
			if (specialState != null && specialState.IsActive)
				stateMinecart.OnEnterRoom();
			state.OnEnterRoom();
		}

		public override void OnLeaveRoom() {
			if (specialState != null && specialState.IsActive)
				stateMinecart.OnLeaveRoom();
			state.OnLeaveRoom();

			// Clear events.
			eventJump = null;
			eventLand = null;
		}

		public override void Die() {
			// Don't actually die.
		}

		public override void OnCrush(bool horizontal) {
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
			RespawnDeath();
			if (horizontal)
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_HORIZONTAL);
			else
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_VERTICAL);
		}

		public override void OnLand() {
			base.OnLand();

			// Notify the tile we landed on.
			Tile tile = RoomControl.GetTopTile(RoomControl.GetTileLocation(Position));
			if (tile != null)
				tile.OnLand(movement.JumpStartTile);
			movement.JumpStartTile = -Point2I.One;
			
			if (eventLand != null)
				eventLand(this);

			Physics.Gravity = GameSettings.DEFAULT_GRAVITY;
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_LAND);
		}

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_HURT);
			state.OnHurt(damage);
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();
			
			// Sync the graphics image variant with the current tunic.
			switch (tunic) {
			case PlayerTunics.GreenTunic:	Graphics.ImageVariant = GameData.VARIANT_GREEN;	break;
			case PlayerTunics.RedTunic:		Graphics.ImageVariant = GameData.VARIANT_RED;	break;
			case PlayerTunics.BlueTunic:	Graphics.ImageVariant = GameData.VARIANT_BLUE;	break;
			}
		}

		public override void Update() {
			// Pre-state update.
			if (!isStateControlled) {
				movement.Update();
				UpdateUseDirections();
				CheckPressInteractions();
				RequestNaturalState();
			}
			
			// Update the current player states.
			state.Update();
			if (specialState != null && specialState.IsActive)
				specialState.Update();

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

			CheckRoomTransitions();
		}

		public override void Draw(RoomGraphics g) {
			state.DrawUnder(g);
			if (specialState != null && specialState.IsActive)
				specialState.DrawUnder(g);

			base.Draw(g);

			state.DrawOver(g);
			if (specialState != null && specialState.IsActive)
				specialState.DrawOver(g);
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
		
		public bool AllowRoomTransition {
			get { return movement.MoveMode.CanRoomChange; }
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

		// Tools.

		public PlayerToolSword ToolSword {
			get { return toolSword; }
		}
		
		public PlayerToolShield ToolShield {
			get { return toolShield; }
		}
		
		public PlayerToolVisual ToolVisual {
			get { return toolVisual; }
		}
	}
}
