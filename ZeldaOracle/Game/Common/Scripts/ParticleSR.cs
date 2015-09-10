using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Scripts;
using Color			= ZeldaOracle.Common.Graphics.Color;


namespace ZeldaOracle.Common.Scripts {
/** <summary>
 * Script reader for sprite sheets. The script can contain
 * information for multiple sprite sheets with corresponding
 * images in the content folder.
 *
 * FORMAT:
 *
 * @spritesheet [name]
 * @size [width] [height]
 * @sprite [frame_x] [frame_y] [frame_width] [frame_height] [offset_x] [offset_y] [source_width] [source_height]
 * @end
 *
 * Note: All declared sprites must be within the
 * @spritesheet and @end commands.
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleSR : ScriptReader {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The name of the default particles sprite sheet. </summary> */
	public const string DefaultParticleSheet = "sheet_particles";

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Shared
	/** <summary> The order of the comment sections. </summary> */
	internal static List<string> sectionOrder;

	public static bool UsingDegrees = true;

	/** <summary> The particle type being read. </summary> */
	private ParticleType particle;
	/** <summary> The emitter type being read. </summary> */
	private ParticleEmitter emitter;
	/** <summary> The effect type being read. </summary> */
	private ParticleEffectType effect;
	/** <summary> The sprite sheet of the type being read. </summary> */
	private SpriteSheet sheet;
	/** <summary> The current comment section. </summary> */
	private string section;

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Resets the reader for the next type. </summary> */
	private void Reset() {
		particle	= null;
		emitter		= null;
		effect		= null;
		sheet		= Resources.GetSpriteSheet(DefaultParticleSheet);
	}

