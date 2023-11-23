using UnityEngine;

namespace VisualStateMachine.Tools
{
	public class DevLog
	{
		public static void Log(object msg, Object context = null)
		{
			#if DEVELOPMENT_BUILD
			{
				Debug.Log(msg, context);
			}
			#endif
		}
	}
}