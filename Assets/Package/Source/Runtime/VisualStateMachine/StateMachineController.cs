using System;
using UnityEngine;

namespace VisualStateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		public event Action OnComplete;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachine _stateMachineInstance;
		private bool _stateMachineIsNull;

		public void OnValidate()
		{
			if (_stateMachine == null) return;
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
		}

		public void SetStateMachine(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			OnValidate();
		}
	
		public void Start()
		{
			_stateMachineIsNull = _stateMachine == null;
			if(_stateMachineIsNull) throw new NullReferenceException("State machine is null");
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
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

		public void Complete()
		{
			OnComplete.Invoke();
		}
	}
}