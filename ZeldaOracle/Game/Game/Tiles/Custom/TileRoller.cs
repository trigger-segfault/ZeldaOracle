using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles.Custom {

	public class TileRoller : Tile {
		
		private Point2I startLocation;
		private int returnTimer;
		private int pushTimer;
		private bool pushed;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileRoller() {
			fallsInHoles = false;
			// TODO: Rollers can't be sword-stabbed, aren't pushable diagonally, and are only solid to the player.
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			startLocation = Location;
			returnTimer = 0;
			pushTimer = 0;
			pushed = false;

			Graphics.SyncPlaybackWithRoomTicks = false;
			Graphics.PlayAnimation(TileData.SpriteList[1]);
			Graphics.AnimationPlayer.SkipToEnd();

			CollisionModel = new CollisionModel(
				new Rectangle2I(Point2I.FromBoolean(!IsVertical, Size[!IsVertical], 1) *
					GameSettings.TILE_SIZE));
		}

		public override bool OnPush(Direction direction, float movementSpeed) {
			// Don't use the Tile's built in pushing
			return false;
		}

		public override void OnPushing(Direction direction) {
			if (!IsMoving && IsPushableDirection(direction) &&
				GameControl.Inventory.IsWeaponButtonDown("bracelet"))
			{
				ResetReturnTimer();
				// Let the update method know that we're still pushing the tile
				pushed = true;
				if (pushTimer == PushDelay) {
					MoveRoller(direction);
				}
				else {
					pushTimer++;
				}
			}
		}

		public override void OnCompleteMovement() {
			base.OnCompleteMovement();
			ResetReturnTimer();
		}

		public override void Update() {
			base.Update();

			if (!IsMoving) {
				if (!pushed)
					pushTimer = 0;

				if (StartPosition != CurrentPosition) {
					int newLayer;
					if (IsMoveObstructed(ReturnDirection, out newLayer)) {
						// Reset the return timer while it can't roll back.
						ResetReturnTimer();
					}
					else {
						if (returnTimer == 0)
							MoveRoller(ReturnDirection);
						else
							returnTimer--;
					}
				}
			}
			
			pushed = false;
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			for (int i = 1; i < Size[!IsVertical]; i++) {
				g.DrawAnimationPlayer(Graphics.AnimationPlayer,
					Graphics.AnimationPlayer.PlaybackTime,
					Position + Point2I.FromBoolean(!IsVertical, i * GameSettings.TILE_SIZE),
					Graphics.DepthLayer, Position);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Moves the roller.
		private bool MoveRoller(Direction direction) {
			if (direction.IsVertical != IsVertical)
				return false;

			if (Move(direction, 1, GameSettings.TILE_ROLLER_MOVE_SPEED)) {
				AudioSystem.PlaySound(GameData.SOUND_BLUE_ROLLER);
				Graphics.PlayAnimation(TileData.SpriteList[1]);
				Graphics.SubStripIndex = (direction <= 1 ? 0 : 1);
				return true;
			}
			return false;
		}

		private bool IsPushableDirection(Direction direction) {
			if (direction.IsVertical != IsVertical)
				return false;
			if (direction == Direction.Right || direction == Direction.Down)
				return (CurrentPosition >= StartPosition);
			else
				return (CurrentPosition <= StartPosition);
		}

		private void ResetReturnTimer() {
			returnTimer = 60;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool vertical = args.Properties.Get<bool>("vertical", false);
			int length = GMath.Max(1, args.Properties.Get<Point2I>("size", Point2I.One)[!vertical]);
			for (int i = 0; i < length; i++) {
				ISprite sprite = args.Tile.Sprite;
				g.DrawSprite(
					sprite,
					args.SpriteSettings,
					args.Position + Point2I.FromBoolean(!vertical, i * GameSettings.TILE_SIZE),
					args.Color);
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.Movable | TileFlags.NotGrabbable |
				TileFlags.NoClingOnStab;
			
			data.Properties.Set("vertical", false)
				.SetDocumentation("Vertical", "Roller", "The roller rolls vertically.").Hide();
			data.Properties
				.SetDocumentation("size", "Length", "single_axis", "!vertical:1", "Roller", "The length of the roller in tiles.");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private bool IsVertical {
			get { return Properties.Get<bool>("vertical", false); }
		}

		private Direction ReturnDirection {
			get { return Direction.FromPoint(startLocation - Location); }
		}

		private int StartPosition {
			get { return startLocation[IsVertical]; }
		}

		private int CurrentPosition {
			get { return Location[IsVertical]; }
		}
	}
}
