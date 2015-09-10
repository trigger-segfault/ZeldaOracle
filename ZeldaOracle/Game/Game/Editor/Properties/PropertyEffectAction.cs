using System;
using System.Collections.Generic;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Input;

using GameFramework.MyGame.Editor;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyEffectAction :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyEmitterList propEmitter;
	/** <summary> The x property of the point. </summary> */
	protected PropertyParticleList propParticle;
	/** <summary> The x property of the point. </summary> */
	protected PropertyDouble propFrequency;
	/** <summary> The x property of the point. </summary> */
	protected PropertyDouble propDuration;
	/** <summary> The x property of the point. </summary> */
	protected PropertyInt propCount;
	/** <summary> The x property of the point. </summary> */
	protected PropertyDouble propDelay;
	/** <summary> True if the effect action is a stream. </summary> */
	protected bool isStream;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyEffectAction()
		: base(false) {

		this.isStream = true;

		this.propEmitter	= (PropertyEmitterList)AddProperty(new PropertyEmitterList("Emitter", null, null));
		this.propParticle	= (PropertyParticleList)AddProperty(new PropertyParticleList("Particle", null, null));
		this.propFrequency	= (PropertyDouble)AddProperty(new PropertyDouble("Frequency", 0.0, 0.0, new RangeF(0.0, Double.PositiveInfinity)));
		this.propDuration	= (PropertyDouble)AddProperty(new PropertyDouble("Duration", 0.0, 0.0, new RangeF(-1.0, Double.PositiveInfinity)));
		this.propCount		= null;
		this.propDelay		= (PropertyDouble)AddProperty(new PropertyDouble("Delay", 0.0, 0.0, new RangeF(0.0, Double.PositiveInfinity)));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyEffectAction(string name, bool isStream, PropertyAction action = null)
		: base(name, false, null, null, action) {

		this.isStream		= isStream;

		ParticleEmitter emitter = Resources.ParticleEmitters[0];
		ParticleType particle = Resources.ParticleTypes[0];

		this.propEmitter	= (PropertyEmitterList)AddProperty(new PropertyEmitterList("Emitter", emitter, null, action));
		this.propParticle	= (PropertyParticleList)AddProperty(new PropertyParticleList("Particle", particle, null, action));
		if (isStream) {
			this.propFrequency	= (PropertyDouble)AddProperty(new PropertyDouble("Frequency", 0.0, 0.0, new RangeF(0.0, Double.PositiveInfinity), action));
			this.propDuration	= (PropertyDouble)AddProperty(new PropertyDouble("Duration", 0.0, 0.0, new RangeF(-1.0, Double.PositiveInfinity), action));
			this.propCount		= null;
		}
		else {
			this.propFrequency	= null;
			this.propDuration	= null;
			this.propCount		= (PropertyInt)AddProperty(new PropertyInt("Count", 0, 0, new RangeI(0, Int32.MaxValue), action));
		}
		this.propDelay		= (PropertyDouble)AddProperty(new PropertyDouble("Delay", 0.0, 0.0, new RangeF(0.0, Double.PositiveInfinity), action));

		this.value			= new ParticleEffectAction(emitter, particle, 0.0, (isStream ? 0.0 : -1), 0.0, isStream);
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyEffectAction(string name, ParticleEffectAction effectAction, PropertyAction action = null)
		: base(name, false, effectAction, null, action) {

		this.isStream		= effectAction.IsStream;

		this.propEmitter	= (PropertyEmitterList)AddProperty(new PropertyEmitterList("Emitter", effectAction.Emitter, null, action));
		this.propParticle	= (PropertyParticleList)AddProperty(new PropertyParticleList("Particle", effectAction.Type, null, action));
		if (isStream) {
			this.propFrequency	= (PropertyDouble)AddProperty(new PropertyDouble("Frequency", effectAction.Frequency, 0.0, new RangeF(0.0, Double.PositiveInfinity), action));
			this.propDuration	= (PropertyDouble)AddProperty(new PropertyDouble("Duration", effectAction.Duration, 0.0, new RangeF(-1.0, Double.PositiveInfinity), action));
			this.propCount		= null;
		}
		else {
			this.propFrequency	= null;
			this.propDuration	= null;
			this.propCount		= (PropertyInt)AddProperty(new PropertyInt("Count", (int)effectAction.Frequency, 0, new RangeI(0, Int32.MaxValue), action));
		}
		this.propDelay		= (PropertyDouble)AddProperty(new PropertyDouble("Delay", effectAction.Delay, 0.0, new RangeF(0.0, Double.PositiveInfinity), action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public ParticleEffectAction Value {
		get { return (ParticleEffectAction)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public ParticleEffectAction DefaultValue {
		get { return (ParticleEffectAction)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {}
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);

		/*if (propEmitter.ItemCount != Resources.ParticleEmitterCount) {
			propEmitter.ClearItems();
			int index = -1;

			ParticleEmitter[] emitterList = Resources.ParticleEmitters;
			string[] nameList = new string[Resources.ParticleEmitterCount];

			for (int i = 0; i < Resources.ParticleEmitterCount; i++) {
				nameList[i] = emitterList[i].Name;
				if (emitterList[i].Name == Value.Emitter.Name)
					index = i;
			}

			propEmitter.SetItemList(nameList, emitterList);
			propEmitter.Selection = index;
		}
		if (propParticle.ItemCount != Resources.ParticleTypeCount) {
			propParticle.ClearItems();
			int index = -1;

			ParticleType[] particleList = Resources.ParticleTypes;
			string[] nameList = new string[Resources.ParticleTypeCount];

			for (int i = 0; i < Resources.ParticleTypeCount; i++) {
				nameList[i] = particleList[i].Name;
				if (particleList[i].Name == Value.Type.Name)
					index = i;
			}

			propParticle.SetItemList(nameList, particleList);
			propParticle.Selection = index;
		}*/

		if (propEmitter.IsChanged) {
			ParticleEffectAction newValue = (ParticleEffectAction)value;

			newValue.Emitter = propEmitter.SelectionValue;
			value = newValue;
			if (action != null)
				action();
		}
		if (propParticle.IsChanged) {
			ParticleEffectAction newValue = (ParticleEffectAction)value;

			newValue.Type = propParticle.SelectionValue;
			value = newValue;
			if (action != null)
				action();
		}
		if (isStream) {
			if (propFrequency.IsChanged) {
				ParticleEffectAction newValue = (ParticleEffectAction)value;

				newValue.Frequency = propFrequency.Value;
				value = newValue;
				if (action != null)
					action();
			}
			if (propDuration.IsChanged) {
				ParticleEffectAction newValue = (ParticleEffectAction)value;

				newValue.Duration = propDuration.Value;
				value = newValue;
				if (action != null)
					action();
			}
		}
		else {
			if (propCount.IsChanged) {
				ParticleEffectAction newValue = (ParticleEffectAction)value;

				newValue.Frequency = propCount.Value;
				value = newValue;
				if (action != null)
					action();
			}
		}
		if (propDelay.IsChanged) {
			ParticleEffectAction newValue = (ParticleEffectAction)value;

			newValue.Delay = propDelay.Value;
			value = newValue;
			if (action != null)
				action();
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing


	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Sets the value of the property after finished editing. </summary> */
	public override void Finish() {
		Cancel();
	}

	#endregion
}
} // end namespace
