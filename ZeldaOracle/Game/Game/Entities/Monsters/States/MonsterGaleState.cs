using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterGaleState : MonsterState {
		
		// ON MONSTER:
		//   31 before fade. (once fade starts, monster rises up)
		//   18 of fade.

		private int timer;
		private bool isRising;
		private AnimationPlayer galeAnimationPlayer;
		private float galeZPosition;
		private Vector2F monsterPosition;
		private EffectGale galeEffect;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterGaleState(EffectGale galeEffect) {
			this.galeEffect = galeEffect;
			galeAnimationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			monster.IsPassable = true;

			if (galeEffect != null) {
				monster.SetPositionByCenter(galeEffect.Position);
				galeEffect.DestroyAndTransform(monster);
				galeEffect = null;
			}

			timer			= 0;
			isRising		= false;
			galeZPosition	= monster.ZPosition;
			monsterPosition	= monster.Position;

			monster.Physics.Velocity = Vector2F.Zero;
			monster.Graphics.PauseAnimation();
			monster.Physics.Disable();

			galeAnimationPlayer.Play(GameData.ANIM_EFFECT_SEED_GALE);

		}

		public override void OnEnd(MonsterState newState) {
			// Monster should be destroyed, no need to change back to normal...?
		}

		public override void Update() {

			timer++;

			int[] xOffsets = new int[] { -2, 0, 2, 0 };
			monster.Position = monsterPosition + new Vector2F(xOffsets[timer % 4], 0);

			galeAnimationPlayer.Update();

			if (timer == 31) {
				isRising = true;
			}

			if (isRising) {
				monster.ZPosition += 6;

				if (monster.Position.Y - monster.ZPosition < -2) {
					monster.Destroy();
				}
			}

			// After flying above screen, destroy monster.
		}

		public override void DrawOver(RoomGraphics g) {
			// Draw the gale effect.
			if (timer % 2 == 0 || !isRising) {
				g.DrawAnimationPlayer(galeAnimationPlayer, monsterPosition + monster.CenterOffset -
					new Vector2F(0, galeZPosition), DepthLayer.EffectMonsterBurnFlame);
			}
		}
	}
}
