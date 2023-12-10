
using UnityEngine;
using UnityEngine.Serialization;
using VisualStateMachine.Attributes;
using VisualStateMachine.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Sub State Machine"), NodeIcon(NodeIcon.VsmFlatWhite, opacity:0.3f)]
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
			SelectStateMachine();
			
			_stateMachineCore.OnComplete += SubStateMachineComplete;
			_stateMachineCore.Start();
		}
		
		public override void UpdateState()
		{
			SelectStateMachine();
			_stateMachineCore.Update();
		}

		public override void ExitState()
		{
			_stateMachineCore.OnComplete -= SubStateMachineComplete;
			SelectParentStateMachine();
		}

		private void CreateCore()
		{
			if (GuardAgainstInfiniteLoopOfDoom()) return;
			
			_stateMachineCore = new StateMachineCore(SubStateMachine, StateMachineCore);
		}

		private bool GuardAgainstInfiniteLoopOfDoom()
		{
			var parent = StateMachineCore.Parent;
			while (parent != null)
			{
				if (parent.OriginalStateMachine == SubStateMachine)
				{
					var errorMsg = $"Infinite loop of doom detected. {this.GetType().Name} in " +
					               $"{this.StateMachineCore.OriginalStateMachine.name} has a StateMachine used" +
					               $" in parent {parent.OriginalStateMachine.name}";
					throw new StateMachineException(errorMsg);
				}

				parent = parent.Parent;
			}

			return false;
		}

		private void SelectParentStateMachine()
		{
			#if UNITY_EDITOR
			{
				if (Selection.activeObject == _stateMachineCore.StateMachine)
				{
					Selection.activeObject = (Object)_stateMachineCore?.Parent.StateMachine ?? _stateMachineCore.Root;
				}
			}
			#endif
		}

		private void SelectStateMachine()
		{
			#if UNITY_EDITOR
			{
				var stateMachineSelected = _stateMachineCore.StateMachine == Selection.activeObject;
				if (stateMachineSelected) return;
				
				var parentSelected = _stateMachineCore.Parent != null &&
										 _stateMachineCore.Parent.StateMachine == Selection.activeObject;
				var rootSelected = _stateMachineCore.Root == Selection.activeObject;

				if (parentSelected || rootSelected)
				{
					Selection.activeObject = _stateMachineCore.StateMachine;
				}
			}
			#endif
		}

		protected abstract void SubStateMachineComplete(State finalState);
	}
}