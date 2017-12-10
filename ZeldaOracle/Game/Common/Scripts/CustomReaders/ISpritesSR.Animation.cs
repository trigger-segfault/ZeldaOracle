using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public partial class ISpritesSR : ScriptReader {

		/// <summary>Adds CompositeSprite commands to the script reader.</summary>
		public void AddAnimationCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("ANIMATION", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				if (parameters.HasPrefix("continue")) {
					ContinueSprite<Animation>(spriteName);
				}
				else {
					sprite = new Animation();
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.Animation;
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			// Legacy ANIM support
			AddCommand("ANIM", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				if (parameters.HasPrefix("continue")) {
					ContinueSprite<Animation>(spriteName);
				}
				else {
					sprite = new Animation();
					AddResource<ISprite>(spriteName, sprite);
				}
				Mode |= Modes.Animation;
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("SubStrip", (int) Modes.Animation,
				"string loopMode",
			delegate (CommandParam parameters) {
				LoopMode loopMode = LoopMode.Repeat;
				if (parameters.GetString(0) == "reset")
					loopMode = LoopMode.Reset;
				else if (parameters.GetString(0) == "repeat" || parameters.GetString(0) == "loop")
					loopMode = LoopMode.Repeat;
				else if (parameters.GetString(0) == "clamp")
					loopMode = LoopMode.Clamp;
				else
					ThrowParseError("Unknown loop mode '" + parameters.GetString(0) + "' for animation", parameters[0]);

				animationBuilder.CreateSubStrip();
				animationBuilder.SetLoopMode(loopMode);
				if (Animation == null)
					sprite = animationBuilder.Animation;
			});
			//=====================================================================================
			AddCommand("Clone", (int) Modes.Animation,
				"string animationName",
			delegate (CommandParam parameters) {
				animationBuilder.CreateClone(GetSprite<Animation>(parameters.GetString(0)));
				sprite = animationBuilder.Animation;
				SetResource<ISprite>(spriteName, sprite);
			});
			//=====================================================================================
			// FRAME BUILDING.
			//=====================================================================================
			AddCommand("Add", (int) Modes.Animation,
				"string emptyFrame, int duration",
				"string strip, int duration, int stripLength, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0), (int nextSourceX, int nextSourceY) = (1, 0)",
				"string frame, int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
				"string part,  int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
				"string frame, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
				"string part,  int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.AddFrameStrip(
						parameters.GetInt(1),
						parameters.GetPoint(3),
						parameters.GetInt(2),
						parameters.GetPoint(4, Point2I.Zero),
						parameters.GetPoint(5, new Point2I(1, 0)));
				}
				else if (parameters.GetString(0) == "frame") {
					ISprite spr;
					Point2I offset = parameters.GetPoint(3, Point2I.Zero);
					if (parameters[2].Type == CommandParamType.Array) {
						//spr = animationBuilder.SpriteSheet.GetSprite();
						animationBuilder.AddFrame(parameters.GetInt(1), parameters.GetPoint(2), offset);
					}
					else {
						spr = Resources.GetResource<ISprite>(parameters.GetString(2));
						animationBuilder.AddFrame(parameters.GetInt(1), spr, offset);
					}
				}
				else if (parameters.GetString(0) == "part") {
					ISprite spr;
					Point2I offset = parameters.GetPoint(3, Point2I.Zero);
					if (parameters[2].Type == CommandParamType.Array) {
						//spr = animationBuilder.SpriteSheet.GetSprite(parameters.GetPoint(2));
						animationBuilder.AddPart(parameters.GetInt(1), parameters.GetPoint(2), offset);
					}
					else {
						spr = Resources.GetResource<ISprite>(parameters.GetString(2));
						animationBuilder.AddPart(parameters.GetInt(1), spr, offset);
					}
				}
				else if (parameters.GetString(0) == "emptyframe") {
					animationBuilder.AddEmptyFrame(parameters.GetInt(1));
				}
				else
					ThrowParseError("Unknown add type '" + parameters.GetString(0) + "' for animation");
			});
			//=====================================================================================
			AddCommand("Insert", (int) Modes.Animation,
				"string strip, int time, int duration, int stripLength, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0), (int nextSourceX, int nextSourceY) = (1, 0)",
				"string frame, int time, int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
				"string frame, int time, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.InsertFrameStrip(
						parameters.GetInt(1),
						parameters.GetInt(2),
						parameters.GetPoint(4),
						parameters.GetInt(3),
						parameters.GetPoint(5, Point2I.Zero),
						parameters.GetPoint(6, new Point2I(1, 0)));
				}
				else if (parameters.GetString(0) == "frame") {
					if (parameters[3].Type == CommandParamType.Array) {
						animationBuilder.InsertFrame(
							parameters.GetInt(1),
							parameters.GetInt(2),
							parameters.GetPoint(3),
							parameters.GetPoint(4, Point2I.Zero));
					}
					else {
						animationBuilder.InsertFrame(
							parameters.GetInt(1),
							parameters.GetInt(2),
							GetResource<ISprite>(parameters.GetString(3)),
							parameters.GetPoint(4, Point2I.Zero));
					}
				}
				else
					ThrowParseError("Unknown insert type '" + parameters.GetString(0) + "' for animation");
			});
			//=====================================================================================
			// TODO: Implement Animation Combine
			/*AddCommand("Combine", (int) Modes.Animation,
				"string animation, string animationName, (int offsetX, int offsetY) = (0, 0)",
				"string sprite, int time, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
				"string sprite, int time, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
				"string frame, int time, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
			delegate (CommandParam parameters) {
				if (parameters.GetString(0) == "animation") {
					Animation anim = GetSprite<Animation>(parameters.GetString(1));
					foreach (AnimationFrame frame in anim.GetFrames()) {
						animationBuilder.Animation.AddFrame(new AnimationFrame(frame));
					}
				}
				else if (parameters.GetString(0) == "frame") {
					if (parameters[3].Type == CommandParamType.Array) {
						animationBuilder.InsertFrame(
							parameters.GetInt(1),
							parameters.GetInt(2),
							parameters.GetPoint(3),
							parameters.GetPoint(4, Point2I.Zero));
					}
					else {
						animationBuilder.InsertFrame(
							parameters.GetInt(1),
							parameters.GetInt(2),
							GetResource<ISprite>(parameters.GetString(3)),
							parameters.GetPoint(4, Point2I.Zero));
					}
				}
				else
					ThrowParseError("Unknown insert type '" + parameters.GetString(0) + "' for animation");
			});*/
			//=====================================================================================
			// MODIFICATIONS
			//=====================================================================================
			AddCommand("MakeQuad", (int) Modes.Animation,
				"",
			delegate (CommandParam parameters) {
				animationBuilder.MakeQuad();
			});
			//=====================================================================================
			AddCommand("MakeDynamic", (int) Modes.Animation,
				"int numSubstrips, (int nextSourceX, int nextSourceY)",
			delegate (CommandParam parameters) {
				animationBuilder.MakeDynamic(
					parameters.GetInt(0),
					parameters.GetPoint(1));
			});
			//=====================================================================================
			AddCommand("Offset", (int) Modes.Animation,
				"(int offsetX, int offsetY)",
			delegate (CommandParam parameters) {
				animationBuilder.Offset(
					parameters.GetPoint(0));
			});
			//=====================================================================================
			AddCommand("Flicker", (int) Modes.Animation,
				"int alternateDelay, string startOnOrOff",
			delegate (CommandParam parameters) {
				// FLICKER <alternateDelay> <on/off>

				bool startOn = true;
				if (parameters.GetString(1) == "on")
					startOn = true;
				else if (parameters.GetString(1) == "off")
					startOn = false;
				else
					ThrowParseError("Must be either on or off for flicker start state");

				animationBuilder.MakeFlicker(parameters.GetInt(0), startOn);
			});
			//=====================================================================================
			AddCommand("Repeat", (int) Modes.Animation,
				"int numFrames, int numRepeats",
			delegate (CommandParam parameters) {
				animationBuilder.RepeatPreviousFrames(parameters.GetInt(0), parameters.GetInt(1));
			});
			//=====================================================================================
			// Rewinds back to the first animation (if currently on a latter substrip).
			AddCommand("Rewind", (int) Modes.Animation,
				"",
			delegate (CommandParam parameters) {
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			// Shifts the source positions of all sprites in the animation.
			AddCommand("ShiftSource", (int) Modes.Animation,
				"(int shiftX, int shiftY)",
			delegate (CommandParam parameters) {
				animationBuilder.ShiftSourcePositions(
					parameters.GetPoint(0));
			});
			//=====================================================================================
		}
	}
}
