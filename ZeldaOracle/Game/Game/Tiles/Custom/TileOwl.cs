using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Tiles {

	public class TileOwl : Tile {

		private bool		isActivated;
		private Point2I[]	sparklePositions;
		private int			sparkleIndex;
		private int			timer;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileOwl() {
			sparklePositions = new Point2I[] {
					new Point2I(10, 7),
					new Point2I(2, 8),
					new Point2I(15, 10),
					new Point2I(2, 4),
					new Point2I(7, 14),
					new Point2I(13, 1)
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnSeedHit(SeedType seedType, SeedEntity seed) {
			if (seedType == SeedType.Mystery && !isActivated) {
				isActivated		= true;
				sparkleIndex	= 0;
				timer			= 0;
			}
		}

		public override void OnInitialize() {
			isActivated = false;
			//CustomSprite = GameData.SPR_TILE_OWL;
		}

		public override void Update() {
			base.Update();

			// TODO: Sparkle effects should update while text is reading.

			if (isActivated) {
				timer++;
				
				// Create a sparkle every 8 ticks.
				if (timer % 8 == 1 && sparkleIndex < sparklePositions.Length) {
					Effect effect = new Effect(GameData.ANIM_EFFECT_OWL_SPARKLE, DepthLayer.EffectOwlSparkles, true);
					RoomControl.SpawnEntity(effect,
						Position + sparklePositions[sparkleIndex]);
					sparkleIndex++;
				}

				if (timer == 49) {
					CustomSprite = SpriteList[1];
					//CustomSprite = GameData.SPR_TILE_OWL_ACTIVATED;
				}
				if (timer == 58) {
					string text = Properties.GetString("text", GameSettings.TEXT_UNDEFINED);
					RoomControl.GameControl.DisplayMessage(text);
				}
				if (timer > 80) {
					isActivated = false;
					CustomSprite = SpriteList[0];
					//CustomSprite = GameData.SPR_TILE_OWL;
				}
			}
		}
	}
}
