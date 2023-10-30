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
			_state?.Enter();
		}

		public void Update()
		{
			_state?.Update();
		}

		public void Exit()
		{
			_state?.Exit();
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