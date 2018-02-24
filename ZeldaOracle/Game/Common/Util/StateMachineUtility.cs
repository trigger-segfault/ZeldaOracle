using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Util {

	/// <summary>
	/// Utility class that can handle a single state at a time, using an enum type as
	/// the state ID. Each state can be created with callbacks for begin/end/update, 
	/// and can also include timed events.
	/// </summary>
	/// <typeparam name="IDType">The enum used for state IDs.</typeparam>
	public class GenericStateMachine<IDType> where IDType : struct, IConvertible {
		
		//-----------------------------------------------------------------------------
		// Internal State class
		//-----------------------------------------------------------------------------

		public class State {
			public IDType ID { get; set; }
			public StateBeginDelegate BeginFunction { get; set; }
			public StateUdpateDelegate UpdateFunction { get; set; }
			public StateEndDelegate EndFunction { get; set; }
			public GenericStateMachine<IDType> StateMachine { get; set; }
			public bool IsActive { get; set; }
			
			public State() {
				IsActive = false;
			}

			public virtual void Begin() {
				BeginFunction?.Invoke();
				IsActive = true;
			}

			public virtual void Update() {
				UpdateFunction?.Invoke();
			}

			public virtual void End() {
				EndFunction?.Invoke();
				IsActive = false;
			}
		}
		
		public class TimedState : State {
			private int timer;
			public RangeI Duration { get; set; }
			public IDType NextState { get; set; }
			private List<Tuple<int, StateEventDelegate>> timedEvents;
			private int activeDuration;

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

			public int Timer {
				get { return timer; }
				set { timer = value; }
			}

			public int ActiveDuration {
				get { return activeDuration; }
				set { activeDuration = value; }
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

		public GenericStateMachine() {
			states = new Dictionary<IDType, State>();
			stateIds = new List<IDType>();
			foreach (IDType id in Enum.GetValues(typeof(IDType)))
				stateIds.Add(id);
		}
		

		//-----------------------------------------------------------------------------
		// Configuration
		//-----------------------------------------------------------------------------

		/// <summary>Add a new state using the given ID.</summary>
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
		// State Transitions
		//-----------------------------------------------------------------------------

		/// <summary>Begin the state machine on the state with the given ID. This will
		/// not end the previous state if one was active.</summary>
		public void InitializeOnState(IDType id) {
			currentState = states[id];
			if (currentState != null)
				currentState.Begin();
		}

		/// <summary>Begin or transition to the state with the given ID.</summary>
		public void BeginState(IDType id) {
			if (currentState != null)
				currentState.End();
			currentState = states[id];
			if (currentState != null)
				currentState.Begin();
		}

		/// <summary>End the currently active state.</summary>
		public void EndCurrentState() {
			if (currentState != null)
				currentState.End();
			currentState = null;
		}

		/// <summary>Transition to the next state as defined by the order of the
		/// state ID values.</summary>
		public void NextState() {
			int index = stateIds.IndexOf(currentState.ID);
			index = (index + 1) % stateIds.Count;
			BeginState(stateIds[index]);
		}


		//-----------------------------------------------------------------------------
		// Update
		//-----------------------------------------------------------------------------

		/// <summary>Update the currently active state.</summary>
		public void Update() {
			if (currentState != null)
				currentState.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns the ID of the current state.</summary>
		public IDType CurrentState {
			get { return currentState.ID; }
		}
	}

}
