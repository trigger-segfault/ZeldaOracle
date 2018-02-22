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
	public partial class SpriteSR : ScriptReader {

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
			/*AddCommand("ANIM", (int) Modes.Root,
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
			});*/
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("SUBSTRIP", (int) Modes.Animation,
				"string loopMode",
			delegate (CommandParam parameters) {
				LoopMode loopMode = LoopMode.Repeat;
				if (!Enum.TryParse(parameters.GetString(0), true, out loopMode))
					ThrowParseError("Unknown loop mode '" + parameters.GetString(0) + "' for animation", parameters[0]);
				
				animationBuilder.CreateSubStrip();
				animationBuilder.SetLoopMode(loopMode);
				if (Animation == null)
					sprite = animationBuilder.Animation;
			});
			//=====================================================================================
			AddCommand("CLONE", (int) Modes.Animation,
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
				"int duration, int stripLength, Point sourceIndex, Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depth = 0, Point relative = (1, 0)",
				"int duration, int stripLength, (Point sourceIndex, string definition), Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depth = 0, Point relative = (1, 0)",
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
				Rectangle2I? clipping = parameters.GetRectangle(4);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				animationBuilder.AddFrameStrip(
					parameters.GetInt(0),
					source,
					sourceIndex,
					definition,
					parameters.GetInt(1),
					parameters.GetPoint(3),
					clipping,
					Flip.None,
					Rotation.None,
					parameters.GetInt(5),
					parameters.GetPoint(6));
			});
			//=====================================================================================
			AddCommand("ADD frame", (int) Modes.Animation,
				"int duration, Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				Rectangle2I? clipping = parameters.GetRectangle(3);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				ISprite addSprite = GetSpriteFromParams(parameters, 1, out source, out index, out definition);
				if (source != null) {
					animationBuilder.AddFrame(
						parameters.GetInt(0),
						source,
						index,
						definition,
						parameters.GetPoint(2),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(4));
				}
				else {
					animationBuilder.AddFrame(
						parameters.GetInt(0),
						addSprite,
						parameters.GetPoint(2),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(4));
				}
			});
			//=====================================================================================
			AddCommand("ADD part", (int) Modes.Animation,
				"int duration, Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				Rectangle2I? clipping = parameters.GetRectangle(3);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				ISprite addSprite = GetSpriteFromParams(parameters, 1, out source, out index, out definition);
				if (source != null) {
					animationBuilder.AddPart(
						parameters.GetInt(0),
						source,
						index,
						definition,
						parameters.GetPoint(2),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(4));
				}
				else {
					animationBuilder.AddPart(
						parameters.GetInt(0),
						addSprite,
						parameters.GetPoint(2),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(4));
				}
			});
			//=====================================================================================
			AddCommand("ADD static", (int) Modes.Animation,
				"Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				Rectangle2I? clipping = parameters.GetRectangle(2);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				ISprite addSprite = GetSpriteFromParams(parameters, 0, out source, out index, out definition);
				if (source != null) {
					animationBuilder.AddStatic(
						source,
						index,
						definition,
						parameters.GetPoint(1),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
				else {
					animationBuilder.AddStatic(
						addSprite,
						parameters.GetPoint(1),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(3));
				}
			});
			//=====================================================================================
			AddCommand("INSERT strip", (int) Modes.Animation,
				"int time, int duration, int stripLength, Point sourceIndex, Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depth = 0, Point relative = (1, 0)",
				"int time, int duration, int stripLength, (Point sourceIndex, string definition), " +
					"Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), int depth = 0, Point relative = (1, 0)",
			delegate (CommandParam parameters) {
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
				Rectangle2I? clipping = parameters.GetRectangle(5);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				animationBuilder.InsertFrameStrip(
					parameters.GetInt(0),
					parameters.GetInt(1),
					source,
					index,
					definition,
					parameters.GetInt(2),
					parameters.GetPoint(4),
					clipping,
					Flip.None,
					Rotation.None,
					parameters.GetInt(6),
					parameters.GetPoint(7));
			});
			//=====================================================================================
			AddCommand("INSERT frame", (int) Modes.Animation,
				"int time, int duration, Sprite sprite, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), int depth = 0",
			delegate (CommandParam parameters) {
				ISpriteSource source;
				Point2I index;
				string definition;
				Rectangle2I? clipping = parameters.GetRectangle(4);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				ISprite addSprite = GetSpriteFromParams(parameters, 2, out source, out index, out definition);
				if (source != null) {
					animationBuilder.InsertFrame(
						parameters.GetInt(0),
						parameters.GetInt(1),
						source,
						index,
						definition,
						parameters.GetPoint(3),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(5));
				}
				else {
					animationBuilder.InsertFrame(
						parameters.GetInt(0),
						parameters.GetInt(1),
						addSprite,
						parameters.GetPoint(3),
						clipping,
						Flip.None,
						Rotation.None,
						parameters.GetInt(5));
				}
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.Animation,
				"string animationName, Point drawOffset = (0, 0), Rectangle clipping = (0, 0, -1, -1), " +
					"int depthOffset = 0",
				"(string animationName, int substrip), Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depthOffset = 0",
				"int timeOffset, string animationName, Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depthOffset = 0",
				"int timeOffset, (string animationName, int substrip), Point drawOffset = (0, 0), " +
					"Rectangle clipping = (0, 0, -1, -1), int depthOffset = 0",
			delegate (CommandParam parameters) {
				int paramOffset = 0;
				int timeOffset = 0;
				if (parameters.GetParam(0).IsValidType(CommandParamType.Integer)) {
					paramOffset = 1;
					timeOffset = parameters.GetInt(0);
				}
				string animationName;
				int substrip = 0;
				if (parameters.GetParam(paramOffset).IsValidType(CommandParamType.String)) {
					animationName = parameters.GetString(paramOffset);
				}
				else {
					var subParam = parameters.GetParam(paramOffset);
					animationName = subParam.GetString(0);
					substrip = subParam.GetInt(1);
				}
				Point2I drawOffset = parameters.GetPoint(paramOffset + 1);
				Rectangle2I? clipping = parameters.GetRectangle(paramOffset + 2);
				if (clipping.Value.Size == -Point2I.One)
					clipping = null;
				int depthOffset = parameters.GetInt(paramOffset + 3);
				Animation combineAnim = GetSprite<Animation>(animationName);
				animationBuilder.Combine(combineAnim, substrip, timeOffset, drawOffset, clipping, depthOffset);
				combineAnim = combineAnim.GetSubstrip(substrip);
			});
			//=====================================================================================
			// MODIFICATIONS
			//=====================================================================================
			AddCommand("MAKEQUAD", (int) Modes.Animation,
				"",
			delegate (CommandParam parameters) {
				animationBuilder.MakeQuad();
			});
			//=====================================================================================
			AddCommand("MAKEDYNAMIC", (int) Modes.Animation,
				"int numSubstrips, Point relative",
			delegate (CommandParam parameters) {
				animationBuilder.MakeDynamic(
					parameters.GetInt(0),
					parameters.GetPoint(1));
			});
			//=====================================================================================
			AddCommand("OFFSET", (int) Modes.Animation,
				"Point offset",
			delegate (CommandParam parameters) {
				animationBuilder.Offset(
					parameters.GetPoint(0));
			});
			//=====================================================================================
			AddCommand("FLICKER", (int) Modes.Animation,
				"int alternateDelay",
			delegate (CommandParam parameters) {
				// FLICKER <alternateDelay>
				animationBuilder.MakeFlicker(parameters.GetInt(0));
			});
			//=====================================================================================
			AddCommand("REPEAT", (int) Modes.Animation,
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
			AddCommand("REWIND", (int) Modes.Animation,
				"",
			delegate (CommandParam parameters) {
				animationBuilder.Animation = Animation;
			});
			//=====================================================================================
			// Shifts the source positions of all sprites in the animation.
			AddCommand("SHIFTSOURCE", (int) Modes.Animation,
				"Point relative",
			delegate (CommandParam parameters) {
				animationBuilder.ShiftSourcePositions(
					parameters.GetPoint(0));
			});
			//=====================================================================================
			// Changes the definition for all style sprites
			AddCommand("CHANGESTYLE", (int) Modes.Animation,
				"const all, string newStyle",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(1);
				animationBuilder.ChangeStyle(null, style, true);
			});
			//=====================================================================================
			// Changes the definition for all style sprites
			AddCommand("CHANGESTYLE", (int) Modes.Animation,
				"string oldStyle, string newStyle",
			delegate (CommandParam parameters) {
				string oldStyle = parameters.GetString(0);
				string style = parameters.GetString(1);
				animationBuilder.ChangeStyle(oldStyle, style, false);
			});
			//=====================================================================================
			// Changes the definition for all color sprites
			AddCommand("CHANGECOLOR", (int) Modes.Animation,
				"const all, string newColor",
			delegate (CommandParam parameters) {
				string color = parameters.GetString(1);
				animationBuilder.ChangeColor(null, color, true);
			});
			//=====================================================================================
			// Changes the definition for all color sprites
			AddCommand("CHANGECOLOR", (int) Modes.Animation,
				"string oldColor, string newColor",
			delegate (CommandParam parameters) {
				string oldColor = parameters.GetString(0);
				string color = parameters.GetString(1);
				animationBuilder.ChangeColor(oldColor, color, false);
			});
			//=====================================================================================
			AddCommand("CLIP", (int) Modes.Animation,
				"Rectangle clipping",
			delegate (CommandParam parameters) {
				animationBuilder.Clip(parameters.GetRectangle(0));
			});
			//=====================================================================================
		}
	}
}
