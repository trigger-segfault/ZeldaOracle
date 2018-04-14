using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {
	
	public class TilePathMove {
		public int Distance { get; set; }
		public float Speed { get; set; }
		public int Delay { get; set; }
		public Direction Direction { get; set; }
		public bool HasMovement {
			get { return Direction.IsValid && Distance > 0; }
		}
		
		public TilePathMove() {
			Distance	= 0;
			Speed		= 1.0f;
			Delay		= 0;
			Direction	= Direction.Right;
		}
	}

	public class TilePath {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const string PauseCmd = "pause";
		public const string RepeatCmd = "repeat";
		public const string SpeedCmd = "speed";

		public const int MaxDistance = 50;
		public const int MaxPause = 10000;
		public const float MaxSpeed = 16f;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private List<TilePathMove> moves;
		private bool repeats;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePath() {
			moves	= new List<TilePathMove>();
			repeats	= false;
		}
		

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddMove(int direction, int distance, float speed, int delay) {
			moves.Add(new TilePathMove() {
				Direction = direction,
				Distance = distance,
				Speed = speed,
				Delay = delay,
			});
		}



		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		public override string ToString() {
			string str = "";

			// The current speed, only add a speed command when it changes.
			float speed = 1f;

			for (int i = 0; i < moves.Count; i++) {
				TilePathMove move = moves[i];
				if (move.Delay > 0)
					str += TilePath.PauseCmd + " " + move.Delay + "; ";
				if (move.Speed != speed)
					str += TilePath.SpeedCmd + " " + move.Speed + "; ";
				// Don't add the move if its invalid or has no distance
				if (move.Distance > 0 && move.Direction.IsValid)
					str += move.Direction.ToString() + " " + move.Distance + "; ";
			}

			// Add repeat to the end
			if (repeats)
				str += TilePath.RepeatCmd + ";";
			// Trim the last space off of the end
			else
				str = str.TrimEnd();

			return str;
		}

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static TilePath Parse(string str) {
			string[] commands = str.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			if (commands.Length == 0)
				return null;

			TilePath path = new TilePath();
			float speed = 1.0f;
			int delay = 0;
			int extraDelay;

			for (int i = 0; i < commands.Length; i++) {
				string[] tokens = commands[i].Split(new char[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 0)
					continue;

				string cmd = tokens[0];
				string param = (tokens.Length > 1 ? tokens[1] : "");

				Direction direction;
				if (Direction.TryParse(cmd, false, out direction)) {
					int distance;
					if (int.TryParse(param, out distance)) {
						path.AddMove(direction, distance, speed, delay);
						delay = 0;
					}
				}
				else if (cmd == TilePath.SpeedCmd)
					float.TryParse(param, out speed);
				else if (cmd == TilePath.PauseCmd) {
					if (int.TryParse(param, out extraDelay))
						delay += extraDelay;
				}
				else if (cmd == TilePath.RepeatCmd)
					path.repeats = true;
			}

			// Add a no-movement command for the final delay
			if (delay > 0)
				path.AddMove(Direction.Invalid, 0, 0, delay);

			if (path.moves.Count == 0)
				return null;
			return path;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool Repeats {
			get { return repeats; }
			set { repeats = value; }
		}

		public List<TilePathMove> Moves {
			get { return moves; }
		}
	}
}
