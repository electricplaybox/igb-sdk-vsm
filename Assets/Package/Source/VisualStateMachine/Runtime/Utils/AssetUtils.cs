#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace VisualStateMachine.Editor.Utils
{
	public class AssetUtils
	{
		public static void RemoveSubAsset(string assetPath, string subAssetName)
		{
			Debug.Log($"AssetUtils.RemoveSubAsset: {subAssetName}, {assetPath}");
			
			#if UNITY_EDITOR
			{
				var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

				foreach (var subAsset in subAssets)
				{
					if (subAsset == null)
					{
						RemoveGhostSubAsset(assetPath, subAssetName);
						continue;
					}
					else if (subAsset.name != subAssetName)
					{
						continue;
					}
				
					AssetDatabase.RemoveObjectFromAsset(subAsset);
					break;
				}

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			#endif
		}

		/**
		 * A ghost sub asset is one that's class has been deleted but the physical asset remains
		 */
		public static void RemoveGhostSubAsset(string assetPath, string subAssetName)
		{
			Debug.Log($"AssetUtils.RemoveGhostSubAsset: {subAssetName}, {assetPath}");
			
			#if UNITY_EDITOR
			{
				var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
				Debug.Log($"subAssets.Length: {subAssets.Length}");
				
				var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
				Debug.Log($"mainAsset: {mainAsset}");

				var asset = AssetDatabase.LoadAssetAtPath<StateMachine>(assetPath);
				Debug.Log($"asset: {asset}");

				foreach (var subAsset in subAssets)
				{
					Debug.Log($"subAsset: {subAsset}, {subAssetName}, {assetPath}");
				}
			}
			#endif
		}
	}
}