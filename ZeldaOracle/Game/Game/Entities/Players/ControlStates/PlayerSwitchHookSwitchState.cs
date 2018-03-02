using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSwitchHookSwitchState : PlayerState {

		private SwitchHookProjectile hookProjectile;
		private object hookedObject;
		private Entity hookedEntity;
		private bool isSwitched;
		private float raisedZPosition;
		private Point2I tileSwitchLocation;
		private int direction;
		private Vector2F playerPosition;
		private Vector2F hookedEntityPosition;
		private Vector2F hookProjectilePosition;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwitchHookSwitchState() {
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableGravity					= true;
			StateParameters.DisableSurfaceContact			= true;
			StateParameters.DisablePlayerControl			= true;
		}


		//-----------------------------------------------------------------------------
		// Switching
		//-----------------------------------------------------------------------------

		public void SetupSwitch(object hookedObject, SwitchHookProjectile hookProjectile, int direction) {
			this.hookedObject = hookedObject;
			this.hookProjectile = hookProjectile;
			this.direction = direction;
		}

		private bool CanTileLandAtLocation(Point2I location) {
			if (!player.RoomControl.IsTileInBounds(location))
				return false;
			Tile checkTile = player.RoomControl.GetTopTile(location);
			if (checkTile == null)
				return true;
			return (checkTile.Layer != player.RoomControl.Room.TopLayer &&
					!checkTile.IsNotCoverable && !checkTile.IsSolid	&& !checkTile.IsHoleWaterOrLava);
		}

		private bool WillTileBreakAtLocation(Point2I location) {
			if (!player.RoomControl.IsTileInBounds(location))
				return true;
			Tile checkTile = player.RoomControl.GetTopTile(location);
			if (checkTile == null)
				return false;
			return (checkTile.Layer == player.RoomControl.Room.TopLayer ||
					!checkTile.IsCoverableByBlock || checkTile.IsHoleWaterOrLava);
		}

		private void SwitchPositions() {
			isSwitched = true;
			
			// Swap player and entity positions
			Vector2F tempPosition	= player.Position;
			player.Position			= hookedEntityPosition;
			hookedEntity.Position	= playerPosition;
			hookProjectile.Position	= hookedEntity.Center;
			player.Direction		= Directions.Reverse(player.Direction);

			// Align positions to grid for tiles
			if (hookedObject is Tile) {
				Vector2F center = (tileSwitchLocation * GameSettings.TILE_SIZE) + new Vector2F(8, 8);
				hookedEntity.SetPositionByCenter(center);
			}

			hookedEntityPosition	= hookedEntity.Position;
			playerPosition			= player.Position;
			hookProjectilePosition	= hookProjectile.Position;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			direction		= player.Direction;
			isSwitched		= false;
			raisedZPosition	= 0;
			
			player.Graphics.PlayAnimation(
				player.StateParameters.PlayerAnimations.Throw);
			
			if (hookedObject is Entity) {
				hookedEntity = (Entity) hookedObject;
			}
			else if (hookedObject is Tile) {
				Tile hookedTile = (Tile) hookedObject;
				CarriedTile carriedTile = new CarriedTile(hookedTile);
				carriedTile.SetPositionByCenter(hookedTile.Center);
				carriedTile.Initialize(player.RoomControl);
				hookedEntity = carriedTile;
				player.RoomControl.RemoveTile(hookedTile);

				// Find location for tile to land at
				tileSwitchLocation = player.RoomControl.GetTileLocation(player.Center);
				int syncAxis = Axes.GetOpposite(Directions.ToAxis(direction));
				tileSwitchLocation[syncAxis] = hookedTile.Location[syncAxis];
				
				// Check if there is a hazard tile
				if (hookedTile.StaysOnSwitch && !CanTileLandAtLocation(tileSwitchLocation)) {
					// Attempt to move landing location one tile further to avoid hazard
					int checkDir = Directions.Reverse(direction);
					Point2I newSwitchLocation = tileSwitchLocation + Directions.ToPoint(checkDir);
					if (CanTileLandAtLocation(newSwitchLocation))
						tileSwitchLocation = newSwitchLocation;
				}

				// Spawn drops as the tile is picked up
				if (hookedTile.StaysOnSwitch)
					hookedTile.SpawnDrop();
			}

			hookedEntity.RemoveFromRoom();
			hookProjectile.Position = hookedEntity.Center;
			hookedEntityPosition	= hookedEntity.Position;
			playerPosition			= player.Position;
			hookProjectilePosition	= hookProjectile.Position;
			
			AudioSystem.PlaySound(GameData.SOUND_SWITCH_HOOK_SWITCH);
		}
		
		public override void OnEnd(PlayerState newState) {
			// Destroy the hook projectile
			if (hookProjectile != null && !hookProjectile.IsDestroyed)
				hookProjectile.Destroy();
		}

		public override void Update() {
			base.Update();

			if (!isSwitched) {
				// Rise into the air
				raisedZPosition += GameSettings.SWITCH_HOOK_LIFT_SPEED;
				
				// Perform switch
				if (raisedZPosition >= GameSettings.SWITCH_HOOK_LIFT_HEIGHT) {
					raisedZPosition = GameSettings.SWITCH_HOOK_LIFT_HEIGHT;
					SwitchPositions();
				}
			}
			else {
				// Lower to the ground
				raisedZPosition -= GameSettings.SWITCH_HOOK_LIFT_SPEED;

				if (raisedZPosition <= 0.0f) {
					raisedZPosition = 0.0f;

					// Destroy the hook projectile
					hookProjectile.Destroy();

					// Place or break the hooked entity/tile
					hookedEntity.ZPosition = 0.0f;
					if (hookedObject is Tile) {
						Tile tile = hookedObject as Tile;
						if (tile.BreaksOnSwitch || WillTileBreakAtLocation(tileSwitchLocation)) {
							(hookedEntity as CarriedTile).Break();
						}
						else {
							player.RoomControl.PlaceTileOnHighestLayer(tile, tileSwitchLocation);
						}
					}
					else {
						player.RoomControl.SpawnEntity(hookedEntity);
						hookedEntity.OnLand();
					}

					End();
				}
			}
				
			// Synchronize positions of lifted entities
			if (!RoomControl.IsSideScrolling) {
				player.Position				= playerPosition;
				hookProjectile.Position		= hookProjectilePosition;
				hookedEntity.Position		= hookedEntityPosition;
				player.ZPosition			= raisedZPosition;
				hookProjectile.ZPosition	= raisedZPosition;
				hookedEntity.ZPosition		= raisedZPosition;
			}
			else {
				Vector2F liftOffset = new Vector2F(0, -raisedZPosition);
				player.Position			= playerPosition + liftOffset;
				hookProjectile.Position	= hookProjectilePosition + liftOffset;
				hookedEntity.Position	= hookedEntityPosition + liftOffset;
			}
		}

		public override void DrawOver(RoomGraphics g) {
			hookedEntity.Graphics.Draw(g, DepthLayer.RisingTile);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SwitchHookProjectile Hook {
			get { return hookProjectile; }
			set { hookProjectile = value; }
		}
	}
}
