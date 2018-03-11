using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Tiles {

	public class TileSomariaBlock : Tile {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileSomariaBlock() {
			// TODO: Break when getting crushed by Thwomp. (Only when moving)
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if this tile is touching a solid entity.</summary>
		private bool IsTouchingSolidEntity() {
			Rectangle2F bounds = Bounds;
			CancelBreakSound = false;
			foreach (Entity entity in RoomControl.Entities) {
				if (entity.Physics.IsSolid &&
					!(entity is EffectCreateSomariaBlock) &&
					entity.Physics.PositionedCollisionBox.Intersects(bounds))
				{
					return true;
				}
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			DropList = null;

			// Check if spawned over a hazard tile
			CancelBreakSound = true;
			CheckSurfaceTile();
			if (IsDestroyed)
				return;

			// Check if spawned over a solid entity
			if (IsTouchingSolidEntity()) {
				CancelBreakSound = true;
				Break(false);
			}

			CancelBreakSound = false;
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
			// Destroy if touched by a solid entity
			if (IsTouchingSolidEntity()) {
				CancelBreakSound = true;
				Break(false);
				return;
			}
			
			base.Update();
		}
	}
}
