using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {

	/// <summary>Seeds dropped from the satchel.</summary>
	public class DroppedSeed : SeedEntity {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Create a dropped seed of the given seed type.</summary>
		public DroppedSeed(SeedType type) :
			base(type)
		{
			// Graphics
			Graphics.DepthLayer	= DepthLayer.InAirSeed;
			Graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);

			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 2);
			Physics.HasGravity				= true;
			Physics.IsDestroyedOutsideRoom	= true;
			Physics.IsDestroyedInHoles		= true;
			Physics.DisableSurfaceContact	= false;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnLand() {
			if (IsDestroyed)
				return;

			// Trigger instant reactions
			RoomControl.InteractionManager.TriggerInstantReaction(this,
				InteractionComponent.GetSeedInteractionType(SeedType));
			if (IsDestroyed)
				return;

			// Notify the tile of the seed hitting it
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnHitByProjectile(this);
				if (IsDestroyed)
					return;
			}

			// Spawn the seed effect
			Entity effect = DestroyWithSatchelEffect();
			if (SeedType == SeedType.Scent) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, 3);
			}
			else if (SeedType == SeedType.Ember) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, 1);
				effect.Physics.HasGravity = false;
			}
			else if (SeedType == SeedType.Mystery) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, -1);
			}
		}

		public override void Initialize() {
			base.Initialize();
			
			if (RoomControl.IsSideScrolling) {
				Physics.CollideWithWorld	= true;
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 3);
				Interactions.InteractionBox = Physics.CollisionBox;
			}
			else {
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
				Interactions.InteractionBox	= Physics.CollisionBox;
			}

			// Use the sprite for the current seed type
			Graphics.PlayAnimation(GameData.SPR_ITEM_SEEDS[(int) SeedType]);
		}
	}
}
