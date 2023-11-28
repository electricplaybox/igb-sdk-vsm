using System;
using UnityEngine;

namespace ExampleOne
{
	public class MenuStateMachineData : MonoBehaviour
	{
		[field: NonSerialized] 
		public string PuchaseItemId { get; set; } = string.Empty;
	}
}