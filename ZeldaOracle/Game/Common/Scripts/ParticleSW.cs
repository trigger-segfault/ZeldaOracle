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
using System.IO;


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
public class ParticleSW {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The name of the default particles sprite sheet. </summary> */
	public const string DefaultParticleSheet = "sheet_particles";

	#endregion
	//=========== MEMBERS ============
	#region Members

	private ParticleType[] particles;
	private ParticleEmitter[] emitters;
	private ParticleEffectType[] effects;
	private string section;

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	
	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected void BeginWriting() {
		particles	= Resources.ParticleTypes;
		emitters	= Resources.ParticleEmitters;
		effects		= Resources.ParticleEffects;
	}
	/** <summary> Ends reading the script. </summary> */
	protected void EndWriting() {

	}

	public void WriteScript(StreamWriter writer) {
		BeginWriting();
		Loop(writer);
		EndWriting();
	}

	protected void Loop(StreamWriter writer) {

		writer.WriteLine();

		for (int i = 0; i < particles.Length; i++) {
			bool containsSection = false;
			for (int j = 0; j < ParticleSR.sectionOrder.Count; j++) {
				if ((particles[i].Comments ?? null) == ParticleSR.sectionOrder[j]) {
					containsSection = true;
					break;
				}
			}
			if (!containsSection)
				ParticleSR.sectionOrder.Add((particles[i].Comments ?? null));
		}
		for (int i = 0; i < emitters.Length; i++) {
			bool containsSection = false;
			for (int j = 0; j < ParticleSR.sectionOrder.Count; j++) {
				if ((emitters[i].Comments ?? "") == ParticleSR.sectionOrder[j]) {
					containsSection = true;
					break;
				}
			}
			if (!containsSection)
				ParticleSR.sectionOrder.Add((emitters[i].Comments ?? ""));
		}
		for (int i = 0; i < effects.Length; i++) {
			bool containsSection = false;
			for (int j = 0; j < ParticleSR.sectionOrder.Count; j++) {
				if ((effects[i].Comments ?? "")== ParticleSR.sectionOrder[j]) {
					containsSection = true;
					break;
				}
			}
			if (!containsSection)
				ParticleSR.sectionOrder.Add((effects[i].Comments ?? ""));
		}

		for (int j = 0; j < ParticleSR.sectionOrder.Count; j++) {
			section = ParticleSR.sectionOrder[j];
			bool containsSection = false;

			for (int i = 0; i < particles.Length && !containsSection; i++) {
				if (particles[i].Comments == section) {
					containsSection = true;
					break;
				}
			}
			for (int i = 0; i < emitters.Length && !containsSection; i++) {
				if (emitters[i].Comments == section) {
					containsSection = true;
					break;
				}
			}
			for (int i = 0; i < effects.Length && !containsSection; i++) {
				if (effects[i].Comments == section) {
					containsSection = true;
					break;
				}
			}

			if (!containsSection) {
				ParticleSR.sectionOrder.Remove(section);
				continue;
			}

			writer.WriteLine(new string('#', section.Length + 6));
			writer.WriteLine("#  " + section + "  #");
			writer.WriteLine(new string('#', section.Length + 6));
			writer.WriteLine("[region]");
			writer.WriteLine();

			for (int i = 0; i < particles.Length; i++) {
				if (particles[i].Comments == section) {
					WriteParticle(writer, particles[i]);
				}
			}
			for (int i = 0; i < emitters.Length; i++) {
				if (emitters[i].Comments == section) {
					WriteEmitter(writer, emitters[i]);
				}
			}
			for (int i = 0; i < effects.Length; i++) {
				if (effects[i].Comments == section) {
					WriteEffect(writer, effects[i]);
				}
			}

			writer.WriteLine("[endregion]");
		}

		writer.WriteLine();
	}

