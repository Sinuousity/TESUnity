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

	/// <summary>
	/// Loads a sound effect wav file from Morrowind's Sound directory.
	/// Most likely a temporary implementation of SFX loading.
	/// </summary>
	/// <param name="fileName"> Name of the wav file. Does not include extension </param>
	/// <param name="path"> Path within the Morrowind/Data Files/Sound/ folder. Always ends with a forward slash. </param>
	/// <returns></returns>
	public static SFXData LoadSfx( string fileName , string localPath )
	{
		//check for cached sounds first
		if ( cachedSfx.ContainsKey(fileName) ) return cachedSfx[ fileName ];

		//otherwise load the new one and cache it
		SFXData sfx = new SFXData();
		sfx.name = fileName;
		var fullPath = TESUnity.TESUnity.SettingsFile.engine.dataFilesPath + "/Sound/" + localPath + "/" + fileName + ".wav";
		fullPath = fullPath.Replace("\\" , "/");
		if ( fullPath.StartsWith("/") ) fullPath = fullPath.Remove(0 , 1);
		var loader = new WWW("file:///" + fullPath);
		instance.StartCoroutine(instance.c_LoadSFX( loader , sfx));
		return sfx;
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

	public static void PlaySFX (string fileName , string localPath,Vector3 position)
	{
		var sfx = LoadSfx(fileName , localPath );
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