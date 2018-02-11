using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TileSpikedFloor : Tile {
		
		public TileSpikedFloor() {
			
		}

		public override void Update() {
			Player player = RoomControl.Player;
			
			// Hurt the player if he is standing on this tile.
			if (player.IsOnGround && !player.IsInvincible && player.IsDamageable && player.Physics.TopTile == this) {
				DamageInfo damage = new DamageInfo(2) {
					ApplyKnockback		= true,
					KnockbackDuration	= 9,
					InvincibleDuration	= 35,
					FlickerDuration		= 35,
				};

				if (player.Physics.Velocity.Length > 0.1f) {
					damage.HasSource = true;
					damage.SourcePosition = player.Center + player.Physics.Velocity;
				}
				else {
					damage.HasSource = false;
				}

				player.Hurt(damage);
			}

			base.Update();
		}

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}
	}
}