	protected void WriteParticle(StreamWriter writer, ParticleType p) {
		writer.WriteLine("@particle \"" + p.Name + "\"");

		// Sprites
		if (p.AnimationSpeed != 0)
			writer.WriteLine("@animation_speed " + p.AnimationSpeed.ToString());
		if (p.Sprites.Length > 0) {
			if (p.Sprites[0].Sheet.Name != ParticleSR.DefaultParticleSheet)
				writer.WriteLine("@sheet \"" + p.Sprites[0].Sheet.Name + "\"");

			string spriteLine = "";
			for (int i = 0; i < p.Sprites.Length; i++) {
				spriteLine += " \"" + p.Sprites[i].Name + "\"";
			}
			writer.WriteLine("@sprites" + spriteLine);
		}
		if (p.Colors.Length > 0) {
			string colorLine = "";
			for (int i = 0; i < p.Colors.Length; i++) {
				if (i > 0)
					colorLine += "  ";

				colorLine += " " + p.Colors[i].R.ToString() +
							" " + p.Colors[i].G.ToString() +
							" " + p.Colors[i].B.ToString() +
							" " + p.Colors[i].A.ToString();
			}
			writer.WriteLine("@colors" + colorLine);
		}

		// General
		if (p.LifeSpan != new RangeF(1))
			writer.WriteLine("@life " + p.LifeSpan.Min.ToString() + " " + p.LifeSpan.Max.ToString());
		if (p.FadeDelay != -1)
			writer.WriteLine("@fade " + p.FadeDelay.ToString());

		// Scale
		if (p.InitialScale != new RangeF(1))
			writer.WriteLine("@scale_init " + p.InitialScale.Min.ToString() + " " + p.InitialScale.Max.ToString());
		if (p.ScaleIncrease != RangeF.Zero)
			writer.WriteLine("@scale_increase " + p.ScaleIncrease.Min.ToString() + " " + p.ScaleIncrease.Max.ToString());
		if (p.ScaleJitter != 0)
			writer.WriteLine("@scale_jitter " + p.ScaleJitter.ToString());
		if (p.ScaleClamp != new RangeF(-10000, 10000))
			writer.WriteLine("@scale_clamp " + p.ScaleClamp.Min.ToString() + " " + p.ScaleClamp.Max.ToString());

		// Speed
		if (p.PositionJitter != 0)
			writer.WriteLine("@pos_jitter " + p.PositionJitter.ToString());
		if (p.Speed != RangeF.Zero)
			writer.WriteLine("@speed_init " + p.Speed.Min.ToString() + " " + p.Speed.Max.ToString());
		if (p.SpeedIncrease != RangeF.Zero)
			writer.WriteLine("@speed_increase " + p.SpeedIncrease.Min.ToString() + " " + p.SpeedIncrease.Max.ToString());
		if (p.SpeedFriction != RangeF.Zero)
			writer.WriteLine("@speed_friction " + p.SpeedFriction.Min.ToString() + " " + p.SpeedFriction.Max.ToString());

		// Direction
		if (p.PositionJitter != 0)
			writer.WriteLine("@pos_jitter " + p.PositionJitter.ToString());
		if (p.InitialDirectionOffset != RangeF.Zero)
			writer.WriteLine("@dir_init " + p.InitialDirectionOffset.Min.ToString() + " " + p.InitialDirectionOffset.Max.ToString());
		if (p.DirectionIncrease != RangeF.Zero)
			writer.WriteLine("@dir_increase " + p.DirectionIncrease.Min.ToString() + " " + p.DirectionIncrease.Max.ToString());
		if (p.DirectionFriction != RangeF.Zero)
			writer.WriteLine("@dir_friction " + p.DirectionFriction.Min.ToString() + " " + p.DirectionFriction.Max.ToString());
		if (!p.Gravity.IsZero)
			writer.WriteLine("@gravity " + p.Gravity.X.ToString() + " " + p.Gravity.Y.ToString());

		// Rotation
		if (p.InitialRotation != RangeF.Zero || p.RotateFromDirection)
			writer.WriteLine("@rot_init " +
				p.InitialRotation.Min.ToString() + " " +
				p.InitialRotation.Max.ToString() + " " +
				p.RotateFromDirection.ToString().ToLower());
		if (p.RotationSpeed != RangeF.Zero)
			writer.WriteLine("@rot_increase " + p.RotationSpeed.Min.ToString() + " " + p.RotationSpeed.Max.ToString());
		if (p.RotationFriction != 0)
			writer.WriteLine("@rot_friction " + p.RotationFriction.ToString());
		if (p.RotationJitter != 0)
			writer.WriteLine("@rot_jitter " + p.RotationFriction.ToString());

		writer.WriteLine("@end");
		writer.WriteLine();
	}
	protected void WriteEmitter(StreamWriter writer, ParticleEmitter e) {
		writer.WriteLine("@emitter \"" + e.Name + "\"");

		if (e.Area is PointArea) {
			PointArea a = (PointArea)e.Area;
			if (!a.Point.IsZero) {
				writer.WriteLine("@area \"point\" " + a.Point.X.ToString() + " " + a.Point.Y.ToString());
			}
		}
		else if (e.Area is LineArea) {
			LineArea a = (LineArea)e.Area;
			writer.WriteLine("@area \"line\" " +
				a.End1.X.ToString() + " " +
				a.End1.Y.ToString() + " " +
				a.End2.X.ToString() + " " +
				a.End2.Y.ToString());
		}
		else if (e.Area is RectArea) {
			RectArea a = (RectArea)e.Area;
			writer.WriteLine("@area \"rect\" " +
				a.Point.X.ToString() + " " +
				a.Point.Y.ToString() + " " +
				a.Size.X.ToString() + " " +
				a.Size.Y.ToString() + " " +
				a.EdgeOnly.ToString().ToLower());
		}
		else if (e.Area is CircleArea) {
			CircleArea a = (CircleArea)e.Area;
			writer.WriteLine("@area \"circle\" " +
				a.Center.X.ToString() + " " +
				a.Center.Y.ToString() + " " +
				a.Radius.ToString() + " " +
				a.EdgeOnly.ToString().ToLower());
		}

		if (!e.Origin.IsZero || e.IsOriginBasedDirection || e.IsSpeedScaledByDistance) {
			writer.WriteLine("@origin " +
				e.Origin.X.ToString() + " " +
				e.Origin.Y.ToString() + " " +
				e.IsSpeedScaledByDistance.ToString().ToLower());
		}
		if (!e.Speed.IsZero) {
			writer.WriteLine("@speed " +
				e.Speed.Min.ToString() + " " +
				e.Speed.Max.ToString());
		}
		if (!e.Direction.IsZero) {
			writer.WriteLine("@direction " +
				e.Direction.X.ToString() + " " +
				e.Direction.Y.ToString());
		}
		
		writer.WriteLine("@end");
		writer.WriteLine();
	}
	protected void WriteEffect(StreamWriter writer, ParticleEffectType e) {
		writer.WriteLine("@effect \"" + e.Name + "\"");

		for (int i = 0; i < e.Actions.Count; i++) {
			ParticleEffectAction a = e.Actions[i];
			if (a.IsStream) {
				writer.WriteLine("@stream \"" +
					a.Emitter.Name + "\" \""
					+ a.Type.Name + "\" "
					+ a.Frequency.ToString() + " "
					+ a.Duration.ToString() + " "
					+ a.Delay.ToString());
			}
			else {
				writer.WriteLine("@burst \"" +
					a.Emitter.Name + "\" \""
					+ a.Type.Name + "\" "
					+ ((int)a.Frequency).ToString() + " "
					+ a.Delay.ToString());
			}
		}

		writer.WriteLine("@end");
		writer.WriteLine();
	}

	#endregion
}
} // end namespace
