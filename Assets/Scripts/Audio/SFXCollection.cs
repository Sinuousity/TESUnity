using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TES Unity SFX Collection", menuName = "TES Unity/New SFX Collection", order = 1)]
public class SFXCollection : ScriptableObject
{
	public string localPath = "fx";
	public List<string> fileNames = new List<string>();
	public List<int> highlighted = new List<int>();
	/// <summary>
	/// NEEDS CHANGE TO A GLOBAL REFERNCE TO SUPPORT DIFFERING INSTALL DIRECTORIES
	/// </summary>
	public string directory
	{
		get
		{
			return TESUnity.TESUnity.Settings.engine.dataFilesPath + "/Sound/" + localPath + "/";
		}
	}
	public bool HasFile(string fileName)
	{
		return fileNames.Contains(fileName);
	}
	public string GetFilePath (string fileName )
	{
		return directory + fileName + ".wav";
	}

	public void FindFiles ()
	{
		if ( Directory.Exists(directory) )
		{
			var filesPaths = Directory.GetFiles(directory);
			foreach (string fp in filesPaths)
			{
				if (fp.EndsWith(".wav"))
				{
					var split = fp.Split('/');
					var fileName = split[ split.Length - 1 ];
					fileName = fileName.Remove(fileName.Length - 4);
					if ( !fileNames.Contains(fileName) )
						fileNames.Add(fileName);
				}
			}

		}
	}

	public void RemoveSelected ()
	{
		var willremove = new List<string>();

		foreach ( int i in highlighted )
			willremove.Add(fileNames[ i ]);
		foreach ( string s in willremove )
			fileNames.Remove(s);

		highlighted.Clear();
	}

	public string RandomFile ()
	{
		return fileNames[ Random.Range(0 , fileNames.Count) ];
	}
}