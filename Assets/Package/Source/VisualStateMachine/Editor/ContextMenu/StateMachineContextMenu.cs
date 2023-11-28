using System;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Editor.Nodes;

namespace VisualStateMachine.Editor.ContextMenu
{
	public class StateMachineContextMenu
	{
		public event Action<Vector2> OnCreateNewStateNode;
		
		private readonly StateMachineGraphView _graphView;

		public StateMachineContextMenu(StateMachineGraphView graphView)
		{
			_graphView = graphView;
			_graphView.RegisterCallback((ContextualMenuPopulateEvent evt) =>
			{
				switch (evt.target.GetType().Name)
				{
					case "StateMachineGraphView":
						BuildGraphViewContext(evt);
						break;
					case "StateNodeView":
						BuildNodeContext(evt);
						break;
				}
			});
		}

		private void BuildNodeContext(ContextualMenuPopulateEvent evt)
		{
			var node = evt.target as StateNodeView;
		}

		private void BuildGraphViewContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.ClearItems();
			evt.menu.AppendAction("Add State", action =>
			{
				OnCreateNewStateNode?.Invoke(action.eventInfo.mousePosition);
			});
		}
	}
}