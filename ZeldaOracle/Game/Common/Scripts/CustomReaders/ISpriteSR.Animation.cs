using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public partial class ISpriteSR : ScriptReader {

		/// <summary>Adds CompositeSprite commands to the script reader.</summary>
		public void AddAnimationCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("ANIMATION", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new Animation();
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.Animation;
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			AddCommand("CONTINUE ANIMATION", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<Animation>(spriteName);
				Mode |= Modes.Animation;
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			// Legacy ANIM support
			AddCommand("ANIM", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new Animation();
				AddResource<ISprite>(spriteName, sprite);
				Mode |= Modes.Animation;
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			AddCommand("CONTINUE ANIM", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<Animation>(spriteName);
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
			AddCommand("ADD emptyframe", (int) Modes.Animation,
				"int duration",
			delegate (CommandParam parameters) {
				animationBuilder.AddEmptyFrame(parameters.GetInt(0));
			});
			//=====================================================================================
			AddCommand("ADD strip", (int) Modes.Animation,
				"int duration, int stripLength, Point sourceIndex, Point drawOffset = (0, 0), int depth = 0, Point relative = (1, 0)",
				"int duration, int stripLength, (Point sourceIndex, string definition), Point drawOffset = (0, 0), int depth = 0, Point relative = (1, 0)",
			delegate (CommandParam parameters) {
				Point2I sourceIndex;
				string definition = null;
				var subParam = parameters.GetParam(2);
				if (subParam.GetParam(0).Type == CommandParamType.Array) {
					sourceIndex = subParam.GetPoint(0);
					definition = subParam.GetString(1);
				}
				else {
					sourceIndex = parameters.GetPoint(2);
				}
				animationBuilder.AddFrameStrip(
					parameters.GetInt(0),
					source,
					sourceIndex,
					definition,
					parameters.GetInt(1),
					parameters.GetPoint(3),
					Flip.None,
					Rotation.None,
					parameters.GetInt(4),
					parameters.GetPoint(5));
			});
			//=====================================================================================
			AddCommand("ADD frame", (int) Modes.Animation,
				"int duration, Sprite sprite, Point drawOffset = (0, 0), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				ISprite addSprite = GetSpriteFromParams(parameters, 1, out source, out index, out definition);
				if (source != null) {
					animationBuilder.AddFrame(
						parameters.GetInt(0),
						source,
						index,
						definition,
						parameters.GetPoint(2),
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
				else {
					animationBuilder.AddFrame(
						parameters.GetInt(0),
						addSprite,
						parameters.GetPoint(2),
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
			});
			//=====================================================================================
			AddCommand("ADD part", (int) Modes.Animation,
				"int duration, Sprite sprite, Point drawOffset = (0, 0), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				ISprite addSprite = GetSpriteFromParams(parameters, 1, out source, out index, out definition);
				if (source != null) {
					animationBuilder.AddPart(
						parameters.GetInt(0),
						source,
						index,
						definition,
						parameters.GetPoint(2),
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
				else {
					animationBuilder.AddPart(
						parameters.GetInt(0),
						addSprite,
						parameters.GetPoint(2),
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
			});
			//=====================================================================================
			/*AddCommand("Add", (int) Modes.Animation, new string[] {
				"string emptyFrame, int duration",
				"string strip, int duration, int stripLength, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0, (int nextIndexX, int nextIndexY) = (1, 0)",
				"string frameOrPart, int duration, string spriteName, (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				// Int needs to go before string as int/float defaults to string.
				"string frameOrPart, int duration, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frameOrPart, int duration, (string animationName, int substrip), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frameOrPart, int duration, (string spriteName, string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frameOrPart, int duration, ((int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frameOrPart, int duration, (string sourceName, (int indexX, int indexY)), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frameOrPart, int duration, (string sourceName, (int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
			}, delegate (CommandParam parameters) {
				if (parameters.GetParam(0).Name == "strip") {
					if (string.Compare(parameters.GetString(0), "strip", true) == 0) {
						Point2I index;
						string definition = null;
						var subParam = parameters.GetParam(3);
						if (subParam.GetParam(0).Type == CommandParamType.Array) {
							index = subParam.GetPoint(0);
							definition = subParam.GetString(1);
						}
						else {
							index = parameters.GetPoint(3);
						}
						animationBuilder.AddFrameStrip(
							parameters.GetInt(1),
							source,
							index,
							definition,
							parameters.GetInt(2),
							parameters.GetPoint(4),
							Flip.None,
							Rotation.None,
							parameters.GetInt(5),
							parameters.GetPoint(6, new Point2I(1, 0)));
					}
					else {
						ThrowCommandParseError("Must specify 'strip' as the first argument for this overload!");
					}
				}
				else if (parameters.GetParam(0).Name == "frameOrPart") {
					ISpriteSource source;
					Point2I index;
					string definition;
					if (string.Compare(parameters.GetString(0), "frame", true) == 0) {
						ISprite addSprite = GetSpriteFromParams(parameters, 2, out source, out index, out definition);
						if (source != null) {
							animationBuilder.AddFrame(
								parameters.GetInt(1),
								source,
								index,
								definition,
								parameters.GetPoint(3),
								Flip.None,
								Rotation.None,
								parameters.GetInt(4));
						}
						else {
							animationBuilder.AddFrame(
								parameters.GetInt(1),
								addSprite,
								parameters.GetPoint(3),
								Flip.None,
								Rotation.None,
								parameters.GetInt(4));
						}
					}
					else if (parameters.GetString(0) == "part") {
						ISprite addSprite = GetSpriteFromParams(parameters, 2, out source, out index, out definition);
						if (source != null) {
							animationBuilder.AddPart(
								parameters.GetInt(1),
								source,
								index,
								definition,
								parameters.GetPoint(3),
								Flip.None,
								Rotation.None,
								parameters.GetInt(4));
						}
						else {
							animationBuilder.AddPart(
								parameters.GetInt(1),
								addSprite,
								parameters.GetPoint(3),
								Flip.None,
								Rotation.None,
								parameters.GetInt(4));
						}
					}
					else {
						ThrowCommandParseError("Must specify 'frame' or 'part' as the first argument for this overload!");
					}
				}
				else if (parameters.GetParam(0).Name == "emptyFrame") {
					if (string.Compare(parameters.GetString(0), "emptyframe", true) == 0) {
						animationBuilder.AddEmptyFrame(parameters.GetInt(1));
					}
					else {
						ThrowCommandParseError("Must specify 'emptyframe' as the first argument for this overload!");
					}
				}
				else {
					ThrowParseError("Unknown add type '" + parameters.GetString(0) + "' for animation");
				}
			});*/
			//=====================================================================================
			AddCommand("Insert", (int) Modes.Animation, new string[] {
				"string strip, int time, int duration, int stripLength, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0, (int nextIndexX, int nextIndexY) = (1, 0)",
				"string strip, int time, int duration, int stripLength, ((int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0, (int nextIndexX, int nextIndexY) = (1, 0)",
				"string frame, int time, int duration, string spriteName, (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				// Int needs to go before string as int/float defaults to string.
				"string frame, int time, int duration, (int indexX, int indexY), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frame, int time, int duration, (string animationName, int substrip), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frame, int time, int duration, (string spriteName, string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frame, int time, int duration, ((int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frame, int time, int duration, (string sourceName, (int indexX, int indexY)), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
				"string frame, int time, int duration, (string sourceName, (int indexX, int indexY), string definition), (int drawOffsetX, int drawOffsetY) = (0, 0), int depth = 0",
			}, delegate (CommandParam parameters) {
				if (parameters.GetParam(0).Name == "strip") {
					if (string.Compare(parameters.GetString(0), "strip", true) == 0) {
						Point2I index;
						string definition = null;
						var subParam = parameters.GetParam(4);
						if (subParam.GetParam(0).Type == CommandParamType.Array) {
							index = subParam.GetPoint(0);
							definition = subParam.GetString(1);
						}
						else {
							index = parameters.GetPoint(4);
						}
						animationBuilder.InsertFrameStrip(
							parameters.GetInt(1),
							parameters.GetInt(2),
							source,
							index,
							definition,
							parameters.GetInt(3),
							parameters.GetPoint(5),
							Flip.None,
							Rotation.None,
							parameters.GetInt(6),
							parameters.GetPoint(7, new Point2I(1, 0)));
					}
					else {
						ThrowCommandParseError("Must specify 'strip' as the first argument for this overload!");
					}
				}
				else if (parameters.GetParam(0).Name == "frame") {
					ISpriteSource source;
					Point2I index;
					string definition;
					if (string.Compare(parameters.GetString(0), "frame", true) == 0) {
						ISprite addSprite = GetSpriteFromParams(parameters, 3, out source, out index, out definition);
						if (source != null) {
							animationBuilder.InsertFrame(
								parameters.GetInt(1),
								parameters.GetInt(2),
								source,
								index,
								definition,
								parameters.GetPoint(4),
								Flip.None,
								Rotation.None,
								parameters.GetInt(5));
						}
						else {
							animationBuilder.InsertFrame(
								parameters.GetInt(1),
								parameters.GetInt(2),
								addSprite,
								parameters.GetPoint(4),
								Flip.None,
								Rotation.None,
								parameters.GetInt(5));
						}
					}
					else {
						ThrowCommandParseError("Must specify 'frame' as the first argument for this overload!");
					}
				}
				else {
					ThrowParseError("Unknown insert type '" + parameters.GetString(0) + "' for animation");
				}
			});
			//=====================================================================================
			// TODO: Implement Animation Combine
			AddCommand("Combine", (int) Modes.Animation,
				"string animationName, (int offsetX, int offsetY) = (0, 0), int depthOffset = 0",
				"(string animationName, int substrip), (int offsetX, int offsetY) = (0, 0), int depthOffset = 0",
				"int timeOffset, string animationName, (int offsetX, int offsetY) = (0, 0), int depthOffset = 0",
				"int timeOffset, (string animationName, int substrip), (int offsetX, int offsetY) = (0, 0), int depthOffset = 0",
			delegate (CommandParam parameters) {
				int paramOffset = 0;
				int timeOffset = 0;
				if (parameters.GetParam(0).Type == CommandParamType.Integer) {
					paramOffset = 1;
					timeOffset = parameters.GetInt(0);
				}
				string animationName;
				int substrip = 0;
				if (parameters.GetParam(paramOffset).Type == CommandParamType.String) {
					animationName = parameters.GetString(paramOffset);
				}
				else {
					var subParam = parameters.GetParam(paramOffset);
					animationName = subParam.GetString(0);
					substrip = subParam.GetInt(1);
				}
				Animation combineAnim = GetSprite<Animation>(animationName);
				combineAnim = combineAnim.GetSubstrip(substrip);
				Point2I drawOffset = parameters.GetPoint(paramOffset + 1);
				int depthOffset = parameters.GetInt(paramOffset + 2);
				foreach (AnimationFrame frame in combineAnim.GetFrames()) {
					AnimationFrame newFrame = new AnimationFrame(frame);
					newFrame.Depth		+= depthOffset;
					newFrame.DrawOffset	+= drawOffset;
					newFrame.StartTime	+= timeOffset;
					Animation.AddFrame(newFrame);
				}
			});
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
				"int numRepeats",
				"int numFrames, int numRepeats",
			delegate (CommandParam parameters) {
				if (parameters.ChildCount == 1)
					animationBuilder.RepeatPreviousFrames(animationBuilder.Animation.FrameCount, parameters.GetInt(0));
				else
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
			// Changes the definition for all definition sprites
			AddCommand("ChangeDefinition", (int) Modes.Animation,
				"string definition",
			delegate (CommandParam parameters) {
				animationBuilder.ShiftSourcePositions(
					parameters.GetPoint(0));
			});
			//=====================================================================================
		}
	}
}
