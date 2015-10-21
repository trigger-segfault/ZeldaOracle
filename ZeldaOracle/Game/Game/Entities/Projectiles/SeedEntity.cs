using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			CreateEffect(seedType, effectPosition);
			Destroy();
		}

		public void DestroyWithVisualEffect(SeedType seedType, Vector2F effectPosition) {
			CreateVisualEffect(seedType, effectPosition);
			Destroy();
		}

		public void CreateVisualEffect(SeedType seedType, Vector2F effectPosition) {
			RoomControl.SpawnEntity(new Effect(
				GameData.ANIM_EFFECT_SEEDS[(int) seedType]), effectPosition);
		}

		public void CreateEffect(SeedType seedType, Vector2F effectPosition) {
			// Create the seed's effect.
			if (seedType == SeedType.Ember) {
				RoomControl.SpawnEntity(new Fire(), effectPosition);
			}
			else if (seedType == SeedType.Scent) {
				RoomControl.SpawnEntity(new ScentPod(), effectPosition);
			}
			else if (seedType == SeedType.Mystery) {
				CreateVisualEffect(seedType, effectPosition);
			}
			else if (seedType == SeedType.Pegasus) {
				CreateVisualEffect(seedType, effectPosition);
			}
			else if (seedType == SeedType.Gale) {
				CreateVisualEffect(seedType, effectPosition);
				//RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_GALE), effectPosition);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SeedType SeedType {
			get { return type; }
		}
	}
}
