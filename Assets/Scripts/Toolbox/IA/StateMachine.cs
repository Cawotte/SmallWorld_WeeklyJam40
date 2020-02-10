
namespace Cawotte.Toolbox.IA {
	
	using UnityEngine;

	/// <summary>
	/// StateMachine that manage a State and holds shared data for states. 
	/// It's "Context" from the design pattern State.
	/// </summary>
	public class StateMachine<T>
	{

		private State<T> currentState;
		protected T subject;

		/// <summary>
		/// Initialize the State Machine with a starting State and the puppet it controls.
		/// </summary>
		/// <param name="startingState"></param>
		public StateMachine(State<T> startingState, T subject) {
			this.subject = subject;
			this.CurrentState = startingState;
		}

		/// <summary>
		/// The Current State of the StateMachine, used to change State.
		/// </summary>
		public State<T>  CurrentState {
			get => currentState;
			set
			{
				currentState?.EndState(); //end the previous state
				currentState = value;
				currentState.StateMachine = this;
				currentState.StartState(); //initialize the new one
			}
		}

		public T Subject { get => subject;  }

		public void Update()
		{
			//Apply current state's behaviour.
			CurrentState.Update();
		}

	}
}

