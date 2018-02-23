using System;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterRiverZora : Monster {
		
		private enum RiverZoraState {
			Submerged,
			Resurfacing,
			Resurfaced,
		}
		
		private FireballProjectile fireball;
		private GenericStateMachine<RiverZoraState> stateMachine;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterRiverZora() {
			// General
			MaxHealth		= 2;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			isGaleable		= false;
			isKnockbackable	= false;
			isFlying		= true;

			// Physics
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;
			Physics.IsDestroyedInHoles	= false;

			// Projectile interactions
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Damage);

			// Behavior
			stateMachine = new GenericStateMachine<RiverZoraState>();
			stateMachine.AddState(RiverZoraState.Submerged)
				.OnBegin(BeginSubmerged)
				.SetDuration(25, 60);
			stateMachine.AddState(RiverZoraState.Resurfacing)
				.OnBegin(BeginResurfacing)
				.SetDuration(48);
			stateMachine.AddState(RiverZoraState.Resurfaced)
				.OnBegin(BeginResurfaced)
				.AppendEvent(48,	OpenMouth)
				.AppendEvent(9,		SpawnFireball)
				.AppendEvent(9,		ShootFireball)
				.AppendEvent(22,	CloseMouth)
				.AppendEvent(9,		stateMachine.NextState)
				.OnEnd(EndResurfaced);
		}


		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------

		public void BeginSubmerged() {
			IsPassable = true;
			Graphics.IsVisible = false;
		}

		public void BeginResurfacing() {
			// Spawn at a random water tile
			position = GetRandomSpawnLocation();
			Graphics.IsVisible = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA_WATER_SWIRLS);
		}

		public void BeginResurfaced() {
			IsPassable = false;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA);
		}

		public void OpenMouth() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA_SHOOT);
		}

		public void SpawnFireball() {
			fireball = new FireballProjectile();
			ShootProjectile(fireball, Vector2F.Zero);
		}

		public void ShootFireball() {
			Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
			fireball.Physics.Velocity = vectorToPlayer.Normalized *
				GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_SPEED;
			fireball = null;
		}

		public void CloseMouth() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA);
		}

		public void EndResurfaced() {
			IsPassable = true;
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true), position);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			stateMachine.BeginState(RiverZoraState.Submerged);
		}

		public override void OnDestroy() {
			base.OnDestroy();
			
			// If we spawned a fireball and have not shot it yet, then destroy it
			if (fireball != null)
				fireball.Destroy();
		}

		public override bool CanSpawnAtLocation(Point2I location) {
			// Only spawn on water tiles
			Tile tile = RoomControl.GetTopTile(location);
			return (tile != null && tile.IsWater && !tile.IsSolid && !tile.IsHole);
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
