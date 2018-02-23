using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TileMagnetSpinner : Tile {

		private WindingOrder spinDirection;
		private int timer;
		private bool isRotating;
		private bool isPlayerAttached;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMagnetSpinner() {
			Graphics.SyncPlaybackWithRoomTicks = false;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		public void OnPlayerAttach() {
			isPlayerAttached = true;
		}

		public void OnPlayerDetach() {
			isPlayerAttached = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			spinDirection = WindingOrder.Clockwise;

			if (Polarity == Polarity.North)
				Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_NORTH);
			else 
				Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH);
			Graphics.SubStripIndex = (spinDirection == WindingOrder.Clockwise ? 0 : 1);

			fallsInHoles = false;

			isPlayerAttached = false;
			isRotating = false;
		}

		public override void Update() {
			Player player = RoomControl.Player;
			Vector2F moveAmount = Vector2F.Zero;
			moveAmount += Directions.ToVector(player.Direction);
			moveAmount -= Directions.ToVector(
				Directions.Add(player.Direction, 1, spinDirection));
			moveAmount *= 4.0f;

			if (isRotating) {
				timer++;
				if (timer == 15 ||
					timer == 15 + 4)
				{
					if (isPlayerAttached)
						RoomControl.Player.Position += moveAmount;
				}

				if (timer == 15 + 4 + 3) {
					timer = 0;
					isRotating = false;
					if (isPlayerAttached) {
						RoomControl.Player.Position += moveAmount;
						RoomControl.Player.Direction = 
							Directions.Add(player.Direction, 1, spinDirection);
					}
					if (Polarity == Polarity.North)
						Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_NORTH);
					else 
						Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH);
					isPlayerAttached = false;
				}
			}
			else {
				timer++;
				if (timer > 30) {
					isRotating = true;
					if (Polarity == Polarity.North)
						Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_NORTH_ROTATE);
					else 
						Graphics.PlayAnimation(GameData.ANIM_TILE_MAGNET_SPINNER_SOUTH_ROTATE);

					timer = 1;

					if (isPlayerAttached) {
						RoomControl.Player.Position += moveAmount;
					}
				}
			}

			base.Update();
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

		public WindingOrder SpinDirection {
			get { return spinDirection; }
		}

		public bool IsRotating {
			get { return isRotating; }
		}
	}
}
