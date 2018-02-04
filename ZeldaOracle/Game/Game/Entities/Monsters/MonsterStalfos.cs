using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterStalfos : BasicMonster {

		private int jumpDelayTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterStalfos() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Blue;

			// Movement
			moveSpeed					= 0.5f;
			numMoveAngles				= 16;
			facePlayerOdds				= 0;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			stopTime.Set(0, 0);
			moveTime.Set(30, 80);

			// Physics
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics
			animationMove = GameData.ANIM_MONSTER_STALFOS;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void JumpAwayFromPlayer() {
			Physics.ZVelocity = GameSettings.MONSTER_STALFOS_ORANGE_JUMP_SPEED;
			Physics.Velocity = (Center -
				RoomControl.Player.Center).Normalized *
				GameSettings.MONSTER_STALFOS_ORANGE_JUMP_MOVE_SPEED;

			jumpDelayTimer = GameSettings.MONSTER_STALFOS_ORANGE_JUMP_RECHARGE_DELAY;

			AudioSystem.PlaySound(GameData.SOUND_MONSTER_JUMP);
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_STALFOS_JUMP);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (Color == MonsterColor.Blue) {
				MaxHealth		= 1;
				ContactDamage	= 2;
			}
			else if (Color == MonsterColor.Orange) {
				MaxHealth		= 2;
				ContactDamage	= 2;

				// Equipped with bone projectiles
				projectileType				= typeof(BoneProjectile);
				shootSpeed					= 1.5f;
				shootPauseDuration			= 0;
				shootType					= ShootType.WhileMoving;
				projectileShootOdds			= 400;
				shootSound					= null;
				aimType						= AimType.SeekPlayer;
			}
			else if (Color == MonsterColor.Green) {
				MaxHealth		= 2;
				ContactDamage	= 2;
			}
			else if (Color == MonsterColor.Red) {
				MaxHealth		= 2;
				ContactDamage	= 2;
			}

			Health = MaxHealth;
			jumpDelayTimer = 0;

			// TODO: possible event system?
			//RoomControl.FireEvent(Events.PlayerUseItem);
			//RoomControl.RegisterEvent(Events.PlayerUseItem, this.OnPlayerUseItem);
		}

		public override void OnLand() {
			base.OnLand();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_STALFOS);
		}

		public override void UpdateAI() {
			base.UpdateAI();
			
			if (jumpDelayTimer > 0)
				jumpDelayTimer--;

			// Check for jumping toward or away from the player
			if (IsOnGround && jumpDelayTimer == 0) {
				float distanceToPlayer = Center.DistanceTo(RoomControl.Player.Center);

				if (distanceToPlayer < GameSettings.MONSTER_STALFOS_ORANGE_JUMP_RANGE) {
					if (Color == MonsterColor.Orange) {
						// TODO: rework this check into some event fired off of when the
						// player uses an item
						if (Controls.A.IsPressed() || Controls.B.IsPressed())
							JumpAwayFromPlayer();
					}
				}
			}
		}
	}
}
