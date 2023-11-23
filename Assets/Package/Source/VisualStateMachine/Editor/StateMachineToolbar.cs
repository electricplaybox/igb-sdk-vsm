using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace VisualStateMachine.Editor
{
	public class StateMachineToolbar : Toolbar
	{
		private ObjectField _graphDataField;

		public StateMachineToolbar(StateMachine stateMachine)
		{
			_graphDataField = new ObjectField("State machine graph:");
			_graphDataField.objectType = typeof(StateMachine);
			_graphDataField.SetValueWithoutNotify(stateMachine);
			_graphDataField.MarkDirtyRepaint();
			Add(_graphDataField);
		}

		public void Update(StateMachine stateMachine)
		{
			_graphDataField.SetValueWithoutNotify(stateMachine);
		}
	}
}