using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public static class GamePaths
	{
		public static string PathCombine(params string[] strings)
		{
			if ( strings.Length < 1 )
				return "";

			if ( strings.Length < 2 )
				return strings[ 0 ];

			int i = 0;
			string res = strings[ i ];
			while ( i < strings.Length )
			{
				res = System.IO.Path.Combine(res , strings[ i++ ]);
			}
			return res;
		}

		public static string FullToLocalPath(string fullPath)
		{
			string fileNameFull = System.IO.Path.GetFileName(fullPath);
			string localPath = fullPath.Replace(TESUnity.Settings.engine.dataFilesPath , "");
			return localPath.Replace(fileNameFull , "");//remove file name + extension
		}

		public static string LocalToFullPath(string localPath)
		{
			return PathCombine(TESUnity.Settings.engine.dataFilesPath , localPath);
		}

		public static string SoundsFolder { get { return PathCombine(TESUnity.Settings.engine.dataFilesPath , "Sound"); } }
	}
}