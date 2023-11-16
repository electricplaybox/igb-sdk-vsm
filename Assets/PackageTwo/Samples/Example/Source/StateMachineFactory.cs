using UnityEngine;
using VisualStateMachine;

namespace Example
{
	public class StateMachineFactory : MonoBehaviour
	{
		[SerializeField] private StateMachine _stateMachine;

		private void OnValidate()
		{
			if (_stateMachine == null) return;
			
			var foo = new StateNode(typeof(FooBarState), _stateMachine);
			var bar = new StateNode(typeof(BarState), _stateMachine);
			
			foo.AddConnection(new StateConnection(foo.Id, "ExitOne", bar.Id));
			bar.AddConnection(new StateConnection(bar.Id, "ExitOne", foo.Id));
			
			_stateMachine.AddNode(foo);
			_stateMachine.AddNode(bar);
			_stateMachine.SetEntryNode(foo);
		}
	}
}