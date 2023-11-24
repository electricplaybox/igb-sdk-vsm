
using System.IO;
using UnityEditor;
using Directory = UnityEngine.Windows.Directory;

namespace Project.Editor
{
	public static class SampleFolderUtility
	{
		private const string SampleFolderName = "Samples";
		private const string HiddenSampleFolderName = "Samples~";
		private const string PackagePath = "Assets/Package";

		[MenuItem("Tools/Toggle Sample Folder Visibility")]
		public static void ToggleSampleFolderVisibility()
		{
			var sampleFolderPath = Path.Combine(PackagePath, SampleFolderName);
			var hiddenFolderPath = Path.Combine(PackagePath, HiddenSampleFolderName);

			if (Directory.Exists(sampleFolderPath))
			{
				AssetDatabase.MoveAsset(sampleFolderPath, hiddenFolderPath);
			}
			else if (Directory.Exists(hiddenFolderPath))
			{
				AssetDatabase.MoveAsset(hiddenFolderPath, sampleFolderPath);
			}

			AssetDatabase.Refresh();
		}
	}

}