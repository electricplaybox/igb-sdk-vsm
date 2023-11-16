using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualStateMachine;

namespace Editor.VisualStateMachineEditor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachine _stateMachine;

		public StateMachineGraphView()
		{
			CreateEmptyGraphView();
		}
		
		public StateMachineGraphView(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;

			CreateEmptyGraphView();
		}
		
		public void Update(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
		}

		private void CreateEmptyGraphView()
		{
			AddToClassList("stretch-to-parent-size");
			RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
		}

		private void OnDestroy()
		{
			
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