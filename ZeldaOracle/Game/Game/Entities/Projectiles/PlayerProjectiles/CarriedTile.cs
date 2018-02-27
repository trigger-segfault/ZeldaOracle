using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {
	public class CarriedTile : Entity {
		private Tile tile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CarriedTile(Tile tile) {
			this.tile = tile;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-3, -5, 6, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-7, -7, 14, 14);
			Physics.Enable(
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles);

			if (tile.HasFlag(TileFlags.Bounces))
				Physics.Enable(PhysicsFlags.Bounces);

			BounceSound = GameData.SOUND_BOMB_BOUNCE;

			// Graphics.
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= false;
			Graphics.IsRipplesEffectVisible	= false;
			Graphics.DepthLayer				= DepthLayer.ProjectileCarriedTile;
			Graphics.DrawOffset				= new Point2I(-8, -13);
			centerOffset					= new Point2I(0, -5);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void Break() {
			if (tile.BreakAnimation != null) {
				Effect breakEffect = new Effect(tile.BreakAnimation, tile.BreakLayer, true);
				RoomControl.SpawnEntity(breakEffect, Center);
			}
			if (tile.BreakSound != null)
				AudioSystem.PlaySound(tile.BreakSound);
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			if (tile.SpriteAsObject != null)
				Graphics.PlayAnimation(tile.SpriteAsObject);
			else
				Graphics.PlayAnimation(tile.Graphics.AnimationPlayer.SpriteOrSubStrip);
			Graphics.IsUnmapped = true;
			Graphics.UnmappedPalette = RoomControl.Zone.Palette;
			Graphics.CreateUnmappedSprite();
		}

		public override void OnLand() {
			CollisionChecks();
			Break();
		}

		public override void OnBounce() {
			base.OnBounce();
			CollisionChecks();
		}

		public override void Update() {
			base.Update();

			if (Physics.IsColliding && !Physics.HasFlags(PhysicsFlags.Bounces))
				Break();
		}


		//-----------------------------------------------------------------------------
		// Carrying Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the pickupable entity while being carried.</summary>
		public override void UpdateCarrying(bool isPickingUp) {
			Graphics.Update();
		}

		/// <summary>Draws the pickupable entity while being carried.</summary>
		public override void DrawCarrying(RoomGraphics g, bool isPickingUp) {
			Graphics.Draw(g, DepthLayer.ProjectileCarriedTile);
		}

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Performs tile and monster collision checks.</summary>
		private void CollisionChecks() {
			// Collide with monsters.
			foreach (Monster monster in Physics.GetEntitiesMeeting<Monster>(CollisionBoxType.Soft)) {
				monster.TriggerInteraction(InteractionType.ThrownObject, this);
				if (IsDestroyed)
					return;
			}

			// Collide with surface tiles.
			Point2I tileLoc = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(tileLoc)) {
				Tile tile = RoomControl.GetTopTile(tileLoc);
				if (tile != null) {
					tile.OnHitByThrownObject(this);
					if (IsDestroyed)
						return;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Tile Tile {
			get { return tile; }
		}
	}
}
