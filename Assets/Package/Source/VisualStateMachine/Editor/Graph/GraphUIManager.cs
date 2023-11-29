using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualStateMachine.Editor.ContextMenu;
using VisualStateMachine.Editor.Manipulators;

namespace VisualStateMachine.Editor
{
	public class GraphUIManager
	{
		private StateMachineGraphView _graphView;
		private StateMachineToolbar _toolbar;
		private StateMachineContextMenu _contextMenu;

		public GraphUIManager(StateMachineGraphView graphView) 
		{
			_graphView = graphView;
		}

		public void CreateToolbar() 
		{
			_toolbar = new StateMachineToolbar(_graphView.StateManager.StateMachine);
			_graphView.Add(_toolbar);
		}
		
		public void UpdateToolbar() 
		{
			_toolbar.Update(_graphView.StateManager.StateMachine);
		}

		public void CreateContextMenu() 
		{
			_contextMenu = new StateMachineContextMenu(_graphView);
			_contextMenu.OnCreateNewStateNode += _graphView.StateManager.CreateNewStateNodeFromContextMenu;
		}
		
		public void CreateGrid()
		{
			var grid = new GridBackground();
			grid.name = "grid";
			grid.AddToClassList("stretch-to-parent-size");
			_graphView.Insert(0, grid);
		}
		
		public void AddManipulators()
		{
			var contentDragger = new StateMachineGraphContentDragger();
			contentDragger.OnDrag += _graphView.HandleGraphDragged;
			
			_graphView.AddManipulator(contentDragger);
			_graphView.AddManipulator(new SelectionDragger());
			_graphView.AddManipulator(new RectangleSelector());
			_graphView.AddManipulator(new FreehandSelector());
		}

		public void CreateEmptyGraphView()
		{
			_graphView.AddToClassList("stretch-to-parent-size");
			_graphView.RegisterCallback<DetachFromPanelEvent>(evt => _graphView.OnDestroy());
			_graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			CreateGrid();
			AddManipulators();
			CreateToolbar();
			CreateContextMenu();
		}

	}
}