using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {

	public class MonsterZolGreen : JumpMonster {
		
		private enum BurrowState {
			Burrowed,
			Burrowing,
			Unburrowing,
			Unburrowed,
		}

		private int jumpIndex;
		private int numJumps;
		private int burrowTimer;
		private int unburrowDelay;
		private BurrowState burrowState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterZolGreen() {
			color			= MonsterColor.Green;
			MaxHealth		= 1;
			ContactDamage	= 1;

			moveSpeed		= 0.75f;
			jumpSpeed		= new RangeF(2.0f);
			stopTime		= new RangeI(48);
			stopAnimation	= GameData.ANIM_MONSTER_ZOL;
			jumpAnimation	= GameData.ANIM_MONSTER_ZOL_JUMP;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void Burrow() {
			IsPassable	= true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL_BURROW);
			burrowTimer = 0;
			burrowState = BurrowState.Burrowing;
		}
		
		private void Unburrow() {
			jumpIndex	= 0;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL_UNBURROW);
			Graphics.IsVisible = true;
			burrowState = BurrowState.Unburrowing;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			jumpIndex = 0;

			// Start burrowed
			numJumps			= GameSettings.MONSTER_ZOL_GREEN_JUMP_COUNT;
			unburrowDelay		= GameSettings.MONSTER_ZOL_GREEN_UNBURROW_DELAY;
			IsPassable			= true;
			burrowState			= BurrowState.Burrowed;
			Graphics.IsVisible	= false;
		}
		public override void OnJump() {
			base.OnJump();
			jumpIndex++;
		}

		public override void OnEndJump() {
			base.OnEndJump();
			
			// Become vulnerable upon landing the first jump after unburrowing
			isPassable = false;

			// Re-burrow after a certain number of jumps
			if (jumpIndex > numJumps)
				Burrow();
		}

		public override void UpdateAI() {
			if (burrowState == BurrowState.Burrowing) {
				if (Graphics.IsAnimationDone) {
					Graphics.IsVisible = false;
					burrowState = BurrowState.Burrowed;
				}
			}
			else if (burrowState == BurrowState.Unburrowing) {
				if (Graphics.IsAnimationDone) {
					Jump();
					Physics.Velocity = Vector2F.Zero;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL);
					Graphics.PauseAnimation();
					burrowState = BurrowState.Unburrowed;
				}
			}
			else if (burrowState == BurrowState.Burrowed) {
				burrowTimer++;
				if (burrowTimer > unburrowDelay) {
					float distanceToPlayer = Vector2F.Distance(RoomControl.Player.Center, Center);
					if (distanceToPlayer < GameSettings.MONSTER_ZOL_GREEN_UNBURROW_RANGE)
						Unburrow();
				}
			}
			else {
				// Update jumping behavior
				base.UpdateAI();
			}
		}
	}
}
