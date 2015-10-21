using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSwitchHookState : PlayerState {

		private SwitchHookProjectile hookProjectile;
		private bool isSwitching;
		private bool isSwitched;
		private float liftZPosition;
		private object hookedObject;
		private Entity hookedEntity;
		private Point2I tileSwitchLocation;
		private int direction;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwitchHookState() {

		}
		


		//-----------------------------------------------------------------------------
		// Switching
		//-----------------------------------------------------------------------------

		public void BeginSwitch(object hookedObject) {
			this.hookedObject	= hookedObject;
			this.isSwitching	= true;
			this.isSwitched		= false;
			this.liftZPosition	= 0;

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

				// Find location for tile to land at.
				tileSwitchLocation = player.RoomControl.GetTileLocation(player.Center);
				tileSwitchLocation[1 - (direction % 2)] = hookedTile.Location[1 - (direction % 2)];

				// Check if there is a hazard tile.
				if (hookedTile.StaysOnSwitch && !CanTileLandAtLocation(tileSwitchLocation)) {
					// Attempt to move landing location one tile further to avoid hazard.
					int checkDir = Directions.Reverse(direction);
					Point2I newSwitchLocation = tileSwitchLocation + Directions.ToPoint(checkDir);
					if (CanTileLandAtLocation(newSwitchLocation))
						tileSwitchLocation = newSwitchLocation;
				}
			}
			
			hookProjectile.Position = hookedEntity.Center;

			player.Physics.HasGravity	= false;
			player.IsStateControlled	= true;
		}

		private bool CanTileLandAtLocation(Point2I location) {
			if (!player.RoomControl.IsTileInBounds(location))
				return false;
			Tile checkTile = player.RoomControl.GetTopTile(location);
			return (checkTile == null ||
					(checkTile.Layer != player.RoomControl.Room.LayerCount - 1 &&
					checkTile.IsCoverable &&
					!checkTile.IsSolid &&
					!checkTile.IsHole &&
					!checkTile.IsWater &&
					!checkTile.IsLava));
		}

		private bool WillTileBreakAtLocation(Point2I location) {
			if (!player.RoomControl.IsTileInBounds(location))
				return true;
			Tile checkTile = player.RoomControl.GetTopTile(location);
			if (checkTile == null)
				return false;
			return (checkTile.Layer == player.RoomControl.Room.LayerCount - 1 ||
					!checkTile.IsCoverable ||
					checkTile.IsStairs ||
					checkTile.IsLadder ||
					checkTile.IsSolid ||
					checkTile.IsHole ||
					checkTile.IsWater ||
					checkTile.IsLava);
		}

		private void SwitchPositions() {
			isSwitched = true;

			Vector2F tempPosition = player.Position;
			player.Position = hookedEntity.Position;
			hookedEntity.Position = tempPosition;

			// Align positions to grid for tiles.
			if (hookedObject is Tile) {
				Vector2F center = (tileSwitchLocation * GameSettings.TILE_SIZE) + new Vector2F(8, 8);
				hookedEntity.SetPositionByCenter(center);
			}

			hookProjectile.Position = hookedEntity.Center;
			player.Direction = Directions.Reverse(player.Direction);
			hookProjectile.OnSwitchPositions();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			isSwitching		= false;
			hookedObject	= null;
			hookedEntity	= null;
			isSwitched		= false;
			liftZPosition	= 0;
			direction		= player.Direction;

			player.Movement.MoveCondition = PlayerMoveCondition.NoControl;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Physics.HasGravity		= true;
			player.IsStateControlled		= false;
			player.Movement.MoveCondition	= PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();

			if (isSwitching) {
				if (!isSwitched) {
					// Rise.
					liftZPosition += GameSettings.SWITCH_HOOK_LIFT_SPEED;
					if (liftZPosition >= GameSettings.SWITCH_HOOK_LIFT_HEIGHT) {
						liftZPosition = GameSettings.SWITCH_HOOK_LIFT_HEIGHT;

						// Perform switch
						SwitchPositions();
					}
				}
				else {
					// Lower.
					liftZPosition -= GameSettings.SWITCH_HOOK_LIFT_SPEED;
					if (liftZPosition <= 0.0f) {
						liftZPosition = 0.0f;

						hookProjectile.Destroy();

						if (hookedObject is Tile) {
							Tile tile = hookedObject as Tile;
							if (tile.BreaksOnSwitch || WillTileBreakAtLocation(tileSwitchLocation)) {
								(hookedEntity as CarriedTile).Break();
							}
							else {
								player.RoomControl.PlaceTileOnHighestLayer(tile, tileSwitchLocation);
							}
						}

						// Return to normal.
						player.BeginNormalState();
					}
				}
				
				// Synchronize z-positions of lifted entities.
				player.ZPosition			= liftZPosition;
				hookProjectile.ZPosition	= liftZPosition;
				hookedEntity.ZPosition		= liftZPosition;
			}
			else if (hookProjectile.IsDestroyed) {
				player.BeginNormalState();
			}
		}

		public override void DrawOver(Common.Graphics.Graphics2D g) {
			if (isSwitching) {
				hookedEntity.Draw(g);
			}
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
