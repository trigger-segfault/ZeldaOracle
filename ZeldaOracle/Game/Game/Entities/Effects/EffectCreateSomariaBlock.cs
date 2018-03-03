using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Effects {

	public class EffectCreateSomariaBlock : Effect {

		private Point2I tileLocation;
		private ItemCane itemCane;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectCreateSomariaBlock(Point2I tileLocation, float zPosition, ItemCane itemCane) :
			base(GameData.ANIM_EFFECT_SOMARIA_BLOCK_CREATE)
		{
			this.itemCane		= itemCane;
			this.tileLocation	= tileLocation;
			this.position		= (tileLocation * GameSettings.TILE_SIZE) + new Vector2F(8, 8);
			this.zPosition		= zPosition;
			
			Graphics.DepthLayer	= DepthLayer.EffectBlockPoof;
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		/// <summary>Return true if a somaria block would not break when spawned at
		/// the given tile location.</summary>
		private bool CanBlockSpawnAtLocation(Point2I location) {
			if (!RoomControl.IsSideScrolling && IsInAir)
				return false;

			// TODO: check if there is a solid block below when side-scrolling.

			if (!RoomControl.IsTileInBounds(location))
				return false;

			foreach (Tile t in RoomControl.TileManager.GetTilesAtPosition(Center)) {
				if (!t.IsCoverableByBlock || t.IsHoleWaterOrLava)
					return false;
			}
			/*
			Tile checkTile = RoomControl.GetTopTile(location);
			if (checkTile == null)
				return true;
			return (checkTile.Layer != RoomControl.Room.TopLayer &&
					checkTile.IsCoverableByBlock && !checkTile.IsHoleWaterOrLava);*/
			return true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (CanBlockSpawnAtLocation(tileLocation)) {
				// Make this effect solid to the player (as if the somaria block has
				// already been spawned)
				Physics.CollisionBox = new Rectangle2F(-8, -8, 16, 16);
				Physics.Enable(PhysicsFlags.Solid);
			}
		}

		public override void OnDestroy() {
			base.OnDestroy();

			if (CanBlockSpawnAtLocation(tileLocation)) {
				// Create the somaria block
				TileSomariaBlock tile = (TileSomariaBlock) Tile.CreateTile(itemCane.SomariaBlockTileData);
				RoomControl.PlaceTileOnHighestLayer(tile, tileLocation);
				itemCane.SomariaBlockTile = tile;
			}
			else {
				// Spawn a poof effect
				RoomControl.SpawnEntity(
					new Effect(GameData.ANIM_EFFECT_SOMARIA_BLOCK_DESTROY, DepthLayer.EffectBlockPoof),
					position, zPosition);
			}
		}
	}
}
