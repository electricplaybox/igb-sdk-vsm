using System.Linq;
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

		private StateNodeData _entryNode;

		private void Awake()
		{
			_graphDataInstance = ScriptableObject.Instantiate<VsmGraphData>(_graphData);
			_graphDataInstance.name = _graphData.name + _graphDataInstance.GetInstanceID();
			_graphDataInstance.Initialize();
			Debug.Log($"new:{_graphDataInstance.GetInstanceID()}, old:{_graphData.GetInstanceID()}");

			_entryNode = _graphDataInstance.Nodes.FirstOrDefault(node => node.EntryPoint);
		}

		private void Start()
		{
			_entryNode.Enter();
		}

		private void Update()
		{
			
		}
	}
}