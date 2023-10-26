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
		
		private VsmGraphData _graphDataInstance;
		private StateNodeData _currentNode;
		private VsmGraphData _graphData;

		private void Awake()
		{
			
		}
	}
}