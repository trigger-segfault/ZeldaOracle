using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Util;
using System;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class SwitchHookProjectile : Projectile {

		private enum SwitchHookState {
			Extending,
			Returning,
			Hooked,
			Lifting,
		}

		private float	speed;
		private int		length;
		private float	distance;
		private int		timer;
		private object	hookedObject;
		private Collectible collectible;
		private GenericStateMachine<SwitchHookState> stateMachine;
		private ItemSwitchHook weapon;
		private Vector2F startOffsetFromPlayer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwitchHookProjectile(ItemSwitchHook weapon) {
			// Graphics
			Graphics.DepthLayer			= DepthLayer.ProjectileSwitchHook;
			Graphics.IsShadowVisible	= false;

			// Physics
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.CollideRoomEdge |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable);

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(-2, -2, 4, 4);		
			Interactions.InteractionEventArgs = new WeaponInteractionEventArgs() {
				Weapon = weapon
			};	

			// Projectile
			syncAnimationWithDirection = true;

			// Switch Hook
			this.weapon = weapon;
			speed = GameSettings.PROJECTILE_SWITCH_HOOK_SPEEDS[weapon.Level];
			length = GameSettings.PROJECTILE_SWITCH_HOOK_LENGTHS[weapon.Level];
			stateMachine = new GenericStateMachine<SwitchHookState>();
			stateMachine.AddState(SwitchHookState.Extending)
				.OnBegin(OnBeginExtendingState)
				.OnEnd(OnEndExtendingState)
				.OnUpdate(OnUpdateExtendingState);
			stateMachine.AddState(SwitchHookState.Returning)
				.OnBegin(OnBeginReturningState)
				.OnEnd(OnEndReturningState)
				.OnUpdate(OnUpdateReturningState);
			stateMachine.AddState(SwitchHookState.Hooked)
				.OnBegin(OnBeginHookedState)
				.OnUpdate(OnUpdateHookedState);
			stateMachine.AddState(SwitchHookState.Lifting)
				.OnBegin(OnBeginLiftingState)
				.OnEnd(OnEndLiftingState)
				.OnUpdate(OnUpdateLiftingState);
		}


		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginExtendingState() {
			collectible = null;
			hookedObject = null;
			distance = 0;
			startOffsetFromPlayer = Vector2F.Zero;
			if (direction.IsHorizontal)
				startOffsetFromPlayer = new Vector2F(0, 3);
			position = RoomControl.Player.Center + startOffsetFromPlayer;
			physics.Velocity = direction.ToVector(speed);
			Interactions.InteractionType = InteractionType.SwitchHook;

			if (direction == Direction.Right)
				Physics.CollisionBox = new Rectangle2F(0, 0, 2, 1);
			else if (direction == Direction.Left)
				Physics.CollisionBox = new Rectangle2F(-2, 0, 2, 1);
			else if (direction == Direction.Up)
				Physics.CollisionBox = new Rectangle2F(0, -2, 1, 2);
			else if (direction == Direction.Down)
				Physics.CollisionBox = new Rectangle2F(0, 0, 1, 4);
		}

		private void OnEndExtendingState() {
			Interactions.InteractionType = InteractionType.None;
		}

		private void OnUpdateExtendingState() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_SWITCH_HOOK_LOOP);

			distance += speed;

			if (Physics.IsColliding) {
				Collision collision = Physics.GetCollisionInDirection(direction);

				// If colliding with a tile, then attempt to switch with it
				if (collision.Tile != null && collision.Tile.IsSwitchable) {
					hookedObject = collision.Tile;
					stateMachine.BeginState(SwitchHookState.Hooked);
				}
				else {
					stateMachine.BeginState(SwitchHookState.Returning);
				}
			
				// Spawn a cling effect
				if (collision.Tile != null) {
					Effect effect = new EffectCling();
					RoomControl.SpawnEntity(effect,
						position + direction.ToVector(5.0f), zPosition);
					AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
				}
			}
			else if (distance >= length) {
				// Return after extending to the maximum length
				stateMachine.BeginState(SwitchHookState.Returning);
			}
		}

		private void OnBeginReturningState() {
			hookedObject				= null;
			physics.CollideWithWorld	= false;
			physics.CollideWithRoomEdge	= false;
			Graphics.StopAnimation();
		}

		private void OnEndReturningState() {
			if (collectible != null) {
				collectible.Collect();
				collectible = null;
			}
			Destroy();
		}

		private void OnUpdateReturningState() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_SWITCH_HOOK_LOOP);

			// Move back toward the player
			Vector2F trajectory = (RoomControl.Player.Center +
				new Vector2F(0, 3)) - Center;
			if (trajectory.Length <= speed)
				stateMachine.EndCurrentState();
			else
				physics.Velocity = trajectory.Normalized * speed;
		}

		private void OnBeginHookedState() {
			timer = 0;
			graphics.PlayAnimation();
			Physics.Velocity = Vector2F.Zero;
		}

		private void OnUpdateHookedState() {
			// This is the state when latched on to on object, before lifting up
			timer++;
			if (timer >= GameSettings.SWITCH_HOOK_LATCH_DURATION) {
				// Start lifting if object is still alive
				if (IsHookedObjectAlive())
					stateMachine.BeginState(SwitchHookState.Lifting);
				else
					BeginReturn(true);
			}
		}

		private void OnBeginLiftingState() {
			// Notify the player's Switch hook state to begin switching
			RoomControl.Player.SwitchHookState.BeginSwitch(hookedObject);
			Graphics.ClearAnimation();
		}

		private void OnEndLiftingState() {
			// PlayerSwitchHookSwitchState will handle this functionality
		}

		private void OnUpdateLiftingState() {
			// PlayerSwitchHookSwitchState will handle this functionality
		}


		//-----------------------------------------------------------------------------
		// Hook Methods
		//-----------------------------------------------------------------------------

		/// <summary>Latch onto and switch with the given entity.</summary>
		public void SwitchWithEntity(Entity entity) {
			if (stateMachine.CurrentState == SwitchHookState.Extending) {
				hookedObject = entity;
				stateMachine.BeginState(SwitchHookState.Hooked);
			}
		}

		/// <summary>Begin returning back to the player.</summary>
		public void BeginReturn(bool grabbing) {
			if (stateMachine.CurrentState != SwitchHookState.Returning) {
				stateMachine.BeginState(SwitchHookState.Returning);

				// Set the animation to the hook closed sprite
				if (grabbing)
					Graphics.AnimationPlayer.PlaybackTime = 4; // TODO: magic number
			}
		}

		/// <summary>Returns true if the current hooked object is considered alive.
		/// </summary>
		public bool IsHookedObjectAlive() {
			if (hookedObject == null)
				return false;
			if (hookedObject is Entity)
				return ((Entity) hookedObject).IsAliveAndInRoom;
			if (hookedObject is Tile)
				return ((Tile) hookedObject).IsAlive;
			return true;
		}
		
		/// <summary>Grab a collectible, carrying it to be collected upon returning to
		/// the player.</summary>
		public void GrabCollectible(Collectible collectible) {
			this.collectible = collectible;
			collectible.Destroy();
			BeginReturn(true);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			Graphics.SetAnimation(GameData.ANIM_PROJECTILE_SWITCH_HOOK);
			stateMachine.InitializeOnState(SwitchHookState.Extending);
		}

		public override bool Intercept() {
			if (stateMachine.CurrentState != SwitchHookState.Returning) {
				stateMachine.BeginState(SwitchHookState.Returning);
				return true;
			}
			return false;
		}

		public override void Update() {
			stateMachine.Update();
			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			// Draw 3 links between hook and player (alternating which one is visible)
			Vector2F hookStartPos = RoomControl.Player.Center + startOffsetFromPlayer;
			int linkIndex = (GameControl.RoomTicks % 3) + 1;
			float percent = (linkIndex / 4.0f);
			Vector2F linkPos = Vector2F.Lerp(hookStartPos, position, percent);
			g.DrawSprite(GameData.SPR_SWITCH_HOOK_LINK,
				linkPos - new Vector2F(0, zPosition), Graphics.DepthLayer);

			// Draw the collectible over hook
			if (collectible != null) {
				Vector2F pos = Center + direction.ToVector(4.0f);
				collectible.SetPositionByCenter(pos);
				collectible.ZPosition = zPosition;
				collectible.Graphics.Draw(g);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemSwitchHook Weapon {
			get { return weapon; }
			set { weapon = value; }
		}

		public float Speed {
			get { return speed; }
		}
	}
}
