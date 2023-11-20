using System;
using UnityEngine;
using VisualStateMachine.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Sub State Machine")]
	public class SubStateMachine : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachineController _subController;
		private GameObject _subControllerGo;

		public override void Enter()
		{
			_subControllerGo = new GameObject("SubController");
			_subController = _subControllerGo.AddComponent<StateMachineController>();
			_subController.OnComplete += HandleComplete;
			_subController.transform.SetParent(this.Controller.transform);
			_subController.SetStateMachine(_stateMachine);

			#if UNITY_EDITOR
			{
				if (Selection.activeObject == Controller.gameObject)
				{
					Selection.activeObject = _subController.gameObject;
				}
			}
			#endif
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			_subController.OnComplete -= HandleComplete;
		}

		private void HandleComplete()
		{
			OnComplete?.Invoke();
			
			#if UNITY_EDITOR
			{
				if (Selection.activeObject == _subController.gameObject)
				{
					Selection.activeObject = Controller.gameObject;
				}
			}
			#endif
			
			Destroy(_subControllerGo);
		}
	}
}