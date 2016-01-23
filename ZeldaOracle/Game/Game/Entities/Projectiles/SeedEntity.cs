using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	// The types of seeds.
	public enum SeedType {
		Ember	= 0,
		Scent	= 1,
		Pegasus	= 2,
		Gale	= 3,
		Mystery	= 4
	}


	// The base class for the two types of seeds.
	public class SeedEntity : Projectile, IInterceptable {
		
		protected SeedType type;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SeedEntity(SeedType type) {
			this.type = type;
		}
		

		//-----------------------------------------------------------------------------
		// Seed Effects
		//-----------------------------------------------------------------------------

		public void Intercept() {
			DestroyWithEffect();
		}
		
		public void TriggerMonsterReaction(Monster monster) {
			// Destroy the seed and create the effect.
			Entity effect = DestroyWithEffect();

			// Trigger the immediate seed effect actions.
			if (type == SeedType.Scent)
				monster.TriggerInteraction(InteractionType.ScentSeed, effect);
			else if (type == SeedType.Pegasus)
				monster.TriggerInteraction(InteractionType.PegasusSeed, effect);
			else if (type == SeedType.Mystery)
				monster.TriggerInteraction(InteractionType.MysterySeed, effect);
		}

		// Destroy the seed and spawn its regular effect.
		public Entity DestroyWithAbsorbedEffect(bool satchelEffect = false) {
			Entity effectEntity = DestroyWithEffect(satchelEffect);
			if (effectEntity is Fire)
				(effectEntity as Fire).IsAbsorbed = true;
			return effectEntity;
		}

		// Destroy the seed and spawn its regular effect.
		public Entity DestroyWithEffect(bool satchelEffect = false) {
			Entity effectEntity = CreateEffect(type, satchelEffect, Center);
			DestroyAndTransform(effectEntity);
			return effectEntity;
		}
		
		// Destroy the seed and spawn its satchel effect.
		public Entity DestroyWithSatchelEffect() {
			Entity effectEntity = CreateEffect(type, true, Center);
			DestroyAndTransform(effectEntity);
			return effectEntity;
		}

		private Entity CreateEffect(SeedType seedType, bool satchelEffect, Vector2F effectPosition) {
			Entity effectEntity = null;

			// Create the seed's effect.
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
					effectEntity = new Effect(GameData.ANIM_EFFECT_SEED_SCENT, DepthLayer.EffectSeed);
					AudioSystem.PlaySound(GameData.SOUND_SCENT_SEED);
				}
			}
			else if (seedType == SeedType.Mystery) {
				effectEntity = new Effect(GameData.ANIM_EFFECT_SEED_MYSTERY, DepthLayer.EffectSeed);
				AudioSystem.PlaySound(GameData.SOUND_MYSTERY_SEED);
			}
			else if (seedType == SeedType.Pegasus) {
				effectEntity = new Effect(GameData.ANIM_EFFECT_SEED_PEGASUS, DepthLayer.EffectSeed);
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

		public SeedType SeedType {
			get { return type; }
			set { type = value; }
		}
	}
}
