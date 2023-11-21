using System;
using UnityEngine;

namespace VisualStateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		public event Action OnComplete;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachine _stateMachineInstance;
		private bool _isComplete;

		public void OnValidate()
		{
			if (_stateMachine == null) return;
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
		}

		public void SetStateMachine(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			if (_stateMachine == null) return;
			
			_isComplete = false;
			OnValidate();
		}
	
		public void Start()
		{
			if (_stateMachine == null) throw new NullReferenceException("State machine is null");
			
			_stateMachineInstance = StateMachine.CreateInstance(_stateMachine);
			_stateMachineInstance.Initialize(this);
		}

		public void Update()
		{
			if (_isComplete) return;
			if (_stateMachine == null) return;
			
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
			_isComplete = true;
			OnComplete.Invoke();
		}
	}
}