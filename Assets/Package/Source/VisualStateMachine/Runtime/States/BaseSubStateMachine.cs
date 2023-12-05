
using UnityEngine;
using UnityEngine.Serialization;
using VisualStateMachine.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Purple), NodeLabel("Sub State Machine"), NodeIcon(NodeIcon.VsmFlatWhite)]
	public abstract class BaseSubStateMachine : State
	{
		[FormerlySerializedAs("StateMachine")] [SerializeField] 
		protected StateMachine SubStateMachine;
		
		protected StateMachineController SubController;
		private GameObject _subControllerGo;

		public override void EnterState()
		{
			_subControllerGo = new GameObject("SubController");
			SubController = _subControllerGo.AddComponent<StateMachineController>();
			SubController.OnComplete += HandleComplete;
			SubController.transform.SetParent(this.Controller.transform);
			SubController.SetStateMachine(SubStateMachine);

			#if UNITY_EDITOR
			{
				if (Selection.activeObject == Controller.gameObject)
				{
					Selection.activeObject = SubController.gameObject;
				}
			}
			#endif
		}

		public override void ExitState()
		{
			SubController.OnComplete -= HandleComplete;
			
			#if UNITY_EDITOR
			{
				if (Selection.activeObject == SubController.gameObject)
				{
					Selection.activeObject = Controller.gameObject;
				}
			}
			#endif
			
			Destroy(_subControllerGo);
		}

		protected abstract void HandleComplete();
	}
}