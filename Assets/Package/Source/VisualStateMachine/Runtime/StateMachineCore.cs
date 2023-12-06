using System;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	public class StateMachineCore
	{
		public event Action OnComplete;
		
		public StateMachine StateMachine => Application.isPlaying 
			? _stateMachineInstance 
			: _originalStateMachine;
		
		public State CurrentState => StateMachine.CurrentState;
		
		private readonly bool _stateMachineIsNull;
		private StateMachine _stateMachineInstance;
		private StateMachine _originalStateMachine;
		private bool _isComplete;
		
		public StateMachineCore(StateMachine stateMachine, StateMachineController controller)
		{
			DevLog.Log("StateMachineCore.Ctr");
			
			if (stateMachine == null)
			{
				_stateMachineIsNull = true;
				throw new StateMachineException("StateMachine is null");
			}
			
			_stateMachineInstance = StateMachine.CreateInstance(stateMachine);
			_stateMachineInstance.Initialize(controller);
		}

		public void Start()
		{
			DevLog.Log("StateMachineCore.Start");
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.Start();
		}

		public void Update()
		{
			DevLog.Log("StateMachineCore.Update");
			if (_stateMachineIsNull) return;
			if (_isComplete) return;
			
			_stateMachineInstance.Update();
		}
		
		public void Complete()
		{
			DevLog.Log("StateMachineCore.Complete");
			_isComplete = true;
			OnComplete?.Invoke();
		}
	}
}