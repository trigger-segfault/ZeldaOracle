using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class JumpMonster : Monster {
		
		protected enum JumpMonsterState {
			Stopped,
			Crawling,
			PreparingJump,
			Jumping,
		}

		// Movement
		protected Animation	stopAnimation;
		protected Animation	jumpAnimation;
		protected Animation	prepareJumpAnimation;
		protected float		moveSpeed;
		protected float		crawlSpeed;
		protected RangeI	crawlTime;
		protected RangeI	stopTime;
		protected RangeI	moveTime;
		protected RangeI	prepareJumpTime;
		protected RangeF	jumpSpeed;
		protected bool		shakeDuringPrepareJump;
		protected int		jumpOdds;

		protected int		stopTimer;
		protected bool		isCrawling;
		protected Sound		jumpSound;
		protected JumpMonsterState jumpState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public JumpMonster() {
			// Unit
			syncAnimationWithDirection = false;

			// Monster
			Color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;

			// Jump monster
			jumpOdds				= 1;
			moveSpeed				= 1.0f;
			stopTime				= new RangeI(30, 60);
			jumpSpeed				= new RangeF(3, 3);
			prepareJumpTime			= new RangeI(0);
			shakeDuringPrepareJump	= true;
			stopAnimation			= GameData.ANIM_MONSTER_ZOL;
			jumpAnimation			= GameData.ANIM_MONSTER_ZOL_JUMP;
			prepareJumpAnimation	= null;
			jumpSound				= GameData.SOUND_MONSTER_JUMP;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnPrepareJump() {}

		public virtual void OnJump() {}
		
		public virtual void OnEndJump() {}

		public virtual void OnCrawl() {}

		public virtual void OnEndCrawl() {}


		//-----------------------------------------------------------------------------
		// Jump methods
		//-----------------------------------------------------------------------------

		public void PrepareJump() {
			jumpState = JumpMonsterState.PreparingJump;
			stopTimer = GRandom.NextInt(prepareJumpTime);
			Physics.Velocity = Vector2F.Zero;
			if (prepareJumpAnimation != null)
				Graphics.PlayAnimation(prepareJumpAnimation);
			OnPrepareJump();
		}

		public void Jump() {
			jumpState = JumpMonsterState.Jumping;

			// Jump towards the player
			Physics.ZVelocity = GRandom.NextFloat(jumpSpeed);
			Physics.Velocity = (RoomControl.Player.Center -
				Center).Normalized * moveSpeed;

			if (jumpAnimation != null)
				Graphics.PlayAnimation(jumpAnimation);
			if (jumpSound != null)
				AudioSystem.PlaySound(jumpSound);

			OnJump();
		}

		public void EndJump() {
			Physics.Velocity = Vector2F.Zero;
			jumpState = JumpMonsterState.Stopped;
			stopTimer = GRandom.NextInt(stopTime);
			if (stopAnimation != null)
				Graphics.PlayAnimation(stopAnimation);
			OnEndJump();
		}

		public void Crawl() {
			jumpState = JumpMonsterState.Crawling;
			stopTimer = GRandom.NextInt(crawlTime);

			// Crawl towards the player
			Physics.Velocity = (RoomControl.Player.Center -
				Center).Normalized * crawlSpeed;

			OnCrawl();
		}

		public void EndCrawl() {
			Physics.Velocity = Vector2F.Zero;
			jumpState = JumpMonsterState.Stopped;
			stopTimer = GRandom.NextInt(stopTime);
			OnEndCrawl();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			jumpState = JumpMonsterState.Stopped;

			stopTimer = GRandom.NextInt(stopTime);
			if (stopAnimation != null)
				Graphics.PlayAnimation(stopAnimation);
		}

		public override void UpdateAI() {
			if (IsOnGround) {
				if (jumpState == JumpMonsterState.Jumping) {
					EndJump();
				}
				else if (jumpState == JumpMonsterState.Crawling) {
					stopTimer--;
					if (stopTimer <= 0)
						EndCrawl();
				}
				else if (jumpState == JumpMonsterState.PreparingJump) {
					stopTimer--;
					
					// Shake horizontally by 1 pixel
					if (shakeDuringPrepareJump) {
						if (stopTimer % 8 == 0)
							position += new Vector2F(1, 0);
						else if (stopTimer % 8 == 4)
							position -= new Vector2F(1, 0);
					}

					if (stopTimer <= 0)
						Jump();
				}
				else {
					stopTimer--;
					if (stopTimer <= 0) {
						// Either jump or crawl
						if (jumpOdds == 1 || GRandom.NextInt(jumpOdds) == 0) {
							if (prepareJumpTime.Max > 0)
								PrepareJump();
							else
								Jump();
						}
						else {
							Crawl();
						}
					}
				}
			}
		}

	}
}
