
namespace Cawotte.Toolbox.IA {

	using UnityEngine;

	/// <summary>
	/// Parent abstract class to represent a State in a State design pattern.
	/// 
	/// Each State has a behaviour implemented in its own Update() method, and will call itself
	/// the next State when a change of State needs to be done.
	/// </summary>

	public abstract class State<T>
	{
		/// <summary>
		/// Reference to the State Machine that holds everything together.
		/// </summary>
		protected StateMachine<T> stateMachine;

		//debug
		public Color stateColor;

		public StateMachine<T> StateMachine { get => stateMachine; set => stateMachine = value; }

		/// <summary>
		/// Overridable method to initialize a State.
		/// </summary>
		public virtual void StartState()
		{
		}

		/// <summary>
		/// Contains the main behaviour of the state
		/// </summary>
		public abstract void Update();

		/// <summary>
		/// Overridable method to terminate a State.
		/// </summary>
		public virtual void EndState()
		{
		}
	}
}


