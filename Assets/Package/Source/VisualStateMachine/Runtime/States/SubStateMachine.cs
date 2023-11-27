using System;
using UnityEngine;
using VisualStateMachine.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Sub State Machine"), NodeIcon(NodeIcon.VsmFlatWhite)]
	public class SubStateMachine : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachineController _subController;
		private GameObject _subControllerGo;

		public override void EnterState()
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

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			_subController.OnComplete -= HandleComplete;
			
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

		private void HandleComplete()
		{
			OnComplete?.Invoke();
		}
	}
}