using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSwingBigSwordState : PlayerBaseSwingSwordState {

		private const int SL = 18;
		private const int AL = 14;

		private readonly Rectangle2I[,] SWING_TOOL_BOXES_BIG = new Rectangle2I[,] {
			{
				new Rectangle2I(-8, -8 - SL, 16, SL),
				new Rectangle2I(8, -8 - AL, AL, AL),
				new Rectangle2I(8, -8, SL, 16),
				new Rectangle2I(8, 8, AL, AL),
				new Rectangle2I(-8, 8, 16, SL),
			}, {
				new Rectangle2I(8, -8, SL, 16),
				new Rectangle2I(8, -8 - AL, AL, AL),
				new Rectangle2I(-8, -8 - SL, 16, SL),
				new Rectangle2I(-8 - AL, -8 - AL, AL, AL),
				new Rectangle2I(-8 - SL, -8, SL, 16),
			}, {
				new Rectangle2I(-8, -8 - SL, 16, SL),
				new Rectangle2I(-8 - AL, -8 - AL, AL, AL),
				new Rectangle2I(-8 - SL, -8, SL, 16),
				new Rectangle2I(-8 - AL, 8, AL, AL),
				new Rectangle2I(-8, 8, 16, SL),
			}, {
				new Rectangle2I(-8 - SL, -8, SL, 16),
				new Rectangle2I(-8 - AL, 8, AL, AL),
				new Rectangle2I(-8, 8, 16, SL),
				new Rectangle2I(8, 8, AL, AL),
				new Rectangle2I(8, -8, SL, 16),
			}
		};

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingBigSwordState() {
			isReswingable					= false;
			lunge							= false;
			swingAnglePullBack				= 2;
			swingAngleDurations				= new int[] { 12, 4, 4, 4, 10 };
			weaponSwingAnimation			= GameData.ANIM_BIG_SWORD_SWING;
			swingCollisionBoxesNoLunge		= SWING_TOOL_BOXES_BIG;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override Animation GetPlayerSwingAnimation(bool lunge) {
			return player.Animations.SwingBig;
		}

		public override void OnSwingBegin() {
			AudioSystem.PlaySound(GameData.SOUND_BIGGORON_SWORD);
		}

		public override void OnSwingEnd() {
			End();
		}
	}
}
