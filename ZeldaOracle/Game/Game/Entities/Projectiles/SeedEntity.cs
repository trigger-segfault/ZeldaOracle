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
	public class SeedEntity : Projectile {
		
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

		public void DestroyWithEffect(SeedType seedType, Vector2F effectPosition) {
			Entity effectEntity = CreateEffect(seedType, effectPosition);
			DestroyAndTransform(effectEntity);
		}

		public void DestroyWithVisualEffect(SeedType seedType, Vector2F effectPosition) {
			Entity effectEntity = CreateVisualEffect(seedType, effectPosition);
			DestroyAndTransform(effectEntity);
		}

		public Entity CreateVisualEffect(SeedType seedType, Vector2F effectPosition) {
			Effect effect = new Effect(GameData.ANIM_EFFECT_SEEDS[(int) seedType], DepthLayer.EffectSeed);
			RoomControl.SpawnEntity(effect, effectPosition);
			
			if (seedType == SeedType.Ember)
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
			else if (seedType == SeedType.Scent)
				AudioSystem.PlaySound(GameData.SOUND_SCENT_SEED);
			else if (seedType == SeedType.Mystery)
				AudioSystem.PlaySound(GameData.SOUND_MYSTERY_SEED);
			else if (seedType == SeedType.Pegasus)
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
			else if (seedType == SeedType.Gale)
				AudioSystem.PlaySound(GameData.SOUND_GALE_SEED);

			return effect;
		}

		public Entity CreateEffect(SeedType seedType, Vector2F effectPosition) {
			Entity effectEntity = null;

			// Create the seed's effect.
			if (seedType == SeedType.Ember) {
				effectEntity = new Fire();
				RoomControl.SpawnEntity(effectEntity, effectPosition);
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
			}
			else if (seedType == SeedType.Scent) {
				effectEntity = new ScentPod();
				RoomControl.SpawnEntity(effectEntity, effectPosition);
				AudioSystem.PlaySound(GameData.SOUND_SCENT_SEED_POD);
			}
			else if (seedType == SeedType.Mystery) {
				effectEntity = CreateVisualEffect(seedType, effectPosition);
				AudioSystem.PlaySound(GameData.SOUND_MYSTERY_SEED);
			}
			else if (seedType == SeedType.Pegasus) {
				effectEntity = CreateVisualEffect(seedType, effectPosition);
			}
			else if (seedType == SeedType.Gale) {
				effectEntity = CreateVisualEffect(seedType, effectPosition);
				AudioSystem.PlaySound(GameData.SOUND_GALE_SEED);
				//RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_GALE), effectPosition);
			}

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
