using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerPushState : PlayerState {

		private int pushTimer;
		private Tile pushTile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerPushState() {
			StateParameters.PlayerAnimations.Default = GameData.ANIM_PLAYER_PUSH;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Get the tile the player is currently pushing.</summary>
		public Tile GetPushTile() {
			// Make sure the player is in the proper state
			if (player.IsInAir ||
				player.StateParameters.ProhibitPushing ||
				!player.Movement.IsMoving)
				return null;

			// Make sure the player is colliding with a pushable tile
			Collision collision =
				player.Physics.GetCenteredCollisionInDirection(player.Direction);
			if (collision == null || !collision.IsTile || 
				collision.Tile.IsMoving || collision.Tile.IsNotPushable)
				return null;

			return collision.Tile;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {
			pushTimer = 0;
			pushTile = null;
		}
		
		public override void Update() {
			base.Update();
			
			// Get the tile the player is currently pushing
			Tile newPushTile = GetPushTile();

			if (newPushTile == null) {
				End();
			}
			else {
				// Reset push timer if the player is pushing a different tile
				if (newPushTile != pushTile && pushTile != null)
					pushTimer = 0;
				pushTile = newPushTile;

				pushTimer++;
				if (pushTimer > pushTile.PushDelay) {
					// Attempt to move the tile
					if (pushTile.OnPush(player.Direction, player.PushSpeed))
						pushTimer = 0;
					else
						pushTile.OnPushing(player.Direction);
				}
				else {
					pushTile.OnPushing(player.Direction);
				}
			}
		}
	}
}
