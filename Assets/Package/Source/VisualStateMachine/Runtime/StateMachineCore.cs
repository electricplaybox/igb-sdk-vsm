using System;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	public class StateMachineCore
	{
		public event Action<StateMachineCore, State> OnComplete;
		
		public StateMachine StateMachine => Application.isPlaying 
			? _stateMachineInstance 
			: _originalStateMachine;
		
		public State CurrentState => StateMachine.CurrentState;
		public GameObject Root => _root;
		public StateMachineCore Parent => _parent;
		public StateMachine OriginalStateMachine => _originalStateMachine;

		private StateMachine _stateMachineInstance;
		private StateMachine _originalStateMachine;
		private StateMachineCore _parent;
		private GameObject _root;
		private bool _stateMachineIsNull;

		public StateMachineCore(StateMachine stateMachine, GameObject root)
		{
			AwakeStateMachineCore(stateMachine, null, root);
		}
		
		public StateMachineCore(StateMachine stateMachine, StateMachineCore parent)
		{
			AwakeStateMachineCore(stateMachine, parent, parent.Root);
		}
		
		private void AwakeStateMachineCore(StateMachine stateMachine, StateMachineCore parent, GameObject root)
		{
			if (stateMachine == null)
			{
				_stateMachineIsNull = true;
				return;
			}

			_parent = parent;
			_root = root;
			_originalStateMachine = stateMachine;
			_stateMachineInstance = StateMachine.CreateInstance(stateMachine);

			if (!Application.isPlaying) return;
			
			_stateMachineInstance.AwakeStateMachine(this);
		}

		public void Start()
		{
			if (!Application.isPlaying) return;
			
			DevLog.Log($"StateMachineCore.Start: {this.StateMachine.name}");
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.StartStateMachine(this);
		}

		public void Update()
		{
			if (_stateMachineIsNull) return;
			if (StateMachine.IsComplete) return;
			
			_stateMachineInstance.Update();
		}
		
		public void Complete(State finalState)
		{
			Debug.Log($"Complete: {StateMachine.name}, {StateMachine.IsComplete}");
			if (StateMachine.IsComplete) return;
			
			StateMachine.Complete();
			OnComplete?.Invoke(this, finalState);
		}

		public void JumpTo(JumpId jumpId)
		{
			_stateMachineInstance.JumpTo(jumpId);
		}

		public void Dispose()
		{
			_stateMachineInstance.Dispose();
		}
	}
}