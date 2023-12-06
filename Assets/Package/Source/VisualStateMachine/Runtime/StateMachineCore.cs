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
		public StateMachineController Controller => _controller;
		
		private readonly bool _stateMachineIsNull;
		private StateMachine _stateMachineInstance;
		private StateMachine _originalStateMachine;

		private readonly StateMachineController _controller;

		public StateMachineCore(StateMachine stateMachine, StateMachineController controller)
		{
			DevLog.Log("StateMachineCore.Ctr");
			
			if (stateMachine == null)
			{
				_stateMachineIsNull = true;
				throw new StateMachineException("StateMachine is null");
			}
			
			_controller = controller;
			_stateMachineInstance = StateMachine.CreateInstance(stateMachine);
			_stateMachineInstance.Initialize(this);
		}

		public void Start()
		{
			DevLog.Log("StateMachineCore.Start");
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.Start();
		}

		public void Update()
		{
			if (_stateMachineIsNull) return;
			if (StateMachine.IsComplete) return;
			
			DevLog.Log($"StateMachineCore.Update: {StateMachine.IsComplete}");
			_stateMachineInstance.Update();
		}
		
		public void Complete()
		{
			if (StateMachine.IsComplete) return;
			
			DevLog.Log("StateMachineCore.Complete");
			StateMachine.Complete();
			OnComplete?.Invoke();
		}
	}
}