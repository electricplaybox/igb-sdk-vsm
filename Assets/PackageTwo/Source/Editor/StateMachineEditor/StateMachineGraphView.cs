using StateMachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachineToolbar _toolbar;

		public StateMachineGraphView(StateMachineGraph graph)
		{
			this.AddToClassList("stretch-to-parent-size");
			this.RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
			CreateToolbar(graph);
		}

		private void OnDestroy()
		{
			_toolbar.OnSave -= HandleSaveGraph;
			_toolbar.OnGraphChanged -= HandleGraphChanged;
		}

		private void CreateToolbar(StateMachineGraph graph)
		{
			_toolbar = new StateMachineToolbar(graph);
			_toolbar.OnSave += HandleSaveGraph;
			_toolbar.OnGraphChanged += HandleGraphChanged;
			
			Add(_toolbar);
		}

		private void HandleGraphChanged(StateMachineGraph graph)
		{
			throw new System.NotImplementedException();
		}

		private void HandleSaveGraph()
		{
			throw new System.NotImplementedException();
		}

		private void CreateGrid()
		{
			var grid = new GridBackground();
			grid.name = "grid";
			grid.AddToClassList("stretch-to-parent-size");
			Insert(0, grid);
		}

		private void AddManipulators()
		{
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());
		}
	}
}