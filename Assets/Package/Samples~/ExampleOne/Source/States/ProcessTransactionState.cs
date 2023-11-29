using System;
using ExampleOne.Menu;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace ExampleOne.States
{
	public class ProcessTransactionState : State
	{
		[Transition("Success")]
		public event Action OnSuccess;
		
		[Transition("Fail")]
		public event Action OnFail;

		private float _time;

		[SerializeField] private MenuConfig _menuConfig;
		
		public override void EnterState()
		{
			_menuConfig.ShowMenu();
			_time = Time.time;
		}

		public override void UpdateState()
		{
			if (!(Time.time - _time > 2)) return;
			
			var success = UnityEngine.Random.Range(0, 2) == 1;
			if (success)
			{
				OnSuccess?.Invoke();
			}
			else
			{
				OnFail?.Invoke();
			}
		}

		public override void ExitState()
		{
			_menuConfig.HideMenu();
		}
	}
}