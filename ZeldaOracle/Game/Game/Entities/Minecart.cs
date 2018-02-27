using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {

	public class Minecart : Entity {
		
		private TileMinecartTrack minecartTrack;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Minecart(TileMinecartTrack minecartTrack) {
			this.minecartTrack = minecartTrack;

			// Graphics
			Graphics.DepthLayer	= DepthLayer.PlayerAndNPCs;
			Graphics.DrawOffset	= new Point2I(0, 0);
			centerOffset		= new Point2I(8, 8);

			// Physics
			Physics.Enable(PhysicsFlags.Solid);
			Physics.CollisionBox = new Rectangle2F(-1, 3, 18, 15);

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-1, 3, 18, 15);
		}

		
		//-----------------------------------------------------------------------------
		// Minecart Methods
		//-----------------------------------------------------------------------------

		/// <summary>Start moving since the player has entered this minecart.</summary>
		public void StartMoving() {
			// Sync with the player's position
			RoomControl.Player.AttachEntity(this);

			// Do not destroy upon room transitions
			IsPersistentBetweenRooms = true;
			
			// Enable interactions (with monsters)
			Interactions.InteractionType = InteractionType.MineCart;
			physics.IsSolid = false;

			// Remove the minecart from its track
			minecartTrack.SpawnsMinecart = false;

			// TODO: DepthLayer.PlayerAndNPCs
			Graphics.PlayAnimation(GameData.ANIM_MINECART);
		}

		/// <summary>Stop moving since the player has exited this minecart.</summary>
		public void StopMoving(TileMinecartTrack tile, Point2I tileLocation) {
			Parent.DetachEntity(this);
			IsPersistentBetweenRooms = false;
			Interactions.InteractionType = InteractionType.None;
			physics.IsSolid = true;

			// Add the minecart to the new track
			minecartTrack = tile;
			if (minecartTrack != null) {
				minecartTrack.SpawnsMinecart = true;
				position = minecartTrack.Position;
			}
			else {
				position = tileLocation * GameSettings.TILE_SIZE;
			}

			PlayStoppedAnimation();
		}

		/// <summary>Play the minecart animation for the direction of the minecart
		/// track.</summary>
		private void PlayStoppedAnimation() {
			if (minecartTrack != null) {
				if (minecartTrack.IsHorizontal)
					Graphics.PlayAnimation(GameData.SPR_MINECART_HORIZONTAL);
				else
					Graphics.PlayAnimation(GameData.SPR_MINECART_VERTICAL);
			}
			else {
				Graphics.PlayAnimation(GameData.SPR_MINECART_HORIZONTAL);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			PlayStoppedAnimation();
		}

		public override void Update() {
			// Check if the player is colliding with this minecart and should
			// jump into it
			Player player = RoomControl.Player;
			if (player.IsOnGround && player.Movement.IsMoving &&
				!player.StateParameters.ProhibitEnteringMinecart &&
				!player.StateParameters.ProhibitMovementControlOnGround &&
				player.Physics.Collisions.Any(c => c.Entity == this &&
					player.Movement.IsMovingInDirection(c.Direction)))
			{
				player.JumpIntoMinecart(this);
			}

			base.Update();
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileMinecartTrack TrackTile {
			get { return minecartTrack; }
		}
		
		public Point2I TrackTileLocation {
			get {
				if (minecartTrack == null)
					return RoomControl.GetTileLocation(Center);
				return minecartTrack.Location;
			}
		}
	}
}
