using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class EffectGale : Effect {
		
		// FROM SHOOTER TO WALL:
		//   12 before fade.
		//   18 of fade
		// ON MONSTER:
		//   31 before fade. (once fade starts, monster rises up)
		//   18 of fade.
		// ON NOTHING (Dropped from satchel)
		//   0 before fade.
		//   256 of fade.

		private bool droppedFromSatchel;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectGale(bool droppedFromSatchel) :
			base(GameData.ANIM_EFFECT_SEED_GALE, DepthLayer.EffectGale)
		{
			this.droppedFromSatchel = droppedFromSatchel;

			Physics.SoftCollisionBox = new Rectangle2F(-6, -6, 12, 12);

			if (droppedFromSatchel) {
				CreateDestroyTimer(256, 255, 1);
			}
			else {
				CreateDestroyTimer(30, 12, 1);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();
			
			if (droppedFromSatchel) {
				// Collide with player.
				if (Physics.IsMeetingEntity(RoomControl.Player, CollisionBoxType.Soft)) {
					// TODO: Gale player to warp screen.
				}
			}
			else {
				// Collide with monsters.
				CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);

				for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
					Monster monster = iterator.CollisionInfo.Entity as Monster;
					/*
					// TODO: Check better conditions here.
					if (monster.IsAlive) {
						monster.EnterGale(this);
						Destroy();
						return;
					}*/

					// TODO: Monster gale interactions.
					monster.TriggerInteraction(InteractionType.GaleSeed, this);
					if (IsDestroyed)
						return;
					//monster.TriggerInteraction(InteractionType.Fire, this);
					//if (IsDestroyed)
						//return;
				}
			}
		}
	}
}
