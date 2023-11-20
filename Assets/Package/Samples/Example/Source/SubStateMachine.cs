using System;
using UnityEngine;
using VisualStateMachine;

namespace Package.Samples.Example.Source
{
	public class SubStateMachine : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachineController _subController;

		public override void Enter()
		{
			var subControllerGo = new GameObject("SubController");
			
			_subController = subControllerGo.AddComponent<StateMachineController>();
			_subController.OnComplete += HandleComplete;
			_subController.transform.SetParent(this.Controller.transform);
			_subController.SetStateMachine(_stateMachine);
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			_subController.OnComplete -= HandleComplete;
			Destroy(_subController.gameObject);
		}

		private void HandleComplete()
		{
			OnComplete?.Invoke();
		}
	}
}