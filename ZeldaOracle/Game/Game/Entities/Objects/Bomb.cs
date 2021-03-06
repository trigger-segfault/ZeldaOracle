﻿using System;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class Bomb : Entity {

		private int timer;
		private int flashDelay;
		private int fuseTime;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Bomb() {
			// Graphics
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= false;
			Graphics.IsRipplesEffectVisible	= false;
			Graphics.DepthLayer				= DepthLayer.ProjectileBomb;
			Graphics.DrawOffset				= new Point2I(-8, -13);
			centerOffset					= new Point2I(0, -3);

			// Physics
			Physics.CollisionBox = new Rectangle2F(-3, -5, 6, 1);
			Physics.Enable(
				PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles |
				PhysicsFlags.MoveWithConveyors);
			soundBounce = GameData.SOUND_BOMB_BOUNCE;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox	= new Rectangle2I(-4, -9, 8, 8);
			Interactions.InteractionZRange = new RangeF(-5, 5);
			Reactions[InteractionType.Bracelet].Set(EntityReactions.Pickup);
			Reactions[InteractionType.Bracelet].CollisionBox = null;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void Explode() {
			BombExplosion bombExplosion = new BombExplosion();
			RoomControl.SpawnEntity(bombExplosion, Center, ZPosition);
			AudioSystem.PlaySound(GameData.SOUND_BOMB_EXPLODE);

			// Explode nearby top tiles.
			if (zPosition < 4) {
				Rectangle2F tileExplodeArea = Rectangle2F.Zero.Inflated(12, 12);
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

			if (RoomControl.IsSideScrolling)
				Physics.CollisionBox = new Rectangle2F(-3, -5, 6, 6);
			else
				Physics.CollisionBox = new Rectangle2F(-3, -5, 6, 1);
			Interactions.InteractionBox = new Rectangle2I(-4, -9, 8, 8);

			timer		= 0;
			flashDelay	= GameSettings.BOMB_FLICKER_DELAY;
			fuseTime	= GameSettings.BOMB_FUSE_TIME;
			Graphics.PlayAnimation(GameData.ANIM_ITEM_BOMB);
			Graphics.PauseAnimation();
		}

		public override void Update() {
			base.Update();
			BurnFuse();
		}


		//-----------------------------------------------------------------------------
		// Carrying Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the pickupable entity while being carried.</summary>
		public override void UpdateCarrying(bool isPickingUp) {
			Graphics.Update();
			BurnFuse();
		}

		/// <summary>Draws the pickupable entity while being carried.</summary>
		public override void DrawCarrying(RoomGraphics g, bool isPickingUp) {
			Graphics.Draw(g, DepthLayer.ProjectileCarriedTile);
		}
	}
}
