using System;
using Editor.Utils;
using StateMachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateMachineContextMenu
	{
		public event Action<Type, Vector2> OnCreateNewStateNode;
		
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
					case "StateNode":
						BuildNodeContext(evt);
						break;
				}
			});
		}

		private void BuildNodeContext(ContextualMenuPopulateEvent evt)
		{
			
		}

		private void BuildGraphViewContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendSeparator();
			var states = AssetUtils.GetAllDerivedTypes<State>();

			foreach (var stateType in states)
			{
				evt.menu.AppendAction(stateType.Name, action =>
				{
					var state = stateType;
					OnCreateNewStateNode.Invoke(state, action.eventInfo.mousePosition);
				});
			}
		}
	}
}