using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Projectiles.MagicProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterWizzrobe : Monster {

		private enum WizzrobeState {
			Appearing,
			Appeared,
			Disappearing,
			Disapeared,
		}

		private float wizzrobeTimer;
		private WizzrobeState wizzrobeState;
		private Animation appearAnimation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterWizzrobe() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 1;
			color			= MonsterColor.Blue;
			
			// Movement
			syncAnimationWithDirection	= true;
			isFlying = true;

			// Physics
			Physics.CollideWithWorld = false;

			// Weapon interactions
			SetReaction(InteractionType.SwitchHook, Reactions.SwitchHook, WizzrobeSwitchHook);
			// Projectile interactions
			SetReaction(InteractionType.Boomerang, SenderReactions.Intercept);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (color == MonsterColor.Green) {
				MaxHealth		= 2;
				ContactDamage	= 2;
			}
			else if (color == MonsterColor.Red) {
				MaxHealth		= 4;
				ContactDamage	= 2;
			}
			
			Health = MaxHealth;

			appearAnimation = GameData.ANIM_MONSTER_WIZZROBE_HAT;
			wizzrobeState = WizzrobeState.Disapeared;
			graphics.IsVisible = false;
			IsPassable = true;
		}

		public void WizzrobeSwitchHook(Entity sender, EventArgs args) {
			// Strange interaction: after being switched with, the Wizzrobe
			// will proceed to dissapear, using his standing animation until
			// the next time he disapears.
			appearAnimation = GameData.ANIM_MONSTER_WIZZROBE;
			wizzrobeTimer = 0;
			IsPassable = true;
			Physics.Velocity = Vector2F.Zero;
			graphics.PlayAnimation(appearAnimation);
			wizzrobeState = WizzrobeState.Disappearing;
		}

		public override void UpdateAI() {
			wizzrobeTimer++;

			if (wizzrobeState == WizzrobeState.Disapeared) {
				if (wizzrobeTimer >= GameSettings.MONSTER_WIZZROBE_GREEN_APPEAR_DURATION) {
					wizzrobeTimer = 0;
					Graphics.IsVisible = true;
					Graphics.IsFlickering = true;
					graphics.PlayAnimation(appearAnimation);
					wizzrobeState = WizzrobeState.Appearing;

					if (color == MonsterColor.Red) {
						// Appear at a random location
						position = GetRandomSpawnLocation();
					}
				}
			}
			else if (wizzrobeState == WizzrobeState.Appearing) {
				if (wizzrobeTimer ==
					GameSettings.MONSTER_WIZZROBE_GREEN_PEEK_FLICKER_DURATION)
				{
					Graphics.IsFlickering = false;
				}
				if (wizzrobeTimer >= GameSettings.MONSTER_WIZZROBE_GREEN_APPEAR_DURATION) {
					wizzrobeTimer = 0;
					graphics.PlayAnimation(GameData.ANIM_MONSTER_WIZZROBE);
					IsPassable = false;
					FacePlayer();
					wizzrobeState = WizzrobeState.Appeared;
					appearAnimation = GameData.ANIM_MONSTER_WIZZROBE_HAT;
				}
			}
			else if (wizzrobeState == WizzrobeState.Appeared) {
				if (wizzrobeTimer ==
					GameSettings.MONSTER_WIZZROBE_GREEN_SHOOT_DELAY)
				{
					// Shoot
					MagicProjectile projectile = new MagicProjectile();
					ShootFromDirection(projectile, Direction,
						GameSettings.MONSTER_WIZZROBE_SHOOT_SPEED);
				}
				if (wizzrobeTimer >= GameSettings.MONSTER_WIZZROBE_GREEN_APPEAR_DURATION) {
					wizzrobeTimer = 0;
					IsPassable = true;
					Physics.Velocity = Vector2F.Zero;
					graphics.PlayAnimation(appearAnimation);
					wizzrobeState = WizzrobeState.Disappearing;
				}
			}
			else if (wizzrobeState == WizzrobeState.Disappearing) {
				if (wizzrobeTimer == GameSettings.MONSTER_WIZZROBE_GREEN_APPEAR_DURATION -
					GameSettings.MONSTER_WIZZROBE_GREEN_PEEK_FLICKER_DURATION)
				{
					Graphics.IsFlickering = true;
				}
				if (wizzrobeTimer >= GameSettings.MONSTER_WIZZROBE_GREEN_APPEAR_DURATION) {
					wizzrobeTimer = 0;
					Graphics.IsVisible = false;
					Graphics.IsFlickering = false;
					wizzrobeState = WizzrobeState.Disapeared;
				}
			}
		}
	}
}
