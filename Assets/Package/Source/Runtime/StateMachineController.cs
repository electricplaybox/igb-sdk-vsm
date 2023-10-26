using UnityEngine;
using Vsm.Serialization;

namespace Vsm
{
	public class StateMachineController : MonoBehaviour
	{
		[SerializeField] private VsmGraphData _graphData;

		public VsmGraphData GraphData => _graphData;

		public void Start()
		{
		}

		public void Update()
		{
		}

		public void OnDestroy()
		{
		}
	}
}