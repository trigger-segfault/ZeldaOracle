using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {


	public class TileBridge : Tile, ZeldaAPI.Bridge {
		private enum TileBridgeState {
			Creating,
			Destroying,
			Created,
			Destroyed,
		};

		private bool isVertical;
		private int bridgeDirection;
		private int timer;
		private TileBridgeState state;
		private Point2I pieceLocation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileBridge() {
			// 10 frames between bridge pieces (10 frame delay before first bridge piece)
		}


		//-----------------------------------------------------------------------------
		// Bridge Methods
		//-----------------------------------------------------------------------------

		public void CreateBridge(bool instantaneous = false, bool rememberState = false) {
			if (bridgeDirection >= 0 && (state == TileBridgeState.Destroyed || state == TileBridgeState.Destroying)) {
				
				if (state == TileBridgeState.Destroying) {
					pieceLocation += Directions.ToPoint(bridgeDirection);
				}
				else {
					pieceLocation = Location + Directions.ToPoint(bridgeDirection);
				}

				state = TileBridgeState.Creating;
				timer = 0;
			}

			if (instantaneous) {
				while (state == TileBridgeState.Creating) {
					if (RoomControl.IsTileInBounds(pieceLocation) && GetConnectedTile(pieceLocation) == null) {
						TileData pieceTileData = Resources.GetResource<TileData>(
							isVertical ? "bridge_vertical" : "bridge_horizontal");
						Tile pieceTile = Tile.CreateTile(pieceTileData);
						RoomControl.PlaceTileOnHighestLayer(pieceTile, pieceLocation);
						pieceLocation += Directions.ToPoint(bridgeDirection);
					}
					else {
						state = TileBridgeState.Created;
						break;
					}
				}
			}

			if (state == TileBridgeState.Created || state == TileBridgeState.Creating) {
				Properties.Set("built", true);
			}
		}

		public void DestroyBridge(bool instantaneous = false, bool rememberState = false) {
			if (bridgeDirection >= 0 && (state == TileBridgeState.Created || state == TileBridgeState.Creating)) {

				if (state == TileBridgeState.Creating) {
					pieceLocation -= Directions.ToPoint(bridgeDirection);
					if (pieceLocation == Location)
						return;
				}
				else {
					// Find the second to last tile in the bridge.
					pieceLocation = new Point2I(-1, -1);
					TileBridge pieceTile = GetConnectedTile(bridgeDirection);

					while (pieceTile != null) {
						TileBridge nextTile = pieceTile.GetConnectedTile(bridgeDirection);
						if (nextTile == null)
							break;
						pieceLocation = pieceTile.Location;
						pieceTile = nextTile;
					}
				}

				if (pieceLocation.X > 0 && pieceLocation.Y > 0) {
					state = TileBridgeState.Destroying;
					timer = 0;
				}
			}
			
			if (instantaneous) {
				while (state == TileBridgeState.Destroying) {
					TileBridge pieceTile = GetConnectedTile(pieceLocation);
					if (pieceTile != null)
						RoomControl.RemoveTile(pieceTile);
					pieceLocation -= Directions.ToPoint(bridgeDirection);
					if (pieceLocation == Location) {
						state = TileBridgeState.Destroyed;
						break;
					}
				}
			}

			if (state == TileBridgeState.Destroyed || state == TileBridgeState.Destroying) {
				Properties.Set("built", false);
			}
		}

		private int GetBuildDirection() {
			int axis = (isVertical ? Axes.Y : Axes.X);
			if (GetConnectedTile(axis) != null && GetConnectedTile(axis + 2) == null)
				return axis;
			if (GetConnectedTile(axis + 2) != null && GetConnectedTile(axis) == null )
				return (axis + 2);
			return -1;
		}

		private TileBridge GetConnectedTile(int direction) {
			return GetConnectedTile(Location + Directions.ToPoint(direction));
		}

		private TileBridge GetConnectedTile(Point2I location) {
			foreach (Tile tile in RoomControl.TileManager.GetTilesAtLocation(location)) {
				if ((tile is TileBridge) && ((TileBridge) tile).isVertical == isVertical)
					return (TileBridge) tile;
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Zelda API Methods
		//-----------------------------------------------------------------------------
		
		void ZeldaAPI.Bridge.BuildBridge(bool instantaneous, bool rememberState) {
			CreateBridge(instantaneous, rememberState);
		}

		void ZeldaAPI.Bridge.DestroyBridge(bool instantaneous, bool rememberState) {
			DestroyBridge(instantaneous, rememberState);
		}

		bool ZeldaAPI.Bridge.IsBridgeBuilt {
			get { return (state == TileBridgeState.Created || state == TileBridgeState.Creating); }
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			fallsInHoles	= false;
			isVertical		= Properties.GetBoolean("vertical", false);
			bridgeDirection	= Properties.GetInteger("bridge_direction", -1);

			timer = 0;
			pieceLocation = new Point2I(-1, -1);
		}

		public override void OnPostInitialize() {
			base.OnPostInitialize();
			
			// Check if the bridge is built.
			if (bridgeDirection >= 0 && GetConnectedTile(bridgeDirection) != null)
				state = TileBridgeState.Created;
			else
				state = TileBridgeState.Destroyed;
			
			if (Properties.GetBoolean("built", false))
				CreateBridge(true);
			else
				DestroyBridge(true);
		}

		public override void Update() {
			base.Update();
			
			if (state == TileBridgeState.Creating) {
				timer++;
				if (timer >= 10) {
					timer = 0;

					if (RoomControl.IsTileInBounds(pieceLocation) && GetConnectedTile(pieceLocation) == null) {
						AudioSystem.PlaySound(GameData.SOUND_BARRIER);
						TileData pieceTileData = Resources.GetResource<TileData>(
							isVertical ? "bridge_vertical" : "bridge_horizontal");
						Tile pieceTile = Tile.CreateTile(pieceTileData);
						RoomControl.PlaceTileOnHighestLayer(pieceTile, pieceLocation);
						pieceLocation += Directions.ToPoint(bridgeDirection);
					}
					else {
						state = TileBridgeState.Created;
					}
				}
			}
			else if (state == TileBridgeState.Destroying) {
				timer++;
				if (timer >= 10) {
					timer = 0;
					TileBridge pieceTile = GetConnectedTile(pieceLocation);

					if (pieceTile != null) {
						RoomControl.RemoveTile(pieceTile);
						AudioSystem.PlaySound(GameData.SOUND_BARRIER);
					}

					pieceLocation -= Directions.ToPoint(bridgeDirection);

					if (pieceLocation == Location) {
						state = TileBridgeState.Destroyed;
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsVertical {
			 get { return isVertical; }
		}
	}
}
