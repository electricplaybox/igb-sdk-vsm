using UnityEngine;
using Vsm.Serialization;

namespace Vsm
{
	public class StateMachineController : MonoBehaviour
	{
		public VsmGraphData GraphData
		{
			get => _graphData;
			set => _graphData = value;
		}

		public VsmGraphData LiveGraphData => _graphDataInstance != null ? _graphDataInstance : _graphData;

		private VsmGraphData _graphDataInstance;
		private StateNodeData _currentNode;
		
		[SerializeField, HideInInspector]
		private VsmGraphData _graphData;

		private void Awake()
		{
			_graphDataInstance = ScriptableObject.Instantiate<VsmGraphData>(_graphData);
			_graphDataInstance.name = _graphData.name + _graphDataInstance.GetInstanceID();
		}
	}
}