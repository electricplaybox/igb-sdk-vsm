using UnityEngine;

namespace ExampleOne
{
	public class SpinUI : MonoBehaviour
	{
		[SerializeField] private float _speed = 1f;
		
		public void Update()
		{
			transform.Rotate(0, 0, _speed * Time.deltaTime);
		}
	}
}