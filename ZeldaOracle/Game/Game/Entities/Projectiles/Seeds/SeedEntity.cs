using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {

	/// <summary>The base class for the two types of seed entities: DroppedSeed and
	/// SeedProjectile.</summary>
	public abstract class SeedEntity : Projectile {
		
		/// <summary>The type of seed (Ember, Scent, Gale, Pegasus, Mystery).</summary>
		private SeedType type;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SeedEntity(SeedType type) {
			this.type = type;
		}
		

		//-----------------------------------------------------------------------------
		// Seed Effects
		//-----------------------------------------------------------------------------

		/// <summary>Intercept the seed, destroying it while spawning the seed effect.
		/// </summary>
		public override void Intercept() {
			DestroyWithEffect();
		}

		/// <summary>Destroy the seed and spawn its regular effect.</summary>
		public Entity DestroyWithEffect(bool satchelEffect = false) {
			Entity effectEntity = CreateEffect(type, satchelEffect, Center);
			DestroyAndTransform(effectEntity);
			return effectEntity;
		}

		/// <summary>Destroy the seed and spawn its regular effect, but absorbing the
		/// effect such as when fire is instantly put out after having spawned.
		/// </summary>
		public Entity DestroyWithAbsorbedEffect(bool satchelEffect = false) {
			Entity effectEntity = DestroyWithEffect(satchelEffect);
			if (effectEntity is Fire)
				(effectEntity as Fire).IsAbsorbed = true;
			return effectEntity;
		}
		
		/// <summary>Destroy the seed and spawn its satchel effect.</summary>
		public Entity DestroyWithSatchelEffect() {
			Entity effectEntity = CreateEffect(type, true, Center);
			DestroyAndTransform(effectEntity);
			return effectEntity;
		}

		/// <summary>Spawn the effect entity for the given seed type. Some seeds will
		/// have different effects when dropped from the satchel. Returns the spawned
		/// effect entity.</summary>
		private Entity CreateEffect(SeedType seedType,
			bool satchelEffect, Vector2F effectPosition)
		{
			Entity effectEntity = null;
		
			// Create the seed's effect
			if (seedType == SeedType.Ember) {
				effectEntity = new Fire();
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
			}
			else if (seedType == SeedType.Scent) {
				if (satchelEffect) {
					effectEntity = new ScentPod();
					AudioSystem.PlaySound(GameData.SOUND_SCENT_SEED_POD);
				}
				else {
					effectEntity = new Effect(
						GameData.ANIM_EFFECT_SEED_SCENT, DepthLayer.EffectSeed);
					AudioSystem.PlaySound(GameData.SOUND_SCENT_SEED);
				}
			}
			else if (seedType == SeedType.Mystery) {
				effectEntity = new Effect(
					GameData.ANIM_EFFECT_SEED_MYSTERY, DepthLayer.EffectSeed);
				AudioSystem.PlaySound(GameData.SOUND_MYSTERY_SEED);
			}
			else if (seedType == SeedType.Pegasus) {
				effectEntity = new Effect(
					GameData.ANIM_EFFECT_SEED_PEGASUS, DepthLayer.EffectSeed);
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
			}
			else if (seedType == SeedType.Gale) {
				effectEntity = new EffectGale(satchelEffect);
				AudioSystem.PlaySound(GameData.SOUND_GALE_SEED);
			}
			
			RoomControl.SpawnEntity(effectEntity, effectPosition);
			return effectEntity;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The type of seed (Ember, Scent, Gale, Pegasus, Mystery).</summary>
		public SeedType SeedType {
			get { return type; }
			set { type = value; }
		}
	}
}
