using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TES Unity SFX Collection", menuName = "TES Unity/New SFX Collection", order = 1)]
public class SFXCollection : ScriptableObject
{
	public string localPath = "fx";
	/// <summary>
	/// NEEDS CHANGE TO A GLOBAL REFERNCE TO SUPPORT DIFFERING INSTALL DIRECTORIES
	/// </summary>
	public string directory
	{
		get
		{
			if ( TESUnity.TESUnity.FoundSettingsFile )
				return TESUnity.TESUnity.SettingsFile.engine.dataFilesPath + "/Sound/" + localPath + "/";
			return "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files/Sound/" + localPath + "/";
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
	[ContextMenu("Find All Files At Directory")]
	public void FindFiles ()
	{
		if ( Directory.Exists(directory) )
		{
			string[] filesPaths = Directory.GetFiles(directory);
			foreach (string fp in filesPaths)
			{
				if (fp.EndsWith(".wav"))
				{
					string[] split = fp.Split('/');
					string fileName = split[ split.Length - 1 ];
					fileName = fileName.Remove(fileName.Length - 4);
					if ( !fileNames.Contains(fileName) )
						fileNames.Add(fileName);
				}
			}

		}
	}

	public string RandomFile ()
	{
		return fileNames[ Random.Range(0 , fileNames.Count) ];
	}

	public List<string> fileNames = new List<string>();
}