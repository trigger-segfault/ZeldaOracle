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

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSpinSwordState : PlayerBaseSwingSwordState {
		
		private const int SL = 16;
		private const int AL = 13;
		private const int SW = 10;
		private readonly Rectangle2I[] SWING_TOOL_BOXES_SPIN = new Rectangle2I[] {
			new Rectangle2I(8, -8 + 16 - SW, SL, SW),
			new Rectangle2I(-8 + SW, -8 - AL, AL + 16 - SW, AL + 16 - SW),
			new Rectangle2I(-8, -8 - SL, SW, SL),
			new Rectangle2I(-8 - AL, -8 - AL, AL, AL + 16 - SW),
			new Rectangle2I(-8 - SL, -8 + 16 - SW, SL, SW),
			new Rectangle2I(-8 - AL, 8, AL + 16 - SW, AL),
			new Rectangle2I(-8 + 16 - SW, 8, SW, SL),
			new Rectangle2I(8, 8, AL, AL),
		};


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSpinSwordState() {
			limitTilesToDirection	= false;
			isReswingable			= false;

			lunge					= false;
			swingAnglePullBack		= 0;
			swingAngleDurations		= new int[] { 3, 2, 3, 2, 3, 2, 3, 2, 5 };
			weaponSwingAnimation			= GameData.ANIM_SWORD_SPIN;
			playerSwingAnimation			= GameData.ANIM_PLAYER_SPIN;
			playerSwingAnimationUnderwater	= GameData.ANIM_PLAYER_MERMAID_SPIN;
			
			// Will always spin clockwise.
			swingWindingOrders = new WindingOrder[] {
				WindingOrder.Clockwise,
				WindingOrder.Clockwise,
				WindingOrder.Clockwise,
				WindingOrder.Clockwise
			};

			swingCollisionBoxesNoLunge = new Rectangle2I[4, 9];
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 9; j++) {
					int angle = GMath.Wrap((i * 2) - j, Angles.AngleCount);
					swingCollisionBoxesNoLunge[i, j] = SWING_TOOL_BOXES_SPIN[angle];
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Sword Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingTilePeak(int angle, Vector2F hitPoint) {
			// Don't cut the tile when the swing is started.
			if (SwingAngleIndex > 0)
				base.OnSwingTilePeak(angle, hitPoint);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			base.OnSwingBegin();
			AudioSystem.PlaySound(GameData.SOUND_SWORD_SPIN);
		}

		public override void OnSwingEnd() {
			// Cut the center tile.
			CutTilesAtPoint(player.Center);
			End();
		}
	}
}
