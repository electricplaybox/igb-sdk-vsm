using System;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	public class StateMachineCore
	{
		public event Action<State> OnComplete;
		
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
			InitializeStateMachineCore(stateMachine, null, root);
		}
		
		public StateMachineCore(StateMachine stateMachine, StateMachineCore parent)
		{
			InitializeStateMachineCore(stateMachine, parent, parent.Root);
		}
		
		private void InitializeStateMachineCore(StateMachine stateMachine, StateMachineCore parent, GameObject root)
		{
			if (stateMachine == null)
			{
				_stateMachineIsNull = true;
				throw new StateMachineException("StateMachine is null");
			}

			_parent = parent;
			_root = root;
			_originalStateMachine = stateMachine;
			_stateMachineInstance = StateMachine.CreateInstance(stateMachine);
			_stateMachineInstance.Initialize(this);
		}

		public void Start()
		{
			if (_stateMachineIsNull) return;
			
			_stateMachineInstance.Start();
		}

		public void Update()
		{
			if (_stateMachineIsNull) return;
			if (StateMachine.IsComplete) return;
			
			_stateMachineInstance.Update();
		}
		
		public void Complete(State finalState)
		{
			if (StateMachine.IsComplete) return;
			
			StateMachine.Complete();
			OnComplete?.Invoke(finalState);
		}
	}
}