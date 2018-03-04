using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Projectiles {

	public class Boomerang : Projectile {

		private enum BoomerangState {
			Moving,
			Returning,
		}

		protected float moveSpeed;
		protected int returnDelay;

		private int returnTimer;
		private GenericStateMachine<BoomerangState> stateMachine;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Boomerang() {			
			// Graphics
			Graphics.DepthLayer			= DepthLayer.ProjectileBoomerang;
			Graphics.IsShadowVisible	= false;

			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 2);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.CollideRoomEdge);

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 2);
			
			// Boomerang
			moveSpeed = 1.5f;
			returnDelay = 40;
			stateMachine = new GenericStateMachine<BoomerangState>();
			stateMachine.AddState(BoomerangState.Moving)
				.OnBegin(OnBeginMovingState)
				.OnUpdate(OnUpdateMovingState);
			stateMachine.AddState(BoomerangState.Returning)
				.OnBegin(OnBeginReturningState)
				.OnUpdate(OnUpdateReturningState);
		}


		//-----------------------------------------------------------------------------
		// Boomerang Methods
		//-----------------------------------------------------------------------------

		public void BeginReturning() {
			if (stateMachine.CurrentState != BoomerangState.Returning)
				stateMachine.BeginState(BoomerangState.Returning);
		}

		// Occurs when the boomerang has returned to its owner.
		protected virtual void OnReturnedToOwner() { }
		

		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		protected virtual void OnBeginMovingState() {
			returnTimer = 0;
		}

		protected virtual void OnUpdateMovingState() {
			returnTimer++;

			// Begin returning after a delay
			if (returnTimer > returnDelay)
				stateMachine.BeginState(BoomerangState.Returning);
		}

		protected virtual void OnBeginReturningState() {
			physics.CollideWithWorld	= false;
			physics.CollideWithRoomEdge	= false;
		}

		protected virtual void OnUpdateReturningState() {
			// Move toward owner
			Vector2F trajectory = owner.Center - Center;
			if (trajectory.Length <= moveSpeed) {
				OnReturnedToOwner();
				Destroy();
			}
			else {
				physics.Velocity = trajectory.Normalized * moveSpeed;
			}
		}



		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			stateMachine.InitializeOnState(BoomerangState.Moving);
			physics.Velocity = angle.ToVector(moveSpeed);
		}

		public override bool Intercept() {
			if (stateMachine.CurrentState != BoomerangState.Returning) {
				stateMachine.BeginState(BoomerangState.Returning);
				return true;
			}
			return false;
		}

		public override void OnCollideSolid(Collision collision) {
			if (collision.IsTile) {
				// Spawn cling effect
				RoomControl.SpawnEntity(new EffectCling(), position, zPosition);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
				BeginReturning();
			}

			BeginReturning();
		}

		public override void Update() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_BOOMERANG_LOOP);

			stateMachine.Update();

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsReturning {
			get { return (stateMachine.CurrentState == BoomerangState.Returning); }
		}

		public float Speed {
			get { return moveSpeed; }
		}
	}
}
