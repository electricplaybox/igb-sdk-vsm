using System;
using UnityEngine;

namespace StateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachine _stateMachineInstance;
		private bool _stateMachineIsNull;

		public void OnValidate()
		{
			if (_stateMachine == null) return;
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
		}
		
		public void Awake()
		{
			_stateMachineIsNull = _stateMachine == null;
			if(_stateMachineIsNull) throw new NullReferenceException("State machine is null");
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
			Debug.Log($"StateMachine: {_stateMachineInstance.GetInstanceID()}");
		}

		public void Start()
		{
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.Initialize(this);
		}

		public void Update()
		{
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.Update();
		}

		public StateMachine GetStateMachine()
		{
			if (Application.isPlaying)
			{
				return _stateMachineInstance;
			}
			else
			{
				return _stateMachine;
			}
		}
	}
}