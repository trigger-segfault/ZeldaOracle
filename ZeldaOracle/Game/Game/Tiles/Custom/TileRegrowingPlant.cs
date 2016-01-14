using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles {

	public class TileRegrowingPlant : Tile {

		private int regrowTimer;
		private bool isRegrowing;
		private bool isGrown;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileRegrowingPlant() {
			animationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Plant Methods
		//-----------------------------------------------------------------------------

		private void Regrow() {
			regrowTimer = 0;
			isRegrowing = true;
			animationPlayer.Play(GameData.ANIM_TILE_REGROWABLE_PLANT_REGROW);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			regrowTimer = 0;
			isGrown = true;
			isRegrowing = false;
			SetFlags(TileFlags.Cuttable | TileFlags.Bombable, true);
			animationPlayer.Play(new Animation(GameData.SPR_TILE_REGROWABLE_PLANT_GROWN));
		}

		public override void Break(bool spawnDrops) {
			if (isGrown) {
				// Spawn the leaves effect and spawn drops.
				Effect effect = new Effect(GameData.ANIM_EFFECT_LEAVES, DepthLayer.EffectTileBreak);
				RoomControl.SpawnEntity(effect, Center);
				
				if (spawnDrops)
					SpawnDrop();

				isGrown = false;
				animationPlayer.Play(new Animation(GameData.SPR_TILE_REGROWABLE_PLANT_CUT));
				regrowTimer = 0;
				
				SetFlags(TileFlags.Cuttable | TileFlags.Bombable, false);
			}
		}

		public override void Update() {
			base.Update();

			animationPlayer.Update();
			
			if (!isGrown) {
				if (!isRegrowing) {
					regrowTimer++;
					if (regrowTimer == 8 * 60) {
						Regrow();
					}
				}
				else if (animationPlayer.IsDone) {
					isGrown = true;
					isRegrowing = false;
					animationPlayer.Play(new Animation(GameData.SPR_TILE_REGROWABLE_PLANT_GROWN));
					SetFlags(TileFlags.Cuttable | TileFlags.Bombable, true);
				}
			}
		}
	}
}
