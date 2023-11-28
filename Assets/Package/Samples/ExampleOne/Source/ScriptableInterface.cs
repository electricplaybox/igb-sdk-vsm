using UnityEngine;

namespace ExampleOne
{
	public class ScriptableInterface<T> : ScriptableObject
	{
		protected T Instance;

		public void Register(T instance)
		{
			Instance = instance;
		}
		
		public void UnRegister(T instance)
		{
			Instance = instance;
		}
	}
}