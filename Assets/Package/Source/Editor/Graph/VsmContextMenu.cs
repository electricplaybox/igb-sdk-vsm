using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Vsm.Editor.Nodes;
using Vsm.States;

namespace Vsm.Editor.Graph
{
	public class VsmContextMenu
	{
		private readonly VsmGraphView _graphView;

		public VsmContextMenu(VsmGraphView graphView)
		{
			_graphView = graphView;
			_graphView.RegisterCallback((ContextualMenuPopulateEvent evt) =>
			{
				switch (evt.target.GetType().Name)
				{
					case "VsmGraphView":
						BuildGraphViewContext(evt);
						break;
					case "StateNode":
						BuildNodeContext(evt);
						break;
				}
			});
		}

		public event Action OnDeleteSelection;
		public event Action<StateNode> OnSetAsEntryNode;
		public event Action<Type, Vector2> OnCreateNewStateNode;

		private void BuildNodeContext(ContextualMenuPopulateEvent evt)
		{
			var node = evt.target as BaseNode;

			evt.menu.AppendAction("Set as Entry Node", x =>
			{
				OnSetAsEntryNode.Invoke(node as StateNode);
			});

			evt.menu.AppendAction("Delete", x => { OnDeleteSelection.Invoke(); });
		}

		private void BuildGraphViewContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendSeparator();
			var states = GetAllDerivedTypes<State>();

			foreach (var stateType in states)
				evt.menu.AppendAction(stateType.Name, x =>
				{
					var state = stateType;

					//var mousePositionGlobal = x.eventInfo.mousePosition;
					//var mousePositionLocal = _graphView.ChangeCoordinatesTo(_graphView.contentViewContainer, mousePositionGlobal);
					OnCreateNewStateNode.Invoke(state, x.eventInfo.mousePosition);
				});
		}

		public static List<Type> GetAllDerivedTypes<T>()
		{
			var derivedTypes = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
					if (type.IsSubclassOf(typeof(T)))
						derivedTypes.Add(type);
			}

			return derivedTypes;
		}
	}
}