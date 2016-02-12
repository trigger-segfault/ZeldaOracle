using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {
	
	public class TilePathMove {
		public int Distance { get; set; }
		public float Speed { get; set; }
		public int Delay { get; set; }
		public int Direction { get; set; }

		public TilePathMove() {
			Distance	= 0;
			Speed		= 1.0f;
			Delay		= 0;
			Direction	= Directions.Right;
		}
	}

	public class TilePath {

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
		// Static Methods
		//-----------------------------------------------------------------------------
		
		public static TilePath Parse(string str) {
			string[] commands = str.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			if (commands.Length == 0)
				return null;

			TilePath path = new TilePath();
			float speed = 1.0f;
			int delay = 0;

			for (int i = 0; i < commands.Length; i++) {
				string[] tokens = commands[i].Split(new char[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 0)
					continue;

				if (tokens[0] == "right") {
					path.AddMove(Directions.Right, Int32.Parse(tokens[1]), speed, delay);
 					delay = 0;
				}
				else if (tokens[0] == "left") {
					path.AddMove(Directions.Left, Int32.Parse(tokens[1]), speed, delay);
 					delay = 0;
				}
				else if (tokens[0] == "up") {
					path.AddMove(Directions.Up, Int32.Parse(tokens[1]), speed, delay);
 					delay = 0;
				}
				else if (tokens[0] == "down") {
					path.AddMove(Directions.Down, Int32.Parse(tokens[1]), speed, delay);
 					delay = 0;
				}
				else if (tokens[0] == "speed")
					speed = Single.Parse(tokens[1]);
				else if (tokens[0] == "pause")
					delay += Int32.Parse(tokens[1]);
				else if (tokens[0] == "repeat")
					path.repeats = true;
			}

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
