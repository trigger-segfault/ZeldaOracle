using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Tiles {

	public class TileLantern : Tile, ZeldaAPI.Lantern {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLantern() {

		}


		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------
		
		public void Light(bool stayLit = false) {
			if (!IsLit) {
				IsLit = true;
				Graphics.PlayAnimation(SpriteList[0]);
				Properties.Set("lit", true);
				GameControl.FireEvent(this, "light", this);
			}
		}

		public void PutOut(bool stayLit = false) {
			if (IsLit) {
				IsLit = false;
				Graphics.PlayAnimation(SpriteList[1]);
				Properties.Set("lit", false);
				GameControl.FireEvent(this, "put_out", this);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnSeedHit(SeedType seedType, SeedEntity seed) {
			if (seedType == SeedType.Ember && !Properties.GetBoolean("lit", false)) {
				Light();
				// TODO: Should this sound be played in TileLantern.Light() instead?
				AudioSystem.PlaySound(GameData.SOUND_FIRE);
				seed.Destroy();
			}
		}

		public override void OnInitialize() {
			Graphics.PlayAnimation(SpriteList[IsLit ? 0 : 1]);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool lit = args.Properties.GetBoolean("lit", false);
			Tile.DrawTileDataIndex(g, args, lit ? 0 : 1);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsLit {
			get { return Properties.GetBoolean("lit"); }
			set { Properties.Set("lit", value); }
		}
	}
}
