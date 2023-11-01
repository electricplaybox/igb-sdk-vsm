using System;
using UnityEngine;

namespace Vsm.Serialization
{
	[Serializable]
	public class StateNodeData : BaseNodeData
	{
		public string State;
		public string Guid;
		public bool EntryPoint;
		
		[NonSerialized]
		public bool IsActive;

		public void Enter()
		{
			Debug.Log($"Enter: {this.GetHashCode()}");
			IsActive = true;
			_state?.OnEnter();
		}

		public void Update()
		{
			_state?.OnUpdate();
		}

		public void Exit()
		{
			_state?.OnExit();
			IsActive = false;
		}

		private States.State _state;
		
		public void Initialize()
		{
			var type = Type.GetType(State);
			if (type == null) return;
			
			_state = Activator.CreateInstance(type) as States.State;
		}
	}
}