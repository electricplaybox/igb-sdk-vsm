using VisualStateMachine.Editor.ContextMenu;

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
			_toolbar = new StateMachineToolbar(_graphView.StateMachine);
			_graphView.Add(_toolbar);
		}
		
		public void UpdateToolbar() 
		{
			_toolbar.Update(_graphView.StateMachine);
		}

		public void CreateContextMenu() 
		{
			_contextMenu = new StateMachineContextMenu(_graphView);
			_contextMenu.OnCreateNewStateNode += _graphView.StateManager.CreateNewStateNodeFromContextMenu;
		}
	}
}