	private double ConvertToDegrees(double radians) {
		if (UsingDegrees)
			return radians;
		return GMath.ConvertToDegrees(radians);
	}

	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		Reset();
		sectionOrder = new List<string>();
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		Reset();
	}
	/** <summary> Attempt to add a completed word, if it is not empty. </summary> */
	protected override String CompleteWord(string word, List<string> words) {
		if (word.Length > 0)
			words.Add(word);
		return "";
	}
	/** <summary> Read a single line of the script. </summary> */
	protected override void ReadLine(string line) {
		List<string> words = new List<string>();
		string word = "";
		bool quotes = false;
		bool commentLine = false;
		bool sectionLine = false;

		// Parse line character by character.
		for (int i = 0; i < line.Length; i++) {
			char c = line[i];

			// Parse quotes.
			if (quotes) {
				// Closing quotes.
				if (c == '\"') {
					quotes = false;
					words.Add(word);
					word = "";
				}
				else
					word += c;
			}

			// Whitespace.
			else if (c == ' ' || c == '\t')
				word = CompleteWord(word, words);

			// Single-line comment.
			else if (c == '#') {
				if (i == 0)
					commentLine = true;
				if (i + 1 == line.Length && commentLine)
					sectionLine = true;
				word = CompleteWord(word, words);
				//break;
			}

			// Opening quotes.
			else if (c == '\"')
				quotes = true;

			// Other character.
			else
				word += c;
		}

		word = CompleteWord(word, words);

		if (words.Count >= 1) {
			// Remove regions
			if (words[0] == "[region]") {
				words.Clear();
			}
			else if (words[0] == "[endregion]") {
				words.Clear();
			}

			// Read section headers
			else if (sectionLine) {
				section = words[0];
				for (int i = 1; i < words.Count; i++) {
					section += " " + words[i];
				}
				sectionOrder.Add(section);
				words.Clear();
			}
		}

		// Command.
		if (words.Count > 0 && words[0].StartsWith("@")) {
			string command = words[0].Remove(0, 1);
			words.RemoveAt(0);
			ReadCommand(command, words);
		}
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		if (command == "end") {
			if (particle != null)
				Resources.AddParticleType(particle);
			if (emitter != null)
				Resources.AddParticleEmitter(emitter);
			if (effect != null)
				Resources.AddParticleEffect(effect);

			Reset();
		}

		// PARTICLES.
		else if (particle != null) {
			if (command == "clone") {
				particle = new ParticleType(particle.Name,
					Resources.GetParticleType(args[0]));
				particle.Comments = section;
			}

			// General.
			else if (command == "life") {
				particle.SetLifeSpan(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "fade") {
				particle.FadeDelay = Double.Parse(args[0]);
			}

			// Scale.
			else if (command == "scale_init") {
				particle.InitialScale = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "scale_increase") {
				particle.ScaleIncrease = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "scale_jitter") {
				particle.ScaleJitter = Double.Parse(args[0]);
			}
			else if (command == "scale_clamp") {
				particle.ScaleClamp = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}

			// Speed.
			else if (command == "pos_jitter") {
				particle.PositionJitter = Double.Parse(args[0]);
			}
			else if (command == "speed_init") {
				particle.Speed = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "speed_increase") {
				particle.SpeedIncrease = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "speed_friction") {
				particle.SpeedFriction = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}

			// Direction.
			else if (command == "dir_init") {
				particle.InitialDirectionOffset = new RangeF(ConvertToDegrees(Double.Parse(args[0])), ConvertToDegrees(Double.Parse(args[1])));
			}
			else if (command == "dir_increase") {
				particle.DirectionIncrease = new RangeF(ConvertToDegrees(Double.Parse(args[0])), ConvertToDegrees(Double.Parse(args[1])));
			}
			else if (command == "dir_friction") {
				particle.DirectionFriction = new RangeF(Double.Parse(args[0]), Double.Parse(args[1]));
			}
			else if (command == "gravity") {
				particle.Gravity = new Vector2F(Double.Parse(args[0]), Double.Parse(args[1]));
			}

			// Rotation.
			else if (command == "rot_init") {
				particle.InitialRotation = new RangeF(ConvertToDegrees(Double.Parse(args[0])), ConvertToDegrees(Double.Parse(args[1])));
				particle.RotateFromDirection = Boolean.Parse(args[2]);
			}
			else if (command == "rot_increase") {
				particle.RotationSpeed = new RangeF(ConvertToDegrees(Double.Parse(args[0])), ConvertToDegrees(Double.Parse(args[1])));
			}
			else if (command == "rot_friction") {
				particle.RotationFriction = Double.Parse(args[0]);
			}
			else if (command == "rot_jitter") {
				particle.RotationJitter = ConvertToDegrees(Double.Parse(args[0]));
			}

			// Sprites and Color.
			else if (command == "animation_speed") {
				particle.AnimationSpeed = Double.Parse(args[0]);
			}
			else if (command == "sheet") {
				sheet = Resources.GetSpriteSheet(args[0]);
			}
			else if (command == "sprites") {
				Sprite[] sprites = new Sprite[args.Count];
				for (int i = 0; i < args.Count; ++i)
					//sprites[i] = GameData.SHEET_PARTICLES[args[i]];
					sprites[i] = sheet[args[i]];
				particle.SetSprite(sprites);
			}
			else if (command == "colors") {
				Color[] colors = new Color[args.Count / 4];
				for (int i = 0; i < colors.Length; ++i) {
					int index = i * 4;
					colors[i] = new Color(
						Int32.Parse(args[index + 0]),
						Int32.Parse(args[index + 1]),
						Int32.Parse(args[index + 2]),
						Int32.Parse(args[index + 3]));
				}
				particle.SetColors(colors);
			}
		}

		// EMITTERS.
		else if (emitter != null) {
			if (command == "area") {
				if (args[0] == "point") {
					emitter.Area = new PointArea(
						Double.Parse(args[1]),
						Double.Parse(args[2]));
				}
				else if (args[0] == "circle") {
					emitter.Area = new CircleArea(
						Double.Parse(args[1]),
						Double.Parse(args[2]),
						Double.Parse(args[3]),
						Boolean.Parse(args[4]));
				}
				else if (args[0] == "rect") {
					emitter.Area = new RectArea(
						Double.Parse(args[1]),
						Double.Parse(args[2]),
						Double.Parse(args[3]),
						Double.Parse(args[4]),
						Boolean.Parse(args[5]));
				}
				else if (args[0] == "line") {
					emitter.Area = new LineArea(
						Double.Parse(args[1]),
						Double.Parse(args[2]),
						Double.Parse(args[3]),
						Double.Parse(args[4]));
				}
			}
			else if (command == "origin") {
				emitter.IsOriginBasedDirection = true;
				emitter.Origin = new Vector2F(
					Double.Parse(args[0]),
					Double.Parse(args[1]));
				emitter.IsSpeedScaledByDistance = Boolean.Parse(args[2]);
			}
			else if (command == "speed") {
				emitter.Speed = new RangeF(
					Double.Parse(args[0]),
					Double.Parse(args[1]));
			}
			else if (command == "direction") {
				emitter.Direction = new Vector2F(
					Double.Parse(args[0]),
					Double.Parse(args[1]));
			}
		}

		// EFFECTS.
		else if (effect != null) {
			if (command == "burst") {
				effect.AddBurst(
					Resources.GetParticleEmitter(args[0]),
					Resources.GetParticleType(args[1]),
					Int32.Parse(args[2]),
					Double.Parse(args[3]));
			}
			else if (command == "stream") {
				effect.AddStream(
					Resources.GetParticleEmitter(args[0]),
					Resources.GetParticleType(args[1]),
					Double.Parse(args[2]),
					Double.Parse(args[3]),
					Double.Parse(args[4]));
			}
		}

		else if (command == "particle") {
			particle = new ParticleType(args[0]);
			particle.Comments = section;
			sheet = Resources.GetSpriteSheet(DefaultParticleSheet);
		}
		else if (command == "emitter") {
			emitter = new ParticleEmitter(args[0]);
			emitter.Comments = section;
		}
		else if (command == "effect") {
			effect = new ParticleEffectType(args[0]);
			effect.Comments = section;
		}

		return true;
	}

	#endregion
}
} // end namespace
