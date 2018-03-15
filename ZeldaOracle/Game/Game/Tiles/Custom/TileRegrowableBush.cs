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
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileRegrowableBush : Tile {

		private int regrowTimer;
		private bool isRegrowing;
		private bool isGrown;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileRegrowableBush() {
			Graphics.SyncPlaybackWithRoomTicks = false;
		}


		//-----------------------------------------------------------------------------
		// Plant Methods
		//-----------------------------------------------------------------------------

		private void Regrow() {
			regrowTimer = 0;
			isRegrowing = true;
			Graphics.PlayAnimation(GameData.ANIM_TILE_REGROWABLE_BUSH_REGROW);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			regrowTimer = 0;
			isGrown = true;
			isRegrowing = false;
			SetFlags(TileFlags.Cuttable | TileFlags.Bombable, true);
			Graphics.PlayAnimation(GameData.SPR_TILE_REGROWABLE_BUSH);
		}

		public override void Break(bool spawnDrops) {
			if (isGrown) {
				if (BreakAnimation != null && !CancelBreakEffect) {
					// Spawn the leaves effect and spawn drops.
					Effect effect = new Effect(BreakAnimation, DepthLayer.EffectTileBreak, true);
					RoomControl.SpawnEntity(effect, Center);
				}
				if (BreakSound != null && !CancelBreakSound)
					AudioSystem.PlaySound(BreakSound);

				if (spawnDrops)
					SpawnDrop();

				isGrown = false;
				Graphics.PlayAnimation(GameData.SPR_TILE_REGROWABLE_BUSH_CUT);
				regrowTimer = 0;
				
				SetFlags(TileFlags.Cuttable | TileFlags.Bombable, false);
			}
		}

		public override void Update() {
			base.Update();

			if (!isGrown) {
				if (!isRegrowing) {
					regrowTimer++;
					if (regrowTimer == 8 * 60) {
						Regrow();
					}
				}
				else if (Graphics.IsAnimationDone) {
					isGrown = true;
					isRegrowing = false;
					Graphics.PlayAnimation(GameData.SPR_TILE_REGROWABLE_BUSH);
					SetFlags(TileFlags.Cuttable | TileFlags.Bombable, true);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.Cuttable | TileFlags.Bombable |
				TileFlags.NoClingOnStab;
			
			data.Properties.Set("angle", Angle.Right)
				.SetDocumentation("Angle", "angle", "", "Seed Bouncer", "The angle the seed bouncer is facing.");
		}
	}
}
