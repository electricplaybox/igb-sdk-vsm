
using UnityEngine;
using UnityEngine.Serialization;
using VisualStateMachine.Attributes;
using VisualStateMachine.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Sub State Machine"), NodeIcon(NodeIcon.VsmFlatWhite)]
	public abstract class BaseSubStateMachine : State
	{
		[SerializeField] protected StateMachine SubStateMachine;
		
		private StateMachineCore _stateMachineCore;

		public override void InitializeState()
		{
			CreateCore();
		}

		public override void EnterState()
		{
			#if UNITY_EDITOR
			{
				if (Selection.activeObject == _stateMachineCore.Controller.gameObject)
				{
					Selection.activeObject = _stateMachineCore.StateMachine;
				}
			}
			#endif
			
			_stateMachineCore.OnComplete += HandleComplete;
			_stateMachineCore.Start();
		}

		public override void UpdateState()
		{
			_stateMachineCore.Update();
		}

		public override void ExitState()
		{
			_stateMachineCore.OnComplete -= HandleComplete;
			
			#if UNITY_EDITOR
			{
				if (Selection.activeObject == _stateMachineCore.StateMachine)
				{
					Selection.activeObject = _stateMachineCore.Controller.gameObject;
				}
			}
			#endif
			
			CreateCore();
		}

		protected virtual void CreateCore()
		{
			_stateMachineCore = new StateMachineCore(SubStateMachine, StateMachineCore.Controller);
		}

		protected virtual void HandleComplete()
		{
			DevLog.Log("BaseSubStateMachine.HandleComplete");
		}
	}
}