using System;
using UnityEngine;

namespace Utils
{
	public static class ScriptableObjectUtility
	{
		public static T FromJson<T>(string json) where T : ScriptableObject
		{
			T instance = ScriptableObject.CreateInstance<T>();
			JsonUtility.FromJsonOverwrite(json, instance);
			return instance;
		}
		
		public static T FromJson<T>(string json, Type type) where T : ScriptableObject
		{
			var instance = ScriptableObject.CreateInstance(type);
			JsonUtility.FromJsonOverwrite(json, instance);
			return instance as T;
		}
	}

}