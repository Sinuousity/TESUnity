using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXLoader : MonoBehaviour
{
	public class SFXData
	{
		public string name;
		public AudioClip clip;
		public bool loaded = false;
		public float volume = 0f;
	}

	private static SFXLoader __instance;
	public static SFXLoader instance
	{
		get
		{
			if ( __instance == null )
			{
				GameObject go = new GameObject("SFXLoader");
				DontDestroyOnLoad(go);
				__instance = go.AddComponent<SFXLoader>();
			}
			return __instance;
		}
	}



	public static Dictionary<string , SFXData> cachedSfx = new Dictionary<string , SFXData>();

	public static SFXData LoadSFX(TESUnity.ESM.SOUNRecord record)
	{
		if ( record != null )
		{
			if ( record.FNAM != null )
			{
				SFXData data = LoadSFX(record.FNAM.value , "");
				if ( record.DATA != null ) data.volume = record.DATA.volume;
				return data;
			}
		}
		return null;
	}

	public static SFXData LoadSFX ( string fullPath )
	{
		string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
		string fileNameFull = System.IO.Path.GetFileName(fullPath);
		string localPath = fullPath.Replace(TESUnity.TESUnity.Settings.engine.dataFilesPath , "");
		localPath = fullPath.Remove(0 , 6);//remove "Sound/"
		localPath = fullPath.Replace(fileNameFull , "");//remove file name + extension
		return LoadSFX( fileName , localPath );
	}

	public static SFXData LoadSFX( string fileName , string localPath )
	{
		//check for cached sounds first
		if ( cachedSfx.ContainsKey(fileName) ) return cachedSfx[ fileName ];

		//otherwise load the new one and cache it
		SFXData sfx = new SFXData();
		sfx.name = fileName;
		var fullPath = PathCombine(TESUnity.TESUnity.Settings.engine.dataFilesPath , "Sound" , localPath , fileName );
		fullPath = fullPath.Replace("\\" , "/");//replace any backslashes with forward ones
		if ( fullPath.StartsWith("/") ) fullPath = fullPath.Remove(0 , 1); //remove any leading slashes
		var loader = new WWW("file:///" + fullPath);
		instance.StartCoroutine(instance.c_LoadSFX( loader , sfx));
		return sfx;
	}

	static string PathCombine ( params string[] strings )
	{
		if ( strings.Length < 1 )
			return "";

		if ( strings.Length < 2 )
			return strings[0];

		int i = 0;
		string res = strings[ i ];
		while ( i < strings.Length )
		{
			res = System.IO.Path.Combine(res, strings[ i++ ]);
		}
		return res;
	}

	public IEnumerator c_LoadSFX ( WWW loader , SFXData sfx )
	{
		cachedSfx.Add(sfx.name , sfx);
		while ( !loader.isDone ) yield return loader;
		if ( !string.IsNullOrEmpty(loader.error) ) Debug.Log(loader.error);
		sfx.clip = loader.GetAudioClip(false , false , AudioType.WAV);
		sfx.loaded = true;
		yield return new WaitForSeconds(1f);
		loader.Dispose();
	}

	public static void PlaySFX(TESUnity.ESM.SOUNRecord record , Vector3 position)
	{
		if ( record != null )
		{
			if ( record.FNAM != null )
			{
				PlaySFX(record.FNAM.value , "" , position);
			}
		}
	}

	public static void PlaySFX (string fileName , string localPath , Vector3 position)
	{
		var sfx = LoadSFX(fileName , localPath );
		if ( sfx.loaded )
			AudioSource.PlayClipAtPoint(sfx.clip , position);
		else
			instance.StartCoroutine(instance.c_WaitToPlaySfx(sfx , position));
	}

	public IEnumerator c_WaitToPlaySfx (SFXData sfx , Vector3 position)
	{
		float t = 0f;
		while ( !sfx.loaded && t < 3f)
		{
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if ( sfx.loaded )
			AudioSource.PlayClipAtPoint(sfx.clip , position);
		else
			cachedSfx.Remove(sfx.name);
	}
}