using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	public class VsmGraphView : GraphView
	{
		private readonly VsmContextMenu _contextMenu;
		private readonly VsmDataManager _dataManager;
		private readonly VsmNodeFactory _nodeFactory;
		private readonly VsmPortConnector _portConnector;
		private readonly VsmEffects _vsmEffects;

		private VsmGraphData _graphData;
		private Toolbar _toolbar;

		public VsmGraphView(VsmGraphData graphData)
		{
			CreateGrid();
			
			_portConnector = new VsmPortConnector(this);
			_nodeFactory = new VsmNodeFactory(this);

			_contextMenu = new VsmContextMenu(this);
			_contextMenu.OnDeleteSelection += HandleDeleteSelection;
			_contextMenu.OnSetAsEntryNode += _nodeFactory.SetAsEntryNode;
			_contextMenu.OnCreateNewStateNode += _nodeFactory.CreateNode;

			_dataManager = new VsmDataManager(this);
			_dataManager.OnClearGraph += HandleClearGraph;
			_dataManager.OnCreateNode += _nodeFactory.CreateNode;
			_dataManager.OnConnectPorts += _portConnector.ConnectPorts;
			_dataManager.OnSaved += HandleDataSaved;
			_dataManager.OnLoaded += HandleDataLoaded;
			_dataManager.LoadData(graphData);

			_vsmEffects = new VsmEffects(this);
			_toolbar = new VsmToolBar(_dataManager, _graphData, this);

			graphViewChanged += HandleGraphChanged;
		}

		private void HandleDataLoaded(VsmGraphData graphData)
		{
			_graphData = graphData;
		}

		private void HandleDataSaved(VsmGraphData graphData)
		{
			_graphData = graphData;
		}

		private void HandleClearGraph()
		{
			DeleteElements(graphElements);
		}

		private void HandleDeleteSelection()
		{
			DeleteSelection();
		}

		private GraphViewChange HandleGraphChanged(GraphViewChange graphViewChange)
		{
			_dataManager.SaveData();

			return graphViewChange;
		}

		private void CreateGrid()
		{
			styleSheets.Add(Resources.Load<StyleSheet>("VsmGraph"));
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());

			var grid = new GridBackground();
			Insert(0, grid);
			grid.StretchToParentSize();
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			ports.ForEach(port =>
			{
				if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
			});
			return compatiblePorts;
		}

		public void Dispose()
		{
			if (_graphData == null) return;

			_dataManager.SaveData();

			_contextMenu.OnDeleteSelection -= HandleDeleteSelection;
			_contextMenu.OnSetAsEntryNode -= _nodeFactory.SetAsEntryNode;
			_contextMenu.OnCreateNewStateNode -= _nodeFactory.CreateNode;

			_dataManager.OnClearGraph -= HandleClearGraph;
			_dataManager.OnCreateNode -= _nodeFactory.CreateNode;
			_dataManager.OnConnectPorts -= _portConnector.ConnectPorts;
			_dataManager.OnSaved -= HandleDataSaved;
			_dataManager.OnLoaded -= HandleDataLoaded;
		}

		public void SaveData()
		{
			_dataManager.SaveData();
		}
	}
}