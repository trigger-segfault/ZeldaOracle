using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Tiles.Internal {

	public class AppearingTile : PlaceHolderTile {
		
		private int timer;
		private TileSpawnOptions spawnOptions;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public AppearingTile(TileDataInstance tile, TileSpawnOptions spawnOptions) :
			base(tile)
		{
			this.spawnOptions = spawnOptions;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			timer = 0;
			
			fallsInHoles = false;

			// Spawn the poof effect.
			if (spawnOptions.PoofEffect) {
				Point2I size = tile.Size;
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						Effect effect = new Effect(GameData.ANIM_EFFECT_BLOCK_POOF,
							Entities.DepthLayer.EffectSomariaBlockPoof);
						Vector2F pos = (Location + new Point2I(x, y) + Vector2F.Half) * GameSettings.TILE_SIZE;
						RoomControl.SpawnEntity(effect, pos);
					}
				}
				AudioSystem.PlaySound(GameData.SOUND_APPEAR_VANISH);
			}
		}

		public override void Update() {
			base.Update();

			// Spawn the tile after a delay.
			timer++;
			if (timer >= spawnOptions.SpawnDelayAfterPoof || !spawnOptions.PoofEffect)
				SpawnTile();
		}
	}
}
