using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles {

	public class TileSomariaBlock : Tile {


		public TileSomariaBlock() {
			// TODO: Break when getting crushed by Thwomp. (Only when moving)
		}

		public override void OnInitialize() {
			DropList = null;
			CancelBreakSound = true;
			CheckSurfaceTile();
			CancelBreakSound = false;
			var settings = new CollisionTestSettings(typeof(Entity),
				new Rectangle2F(GameSettings.TILE_SIZE), CollisionBoxType.Hard);
			foreach (Entity entity in RoomControl.Entities) {
				if (CollisionTest.PerformCollisionTest(Position, entity, settings).IsColliding) {
					if (entity.Physics.IsSolid && !(entity is EffectCreateSomariaBlock)) {
						CancelBreakSound = true;
						Break(false);
						break;
					}
					if (entity is Monster) {
						Monster monster = (Monster) entity;
						monster.TriggerInteraction(InteractionType.Block, new TileDummy(this));
					}
				}
			}
		}

		public override void OnFallInHole() {
			Break(false);
		}
		
		public override void OnFallInWater() {
			Break(false);
		}
		
		public override void OnFallInLava() {
			Break(false);
		}

		public override void OnFloating() {
			Break(false);
		}

		public override void Update() {
			base.Update();
			
			var settings = new CollisionTestSettings(typeof(Entity),
				new Rectangle2F(GameSettings.TILE_SIZE), CollisionBoxType.Hard);
			foreach (Entity entity in RoomControl.Entities) {
				if (CollisionTest.PerformCollisionTest(Position, entity, settings).IsColliding) {
					if (entity.Physics.IsSolid && !(entity is EffectCreateSomariaBlock)) {
						Break(false);
						break;
					}
					if (IsMoving && entity is Monster) {
						Monster monster = (Monster) entity;
						monster.TriggerInteraction(InteractionType.Block, new TileDummy(this));
					}
				}
			}
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
