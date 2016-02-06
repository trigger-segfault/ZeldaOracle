﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	/** <summary>
	 * Script reader for sprite sheets. The script can contain
	 * information for multiple sprite sheets with corresponding
	 * images in the content folder.
	 *
	 * FORMAT:
	 *
	 * @font [name]
	 * @grid [char_width] [char_height] [char_spacing_x] [char_spacing_y] [offset_x] [offset_y]
	 * @end
	 *
	 * Note: All declared sprites must be within the
	 * @spritesheet and @end commands.
	 * </summary>
	 */
	public class AnimationSR : ScriptReader {

		private AnimationBuilder animationBuilder;
		private Animation animation;
		private string animationName;
		private TemporaryResources resources;
		private bool useTemporary;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public AnimationSR(TemporaryResources resources = null) {

			this.resources			= resources;
			this.useTemporary		= resources != null;
			this.animationBuilder	= new AnimationBuilder();

			
			//=====================================================================================
			// SPRITE SHEET.
			//=====================================================================================
			AddCommand("SpriteSheet", "string path",
			delegate(CommandParam parameters) {
				SpriteSheet sheet;
				if (useTemporary && resources != null)
					sheet = resources.GetResource<SpriteSheet>(parameters.GetString(0));
				else
					sheet = Resources.GetResource<SpriteSheet>(parameters.GetString(0));
				animationBuilder.SpriteSheet = sheet;
			});
			//=====================================================================================
			// BEGIN/END.
			//=====================================================================================
			AddCommand("Anim", "string name",
			delegate(CommandParam parameters) {
				animationName = parameters.GetString(0);
				animationBuilder.BeginNull();
				animation = null;
			});
			//=====================================================================================
			AddCommand("End", "",
			delegate(CommandParam parameters) {
				if (animation != null) {
					animationBuilder.End();
					if (useTemporary && resources != null)
						resources.AddResource<Animation>(animationName, animation);
					else
						Resources.AddResource<Animation>(animationName, animation);
				}
			});
			//=====================================================================================
			AddCommand("SubStrip", "string loopMode",
			delegate(CommandParam parameters) {
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
				if (animation == null)
					animation = animationBuilder.Animation;
			});
			//=====================================================================================
			AddCommand("Clone", "string animationName",
			delegate(CommandParam parameters) {
				if (useTemporary && resources != null && resources.ExistsResource<Animation>(parameters.GetString(0))) {
					animationBuilder.CreateClone(resources.GetResource<Animation>(parameters.GetString(0)));
					animation = animationBuilder.Animation;
				}
				if (Resources.ExistsResource<Animation>(parameters.GetString(0))) {
					animationBuilder.CreateClone(Resources.GetResource<Animation>(parameters.GetString(0)));
					animation = animationBuilder.Animation;
				}
				else {
					// ERROR: Can't clone nonexistant animation.
					ThrowParseError("The animation '" + parameters.GetString(0) + "' does not exist", parameters[0]);
				}
			});
			//=====================================================================================
			// FRAME BUILDING.
			//=====================================================================================
			AddCommand("Add",
				"string emptyFrame, int duration",
				"string strip, int duration, int stripLength, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0), (int nextSourceX, int nextSourceY) = (1, 0)",
				"string frame, int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
				"string part,  int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
				"string frame, int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
				"string part,  int duration, string spriteName, (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.AddFrameStrip(
						parameters.GetInt(1),
						parameters.GetPoint(3).X,
						parameters.GetPoint(3).Y,
						parameters.GetInt(2),
						parameters.GetPoint(4, Point2I.Zero).X,
						parameters.GetPoint(4, Point2I.Zero).Y,
						parameters.GetPoint(5, new Point2I(1, 0)).X,
						parameters.GetPoint(5, new Point2I(1, 0)).Y);
				}
				else if (parameters.GetString(0) == "frame") {
					Sprite spr;
					Point2I offset = parameters.GetPoint(3, Point2I.Zero);
					if (parameters[2].Type == CommandParamType.Array) {
						spr = new Sprite(animationBuilder.SpriteSheet, parameters.GetPoint(2), offset);
					}
					else {
						spr = new Sprite(Resources.GetResource<Sprite>(parameters.GetString(2)));
						for (Sprite part = spr; part != null; part = part.NextPart)
							part.DrawOffset += offset;
					}
					animationBuilder.AddFrame(parameters.GetInt(1), spr);
				}
				else if (parameters.GetString(0) == "part") {
					Sprite spr;
					Point2I offset = parameters.GetPoint(3, Point2I.Zero);
					if (parameters[2].Type == CommandParamType.Array) {
						spr = new Sprite(animationBuilder.SpriteSheet, parameters.GetPoint(2), offset);
					}
					else {
						spr = new Sprite(Resources.GetResource<Sprite>(parameters.GetString(2)));
						spr.DrawOffset = offset;
					}
					animationBuilder.AddPart(parameters.GetInt(1), spr);
				}
				else if (parameters.GetString(0) == "emptyframe") {
					animationBuilder.AddEmptyFrame(parameters.GetInt(1));
				}
				else
					ThrowParseError("Unknown add type '" + parameters.GetString(0) + "' for animation");
			});
			//=====================================================================================
			AddCommand("Insert",
				"string strip, int time, int duration, int stripLength, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0), (int nextSourceX, int nextSourceY) = (1, 0)",
				"string frame, int time, int duration, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.InsertFrameStrip(
						parameters.GetInt(1),
						parameters.GetInt(2),
						parameters.GetPoint(4).X,
						parameters.GetPoint(4).Y,
						parameters.GetInt(3),
						parameters.GetPoint(5, Point2I.Zero).X,
						parameters.GetPoint(5, Point2I.Zero).Y,
						parameters.GetPoint(6, new Point2I(1, 0)).X,
						parameters.GetPoint(6, new Point2I(1, 0)).Y);
				}
				else if (parameters.GetString(0) == "frame") {
					animationBuilder.InsertFrame(
						parameters.GetInt(1),
						parameters.GetInt(2),
						parameters.GetPoint(3).X,
						parameters.GetPoint(3).Y,
						parameters.GetPoint(4, Point2I.Zero).X,
						parameters.GetPoint(4, Point2I.Zero).Y);
				}
				else
					ThrowParseError("Unknown insert type '" + parameters.GetString(0) + "' for animation");
			});
			//=====================================================================================
			// MODIFICATIONS
			//=====================================================================================
			AddCommand("MakeQuad", "",
			delegate(CommandParam parameters) {
				animationBuilder.MakeQuad();
			});
			//=====================================================================================
			AddCommand("MakeDynamic", "int numSubstrips, (int nextSourceX, int nextSourceY)",
			delegate(CommandParam parameters) {
				animationBuilder.MakeDynamic(
					parameters.GetInt(0), 
					parameters.GetPoint(1).X,
					parameters.GetPoint(1).Y);
			});
			//=====================================================================================
			AddCommand("Offset", "(int offsetX, int offsetY)",
			delegate(CommandParam parameters) {
				animationBuilder.Offset(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});
			//=====================================================================================
			AddCommand("Flicker", "int alternateDelay, string startOnOrOff",
			delegate(CommandParam parameters) {
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
			AddCommand("Repeat", "int numFrames, int numRepeats",
			delegate(CommandParam parameters) {
				animationBuilder.RepeatPreviousFrames(parameters.GetInt(0), parameters.GetInt(1));
			});
			//=====================================================================================
			// Rewinds back to the first animation (if currently on a latter substrip).
			AddCommand("Rewind", "",
			delegate(CommandParam parameters) {
				animationBuilder.Animation = animation;
			});
			//=====================================================================================
			// Shifts the source positions of all sprites in the animation.
			AddCommand("ShiftSource", "(int shiftX, int shiftY)",
			delegate(CommandParam parameters) {
				animationBuilder.ShiftSourcePositions(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});
			//=====================================================================================
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected override void BeginReading() {
			animation = null;
			animationName = "";
			animationBuilder.SpriteSheet = null;
		}

		// Ends reading the script.
		protected override void EndReading() {
			animation = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool UseTemporaryResources {
			get { return useTemporary; }
			set { useTemporary = value; }
		}
	}
} // end namespace
