using Package.Source.Runtime.StateMachine;
using UnityEditor;
using UnityEngine;

namespace Package.Source.Editor.StateMachineEditor
{
	[CreateAssetMenu(fileName = "StateMachineWindowData", menuName = "StateMachine/WindowData")]
	public class StateMachineWindowData : ScriptableObject
	{
		public StateMachineGraph StateMachineGraph
		{
			get
			{
				_stateMachineGraph = StateMachineController != null 
					? StateMachineController.GraphData 
					: _stateMachineGraph;

			return _stateMachineGraph;
			}
		}

		public StateMachineController StateMachineController
		{
			get
			{
				if (_stateMachineControllerId == 0) return default;
				return EditorUtility.InstanceIDToObject(_stateMachineControllerId) as StateMachineController;
			}
		}

		private StateMachineGraph _stateMachineGraph;
		private int _stateMachineControllerId;

		/**
		 * Store the controller as an instanceId so the reference isn't lost at runtime
		 */
		public void SetStateMachineController(StateMachineController controller)
		{
			_stateMachineControllerId = controller.GetInstanceID();
		}

		public void SetStateMachineGraph(StateMachineGraph graph)
		{
			_stateMachineGraph = graph;

			if (StateMachineController == null) return;
			if (_stateMachineControllerId == 0) return;
			if (StateMachineController.GraphData == _stateMachineGraph) return;

			_stateMachineControllerId = 0;
		}
	}
}