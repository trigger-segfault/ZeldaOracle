using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class Bomb : Entity {
		private int timer;
		private int flashDelay;
		private int fuseTime;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Bomb() {
			EnablePhysics(
				PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles);
			
			//OriginOffset				= new Point2I(8, 14);
			Physics.CollisionBox		= new Rectangle2F(-3, -5, 6, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-3, -5, 6, 1);
			graphics.DrawOffset			= new Point2I(-8, -14);
			centerOffset				= new Point2I(0, -4);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void Explode() {
			BombExplosion bombExplosion = new BombExplosion();
			RoomControl.SpawnEntity(bombExplosion, Center, zPosition);

			// Explode nearby tiles.
			if (zPosition < 4) {
				Rectangle2F tileExplodeArea = new Rectangle2F(-12, -12, 24, 24);
				tileExplodeArea.Point += Center;

				Rectangle2I area = RoomControl.GetTileAreaFromRect(tileExplodeArea);
				for (int x = area.Left; x < area.Right; x++) {
					for (int y = area.Top; y < area.Bottom; y++) {
						Tile tile = RoomControl.GetTopTile(x, y);
						Rectangle2F tileRect = new Rectangle2F(x * 16, y * 16, 16, 16);
						if (tile != null && tileRect.Intersects(tileExplodeArea))
							tile.OnBombExplode();
					}
				}
			}

			DestroyAndTransform(bombExplosion);
		}

		private void BurnFuse() {
			timer++;
			if (timer == flashDelay)
				Graphics.PlayAnimation();
			else if (timer == fuseTime) {
				Explode();
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			timer		= 0;
			flashDelay	= 108 - 36;
			fuseTime	= 108;
			Graphics.PlayAnimation(GameData.ANIM_ITEM_BOMB);
			Graphics.PauseAnimation();
		}

		public override void UpdateCarrying() {
			BurnFuse();
			Graphics.Update();
		}

		public override void Update() {
			base.Update();
			BurnFuse();
		}
	}
}
