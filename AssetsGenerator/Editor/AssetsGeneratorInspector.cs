using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AssetsGenerator), true)]
public class AssetsGeneratorInspector : Editor {

	public class CopyData {
		public string path;
		public List<FileInfo> files = new List<FileInfo> ();
		public CopyData (string path) {
			this.path = path;
		}
	}

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();

		AssetsGenerator t = this.target as AssetsGenerator;
		if (GUILayout.Button ("Generate")) {
			if (Directory.Exists (t.targetPath)) {
				Debug.LogError (t.targetPath + " is Exists, there not support replace because this is unsafe for project. Please remove this folder with Manual.");
				return;
			}
			Dictionary<string, CopyData> pathData = new Dictionary<string, CopyData> ();
			foreach (string sp in t.sourcePaths) {
				DirectoryInfo di = new DirectoryInfo (sp);
				FillPathData (pathData, Path.GetFullPath (sp), Path.GetFullPath (t.targetPath), di);
			}
			Generate (pathData, t.copyMeta);
		}
	}

	public static void FillPathData (Dictionary<string, CopyData> pathData, string sourcePath, string target, DirectoryInfo di) {
		foreach (FileInfo fi in di.GetFiles ()) {
			if (fi.Extension == ".meta") {
				continue;
			}
			string fullTargetPath = fi.FullName.Replace (sourcePath, target);
			string key = fullTargetPath.ToLower ();
			if (!pathData.ContainsKey (key)) {
				pathData[key] = new CopyData (fullTargetPath);
			}
			pathData[key].files.Add (fi);
		}
		foreach (DirectoryInfo di2 in di.GetDirectories ()) {
			FillPathData (pathData, sourcePath, target, di2);
		}
	}
	public static void Generate (Dictionary<string, CopyData> pathData, bool copyMeta) {
		int count = 0;
		foreach (var kvp in pathData) {
			count++;
			if (EditorUtility.DisplayCancelableProgressBar ("Copying", kvp.Value.path, (float)count / (float)pathData.Count)) {
				break;
			}
			if (kvp.Value.files.Count == 1) {
				Create (kvp.Value.path, kvp.Value.files[0], copyMeta);
			}
			else {
				int index = 0;
				int selectIndex = 2;
				while (selectIndex == 2) {
					string message = "A Files is Multiple, Please select one.\n";
					for (int i = 0; i < kvp.Value.files.Count; i++) {
						FileInfo assetPath = kvp.Value.files[i];
						if (i > 0) {
							message += "\n";
						}
						message += (i == (index % kvp.Value.files.Count) ? "--->" : "----") + " " + assetPath.FullName;
					}
					selectIndex = EditorUtility.DisplayDialogComplex ("Multiple Files", message, "Select", "Cancle", "Change");
					if (selectIndex == 2) {
						index++;
					}
				}
				if (selectIndex == 1) { // Cancle
					break;
				}
				Create (kvp.Value.path, kvp.Value.files[index % kvp.Value.files.Count], copyMeta);
			}
		}
		EditorUtility.ClearProgressBar ();

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}
	public static void Create (string targetPath, FileInfo source, bool copyMeta) {
		Directory.CreateDirectory (Path.GetDirectoryName (targetPath));

		source.CopyTo (targetPath, true);
		if (copyMeta) {
			FileInfo metaFile = new FileInfo (source.FullName + ".meta");
			if (metaFile.Exists) {
				metaFile.CopyTo (targetPath + ".meta", true);
			}
		}
	}
}
