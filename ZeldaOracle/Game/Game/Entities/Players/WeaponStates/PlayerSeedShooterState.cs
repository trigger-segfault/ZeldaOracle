using System.Linq;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSeedShooterState : PlayerState {

		private enum SubState {
			Aiming,
			Shooting,
		}

		/// <summary>The angle that the player is aiming in.</summary>
		private Angle aimAngle;
		/// <summary>The direction the player should return to after shooting.
		/// </summary>
		private Direction returnDirection;
		/// <summary>The seed shooter weapon item.</summary>
		private ItemSeedShooter weapon;

		private int autoRotateTimer;

		private int shootTimer;

		private GenericStateMachine<SubState> subStateMachine;

		private Entity seedShooter;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSeedShooterState() {
			// State Parameters
			StateParameters.ProhibitMovementControlOnGround = true;

			// Sub-state Machine
			subStateMachine = new GenericStateMachine<SubState>();
			subStateMachine.AddState(SubState.Aiming)
				.OnBegin(OnBeginAimingState)
				.OnUpdate(OnUpdateAimingState);
			subStateMachine.AddState(SubState.Shooting)
				.OnBegin(OnBeginShootingState)
				.OnUpdate(OnUpdateShootingState)
				.OnEnd(OnEndShootingState);
		}


		//-----------------------------------------------------------------------------
		// Sub State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginAimingState() {
			autoRotateTimer	= 0;
		}

		private void OnUpdateAimingState() {
			// Get the desired aim angle
			Angle goalAngle = player.UseAngle;

			// Determine the direction of rotation to get to the goal angle
			WindingOrder rotateDirection = WindingOrder.Clockwise;
			if (goalAngle != aimAngle.Reverse())
				aimAngle.NearestDistanceTo(goalAngle, out rotateDirection);

			bool rotate = false;

			// Check if the player should rotate this frame
			if (Controls.Arrows.Any(c => c.IsPressed())) {
				// Pressing an arrow control will instantly rotate
				autoRotateTimer = 0;
				rotate = true;
			}
			else if (Controls.Arrows.Any(c => c.IsDown())) {
				// Holding down an arrow control will automatically rotate every
				// 16 frames
				autoRotateTimer++;
				if (autoRotateTimer >= GameSettings.SEED_SHOOTER_AUTO_ROTATE_DELAY) {
					rotate = true;
					autoRotateTimer = 0;
				}
			}

			if (rotate && aimAngle != goalAngle) {
				// Rotate the player's aim angle once
				aimAngle = aimAngle.Rotate(1, rotateDirection);

				// Remember the last axis-aligned direction
				if (aimAngle.IsAxisAligned)
					returnDirection = aimAngle.ToDirection();
			}

			// Attempt to shoot a seed when the button is released
			if (!weapon.IsEquipped || !weapon.IsButtonDown()) {
				if (!weapon.SeedTracker.IsMaxedOut && weapon.HasAmmo())
					subStateMachine.BeginState(SubState.Shooting);
				else
					End();
			}
		}

		private void OnBeginShootingState() {
			Vector2F[] projectilePositions = new Vector2F[] {
				new Vector2F(16, 7),
				new Vector2F(15, -2),
				new Vector2F(0, -12),
				new Vector2F(-4, -6),
				new Vector2F(-9, 7),
				new Vector2F(-4, 12),
				new Vector2F(7, 15),
				new Vector2F(15, 11)
			};

			// Spawn the seed projectile
			SeedProjectile seed = new SeedProjectile(weapon.CurrentSeedType, true);
			Vector2F spawnOffset = projectilePositions[aimAngle] +
				new Vector2F(4, 11) - new Vector2F(8, 8);
			int spawnZPosition = 0;

			if (!player.RoomControl.IsSideScrolling) {
				spawnZPosition = 5;
			}
			else {
				spawnOffset.Y -= 5.0f;
				spawnZPosition = 0;
			}

			player.ShootFromAngle(seed, aimAngle,
				GameSettings.SEED_SHOOTER_SHOOT_SPEED,
				spawnOffset, spawnZPosition);
			weapon.SeedTracker.TrackEntity(seed);
			weapon.UseAmmo();
				
			AudioSystem.PlaySound(GameData.SOUND_SEED_SHOOTER);

			shootTimer = 0;
		}

		private void OnUpdateShootingState() {
			shootTimer++;
			if (shootTimer >= GameSettings.SEED_SHOOTER_SHOOT_PAUSE_DURATION)
				subStateMachine.EndCurrentState();
		}

		private void OnEndShootingState() {
			player.Direction = returnDirection;
			End();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			aimAngle = player.UseAngle;
			returnDirection	= player.UseDirection;

			// Begin the aiming sub-state
			subStateMachine.InitializeOnState(SubState.Aiming);
			
			// Play the player's aim animation
			player.Graphics.SubStripIndex = aimAngle.Index;
			player.Graphics.PlayAnimation(
				player.StateParameters.PlayerAnimations.Aim);

			// This state will select the player's substrip index based on the aim
			// angle
			player.SyncAnimationWithDirection = false;
			
			// Equip the seed shooter tool (purely visual)
			seedShooter = new Entity();
			seedShooter.Graphics.PlayAnimation(GameData.ANIM_SEED_SHOOTER);
			seedShooter.Graphics.DepthLayer = DepthLayer.ItemSeedShooter;
			seedShooter.AttachmentOffset = player.CenterOffset;
			player.AttachEntity(seedShooter, player.CenterOffset);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.SyncAnimationWithDirection = true;
			seedShooter.Destroy();
		}

		public override void Update() {
			// Update the current sub-state
			subStateMachine.Update();
			
			// Play the player's aim animation
			player.Graphics.SetAnimation(
				player.StateParameters.PlayerAnimations.Aim);

			// Make player and seed-shooter face the aim angle
			player.Graphics.SubStripIndex = aimAngle.Index;
			seedShooter.Graphics.SubStripIndex = aimAngle;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemSeedShooter Weapon {
			get { return weapon; }
			set { weapon = value; }
		}
	}
}
