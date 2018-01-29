using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterStateMachine<IDType> where IDType : struct, IConvertible {
		
		//-----------------------------------------------------------------------------
		// Internal State class
		//-----------------------------------------------------------------------------

		public class State {
			public IDType ID { get; set; }
			public StateBeginDelegate BeginFunction { get; set; }
			public StateUdpateDelegate UpdateFunction { get; set; }
			public StateEndDelegate EndFunction { get; set; }
			public MonsterStateMachine<IDType> StateMachine { get; set; }
			public bool IsActive { get; set; }
			
			public State() {
				IsActive = false;
			}

			public virtual void Begin() {
				if (BeginFunction != null)
					BeginFunction();
				IsActive = true;
			}
			public virtual void Update() {
				if (UpdateFunction != null)
					UpdateFunction();
			}
			public virtual void End() {
				if (EndFunction != null)
					EndFunction();
				IsActive = false;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public class TimedState : State {
			private int timer;
			public RangeI Duration { get; set; }
			public IDType NextState { get; set; }
			private List<Tuple<int, StateEventDelegate>> timedEvents;
			private float activeDuration;

			public TimedState() {
				timedEvents = new List<Tuple<int, StateEventDelegate>>();
			}

			public TimedState AddEvent(int time, StateEventDelegate function) {
				timedEvents.Add(new Tuple<int, StateEventDelegate>
					(time, function));
				return this;
			}

			public TimedState OnBegin(StateBeginDelegate function) {
				BeginFunction = function;
				return this;
			}

			public TimedState OnEnd(StateEndDelegate function) {
				EndFunction = function;
				return this;
			}

			public TimedState OnUpdate(StateUdpateDelegate function) {
				UpdateFunction = function;
				return this;
			}

			public TimedState AppendEvent(int delay, StateEventDelegate function) {
				int time = delay;
				if (timedEvents.Count > 0)
					time += timedEvents[timedEvents.Count - 1].Item1;
				timedEvents.Add(new Tuple<int, StateEventDelegate>
					(time, function));
				return this;
			}

			public TimedState SetDuration(int duration) {
				Duration = new RangeI(duration);
				return this;
			}

			public TimedState SetDuration(int minDuration, int maxDuration) {
				Duration = new RangeI(minDuration, maxDuration);
				return this;
			}

			public TimedState SetDuration(RangeI duration) {
				Duration = duration;
				return this;
			}

			public override void Begin() {
				base.Begin();
				timer = 0;
				activeDuration = GRandom.NextInt(Duration);
			}

			public override void Update() {
				base.Update();
				if (IsActive) {
					timer++;
					foreach (var timedEvent in timedEvents) {
						if (timedEvent.Item1 == timer)
							timedEvent.Item2();
					}
					if (activeDuration >= 0 && timer >= activeDuration) {
						//if (NextState != null)
							//StateMachine.BeginState(NextState);
						//else
							StateMachine.NextState();
					}
				}
			}
		}
		

		public delegate void StateBeginDelegate();
		public delegate void StateUdpateDelegate();
		public delegate void StateEndDelegate();
		public delegate void StateEventDelegate();

		private Dictionary<IDType, State> states;
		private State currentState;
		private List<IDType> stateIds;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterStateMachine() {
			states = new Dictionary<IDType, State>();
			stateIds = new List<IDType>();
			foreach (IDType id in Enum.GetValues(typeof(IDType)))
				stateIds.Add(id);
		}
		

		//-----------------------------------------------------------------------------
		// Setup
		//-----------------------------------------------------------------------------

		public TimedState AddState(IDType id) {
			TimedState state = new TimedState() {
				ID				= id,
				BeginFunction	= null,
				EndFunction		= null,
				UpdateFunction	= null,
				StateMachine	= this,
				Duration		= new RangeI(-1),
			};
			states[id] = state;
			return state;
		}

		

		//-----------------------------------------------------------------------------
		// Operation
		//-----------------------------------------------------------------------------

		public void Update() {
			if (currentState != null)
				currentState.Update();
		}

		public void BeginState(IDType id) {
			if (currentState != null)
				currentState.End();
			currentState = states[id];
			if (currentState != null)
				currentState.Begin();
		}

		public void NextState() {
			int index = stateIds.IndexOf(currentState.ID);
			index = (index + 1) % stateIds.Count;
			BeginState(stateIds[index]);
		}
	}

}
