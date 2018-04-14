using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Common.Audio;
using System.ComponentModel;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public enum WarpType {
		Tunnel		= 0,
		Entrance	= 1,
		Stairs		= 2,
		
		[Browsable(false)]
		Count		= 3,
	}

	public class WarpAction : ActionTile {

		private WarpType warpType;
		private bool warpEnabled;
		private Direction edgeDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WarpAction() {
		}


		//-----------------------------------------------------------------------------
		// Warp Accessors
		//-----------------------------------------------------------------------------

		// Find the action tile for the this warp point's destination.
		public ActionTileDataInstance FindDestinationPoint() {
			string warpID = properties.Get<string>("destination_warp_point", "?");
			string warpLevelID = Properties.Get<string>("destination_level",
				RoomControl.Level.ID);
			if (warpID.Length == 0 || warpLevelID.Length == 0)
				return null;
			Level warpLevel = RoomControl.GameControl.World.GetLevel(warpLevelID);
			if (warpLevel == null)
				return null;
			return warpLevel.FindActionTileByID(warpID);
		}


		//-----------------------------------------------------------------------------
		// Warping
		//-----------------------------------------------------------------------------

		// This method is called when a room is entered through this warp point.
		public void SetupPlayerInRoom() {
			Player player = RoomControl.Player;
			warpEnabled = false;

			Vector2F center = position + new Vector2F(8, 8);
			player.SetPositionByCenter(center);
			
			// Get the natural state to be in when spawning
			player.RequestSpawnNaturalState(true);

			// Position the player.
			if (warpType == WarpType.Entrance) {
				if (edgeDirection == Direction.Down)
					player.Position += edgeDirection.ToVector(8.0f);
				else
					player.Position += edgeDirection.ToVector(16.0f);
				player.Direction = edgeDirection.Reverse();
			}
			else {
				int faceDirection = Properties.Get<int>("face_direction", Direction.Down);
				player.Direction = faceDirection;
			}

			// Setup the player's state.
			player.InterruptWeapons();
			player.StopPushing();

			player.Graphics.PlayAnimation(player.MoveAnimation);
		}

		// Create the game-state when exiting a room through this warp point.
		public GameState CreateExitState() {
			if (warpType == WarpType.Entrance)
				return RoomEnterExitState.CreateExit(edgeDirection, 25);
			return null;
		}
		
		// Create the game-state when entering a room through this warp point.
		public GameState CreateEnterState() {
			int distance = 19;
			if (edgeDirection == Direction.Down)
				distance += 8;
			if (warpType == WarpType.Entrance)
				return RoomEnterExitState.CreateEnter(
					edgeDirection.Reverse(), distance, null);
			return null;
		}

		// Create the room transition state for this warp point.
		public RoomTransition CreateTransition(ActionTileDataInstance destination) {
			int dir = destination.Properties.Get<int>("face_direction", Direction.Down);
			if (warpType == WarpType.Stairs)
				return new RoomTransitionFade();
			return new RoomTransitionSplit();
		}

		// Warp to the destination point.
		public void Warp(int warpDirection) {
			ActionTileDataInstance destination = FindDestinationPoint();

			if (destination != null) {
				AudioSystem.PlaySound(GameData.SOUND_ROOM_EXIT);
				RoomControl.Warp(this, destination);
				RoomControl.Player.InterruptWeapons();
			}
			else {
				Console.WriteLine("Invalid warp destination!");
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			properties.Print();
			
			warpType        = Properties.GetEnum("warp_type", WarpType.Tunnel);
			collisionBox	= new Rectangle2I(2, 6, 12, 12);
			warpEnabled		= !IsTouchingPlayer();

			// Find the closest room edge.
			edgeDirection = -1;
			Rectangle2I roomBounds = RoomControl.RoomBounds;
			Rectangle2I myBox = new Rectangle2I((int) position.X, (int) position.Y, 16, 16);
			int minDist = -1;
			for (int dir = 0; dir < 4; dir++) {
				int dist = GMath.Abs(myBox.GetEdge(dir) - roomBounds.GetEdge(dir));
				if (dist < minDist || minDist < 0) {
					edgeDirection = dir;
					minDist = dist;
				}
			}

			// Make sure we know if the player respawns on top of this warp point.
			RoomControl.PlayerRespawn += delegate(Player player) {
				warpEnabled = !IsTouchingPlayer();
			};

			// For entrance warp points, intercept room transitions in order to warp.
			RoomControl.RoomTransitioning += delegate(int direction) {
				if (warpType == WarpType.Entrance && direction == edgeDirection && IsTouchingPlayer()) {
					RoomControl.CancelRoomTransition();
					Warp(direction);
				}
			};
		}

		public override void Update() {
			base.Update();

			if (warpType == WarpType.Entrance) {
				warpEnabled = true;
			}
			else if (IsTouchingPlayer()) {
				if (!RoomControl.Player.StateParameters.ProhibitWarping &&
					warpEnabled && RoomControl.Player.IsOnGround)
				{
					Warp(RoomControl.Player.Direction);
					warpEnabled = false;
				}
			}
			else {
				warpEnabled = true;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			WarpType warpType = args.Properties.GetEnum<WarpType>("warp_type", WarpType.Tunnel);
			ISprite sprite = null;
			if (warpType == WarpType.Entrance)
				sprite = GameData.SPR_ACTION_TILE_WARP_ENTRANCE;
			else if (warpType == WarpType.Tunnel)
				sprite = GameData.SPR_ACTION_TILE_WARP_TUNNEL;
			else if (warpType == WarpType.Stairs)
				sprite = GameData.SPR_ACTION_TILE_WARP_STAIRS;
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteSettings,
					args.Position,
					args.Color);
			}
		}

		/// <summary>Initializes the properties and events for the action type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.Properties.SetEnumStr("warp_type", WarpType.Tunnel)
				.SetDocumentation("Warp Type", "enum", typeof(WarpType), "Warp", "The type of warp point.");
			data.Properties.Set("destination_level", "")
				.SetDocumentation("Destination Level", "level", "", "Warp", "The level where the destination point is in.");
			data.Properties.Set("destination_warp_point", "")
				.SetDocumentation("Destination Warp Point", "warp", "destination_level", "Warp", "The id of the warp point destination.");
			data.Properties.Set("face_direction", Direction.Down)
				.SetDocumentation("Face Direction", "direction", "", "Warp", "The direction the player should face when entering a room through this Warp Point.");
			data.Properties.Set("show_area_name", false)
				.SetDocumentation("Show Area Name", "Warp", "True if a message displaying the area name is shown upon entering.");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets if the area name should be shown upon warping.</summary>
		public bool ShowAreaName {
			get { return properties.Get("show_area_name", false); }
			set { properties.Set("show_area_name", value); }
		}
	}
}
