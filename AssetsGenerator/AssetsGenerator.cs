using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute]
public class AssetsGenerator : ScriptableObject {

	public bool copyMeta;
	public string targetPath;
	public List<string> sourcePaths = new List<string> ();

}
