using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	public class VsmToolBar : Toolbar
	{
		private readonly VsmDataManager _dataManager;
		private readonly ObjectField _graphDataField;
		private readonly GraphView _graphView;

		public VsmToolBar(VsmDataManager dataManager, VsmGraphData graphData, GraphView graphView)
		{
			_graphView = graphView;
			_dataManager = dataManager;
			_dataManager.OnSaved += HandleDataSaved;

			_graphDataField = new ObjectField("Graph Data:");
			_graphDataField.objectType = typeof(VsmGraphData);
			_graphDataField.SetValueWithoutNotify(graphData);
			_graphDataField.MarkDirtyRepaint();
			_graphDataField.RegisterValueChangedCallback(evt =>
			{
				_dataManager.LoadData(evt.newValue as VsmGraphData);
			});
			Add(_graphDataField);

			var saveButton = new Button();
			saveButton.text = "Save";
			saveButton.clicked += () => _dataManager.SaveData();
			Add(saveButton);

			_graphView.Add(this);
		}

		private void HandleDataSaved(VsmGraphData graphData)
		{
			_graphDataField.SetValueWithoutNotify(graphData);
		}
	}
}