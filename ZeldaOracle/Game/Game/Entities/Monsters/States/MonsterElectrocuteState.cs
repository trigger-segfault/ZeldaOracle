using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterElectrocuteState : MonsterState {

		private int timer;
		private int animateDuration;
		private int freezePlayerDuration;
		private Animation monsterAnimation;
		private Animation resumeAnimation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterElectrocuteState(Animation monsterAnimation) {
			this.animateDuration		= GameSettings.MONSTER_ELECTROCUTE_ANIMATE_DURATION;
			this.freezePlayerDuration	= GameSettings.MONSTER_ELECTROCUTE_FREEZE_DURATION;
			this.monsterAnimation		= monsterAnimation;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			timer = 0;

			monster.Physics.Velocity = Vector2F.Zero;
			if (monsterAnimation != null) {
				resumeAnimation = monster.Graphics.Animation;//monster.Graphics.PauseAnimation();
				monster.Graphics.PlayAnimation(monsterAnimation);
			}
			monster.RoomControl.Player.Freeze();
			//monster.Physics.Disable();
			AudioSystem.PlaySound(GameData.SOUND_ELECTROCUTE);
		}

		public override void OnEnd(MonsterState newState) {
			if (monsterAnimation != null)
				monster.Graphics.PlayAnimation(resumeAnimation);
			monster.RoomControl.TilePaletteOverride = null;
			monster.RoomControl.EntityPaletteOverride = null;
			monster.OnElectrocuteComplete();
			//monster.Physics.Enable();
		}

		public override void Update() {
			timer++;

			// Update screen shake
			// 5 frames before first screen flash
			// Each screen flash is 8 frames long, with 8 frames between them
			// There are 3 screen flashes.
			if (timer >= 5) {
				int flashIndex = ((timer - 5) / 8);

				if (flashIndex % 2 == 0 && flashIndex <= 4) {
					monster.RoomControl.TilePaletteOverride = GameData.PAL_TILES_ELECTROCUTED;
					monster.RoomControl.EntityPaletteOverride = GameData.PAL_ENTITIES_ELECTROCUTED;
					//if (GRandom.NextBool()) {
					monster.RoomControl.ViewControl.ShakeOffset = new Vector2F(
							(float) GRandom.NextInt(-2, 2),
							(float) GRandom.NextInt(-2, 2));
					//}
				}
				else {
					monster.RoomControl.TilePaletteOverride = null;
					monster.RoomControl.EntityPaletteOverride = null;
				}
			}

			if (timer == freezePlayerDuration) {
				monster.RoomControl.Player.Unfreeze();
				monster.RoomControl.ViewControl.ShakeOffset = Vector2F.Zero;
			}

			if (timer > animateDuration) {
				monster.BeginNormalState();
			}
		}
	}
}
