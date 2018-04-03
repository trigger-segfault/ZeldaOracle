using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.GameStates.RoomStates {

	public class RoomStateColorBarrier : RoomState {

		private int timer;
		private PuzzleColor raiseColor;


		//-----------------------------------------------------------------------------
		// Contsants
		//-----------------------------------------------------------------------------
		
		/*private const int		PLAYER_EXIT_WALK_DISTANCE		= 15;
		private const int		SCREEN_SHAKE_DURATION			= 4;
		private const int		SCREEN_SHAKE_MAGNITUDE			= 3;
		private const int		BEFORE_TURN_DELAY				= 15;
		private const int		AFTER_TURN_DELAY				= 7;
		private static float[]	PLAYER_ROTATE_OFFSETS			= { 12, 10, 8, 2, 0 };
		private const int		PLAYER_ROTATE_OFFSET_INTERVAL	= 4;*/


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateColorBarrier(PuzzleColor raiseColor)
		{
			this.raiseColor		= raiseColor;
			this.updateRoom		= false;
			this.animateRoom	= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 0;

			AudioSystem.PlaySound(GameData.SOUND_BARRIER);

			RoomControl.Area.ColorSwitchColor = raiseColor;

			// Raise or lower all color barriers.
			foreach (Tile tile in RoomControl.TileManager.GetTiles()) {
				TileColorBarrier colorBarrier = tile as TileColorBarrier;
				if (colorBarrier != null) {
					if (colorBarrier.Color == raiseColor)
						colorBarrier.Raise();
					else
						colorBarrier.Lower();
				}
			}
		}

		public override void Update() {
			Player player = RoomControl.Player;

			// Update turnstile states.
			timer++;

			if (timer > 8) {
				gameControl.PopRoomState();
			}
		}
	}
}
