using System.Collections.Generic;
using UnityEngine;
using VisualStateMachine.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Parallel Sub State Machine"), NodeIcon(NodeIcon.VsmFlatWhite, opacity:0.3f)]
	public abstract class BaseParallelSubStateMachine : State
	{
		[SerializeField] protected List<StateMachine> SubStateMachines;

		private List<StateMachineCore> _stateMachineCores = new();
		
		public override void InitializeState()
		{
			CreateCores();
		}
		
		public override void EnterState()
		{
			SelectStateMachines();
			
			foreach(var stateMachineCore in _stateMachineCores)
			{
				stateMachineCore.OnComplete += SubStateMachineComplete;
				stateMachineCore.Start();
			}
		}
		
		public override void UpdateState()
		{
			foreach(var stateMachineCore in _stateMachineCores)
			{
				stateMachineCore.Update();
			}
		}
		
		public override void ExitState()
		{
			foreach(var stateMachineCore in _stateMachineCores)
			{
				stateMachineCore.OnComplete -= SubStateMachineComplete;
			}
			
			SelectParentStateMachine();
		}
		
		private void CreateCores()
		{
			foreach (var subStateMachine in SubStateMachines)
			{
				if (GuardAgainstInfiniteLoopOfDoom(subStateMachine)) continue;
			
				var stateMachineCore = new StateMachineCore(subStateMachine, StateMachineCore);
				_stateMachineCores.Add(stateMachineCore);
			}
		}
		
		private bool GuardAgainstInfiniteLoopOfDoom(StateMachine subStateMachine)
		{
			var parent = StateMachineCore.Parent;
			while (parent != null)
			{
				if (parent.OriginalStateMachine == subStateMachine)
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
				foreach (var stateMachineCore in _stateMachineCores)
				{
					if (Selection.activeObject == stateMachineCore.StateMachine)
					{
						Selection.activeObject = (Object)stateMachineCore?.Parent.StateMachine ?? stateMachineCore.Root;
					}
				}
			}
			#endif
		}
		
		private void SelectStateMachines()
		{
			#if UNITY_EDITOR
			{
				foreach(var stateMachineCore in _stateMachineCores)
				{
					var stateMachineSelected = stateMachineCore.StateMachine == Selection.activeObject;
					if (stateMachineSelected) return;
				
					var parentSelected = stateMachineCore.Parent != null &&
					                     stateMachineCore.Parent.StateMachine == Selection.activeObject;
					var rootSelected = stateMachineCore.Root == Selection.activeObject;

					if (parentSelected || rootSelected)
					{
						Selection.activeObject = stateMachineCore.StateMachine;
					}
				}
			}
			#endif
		}
		
		protected abstract void SubStateMachineComplete(StateMachineCore core, State finalState);
		
	}
}