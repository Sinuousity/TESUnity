using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public abstract class SFXOption { }
	public class Force2DSpace : SFXOption { } // boolean options only need to exist to be true
	public class Looping : SFXOption { }
	public class DontPlayOnAwake : SFXOption { }
	public class PlayOnAwake : SFXOption { }
	public class ForceLinearRolloff : SFXOption { }
	public class ForceLogarithmicRolloff : SFXOption { }
	public class Position : SFXOption { public Vector3 value; public Position(Vector3 position) { value = position; } }
	public class Volume : SFXOption { public float value; public Volume(float volume) { value = volume; } }
	public class Pitch : SFXOption { public float value; public Pitch(float pitch) { value = pitch; } }
	public class SpatialBlend : SFXOption { public float value; public SpatialBlend(float spatialBlend) { value = spatialBlend; } }
	public class Spread : SFXOption { public float value; public Spread(float spread) { value = spread; } }
	public class MaxDistance : SFXOption { public float value; public MaxDistance(float maxDistance) { value = maxDistance; } }
	public class MinDistance : SFXOption { public float value; public MinDistance(float minDistance) { value = minDistance; } }
	public class DopplerLevel : SFXOption { public float value; public DopplerLevel(float doppler) { value = doppler; } }

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
					var go = new GameObject("SFXLoader");
					DontDestroyOnLoad(go);
					__instance = go.AddComponent<SFXLoader>();
				}
				return __instance;
			}
		}

		public static Dictionary<string , SFXData> cachedSfx = new Dictionary<string , SFXData>();

		public static Vector3 GetPointAroundPlayer(float distance)
		{
			return Camera.allCameras[ 0 ].transform.position + Random.onUnitSphere * distance;
		}

		public static SFXData LoadSFX(ESM.SOUNRecord record)
		{
			if ( record != null )
			{
				if ( record.FNAM != null )
				{
					var data = LoadSFX(GamePaths.PathCombine(TESUnity.SoundsPath , record.FNAM.value));
					if ( data == null ) return null;
					if ( record.DATA != null ) data.volume = record.DATA.volume;
					return data;
				}
			}
			return null;
		}

		public static SFXData LoadSFX(string localPath)
		{
			var fullPath = GamePaths.PathCombine(TESUnity.SoundsPath , localPath);
			var fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
			//check for cached sounds first
			if ( cachedSfx.ContainsKey(fileName) ) return cachedSfx[ fileName ];

			//otherwise load the new one and cache it
			var sfx = new SFXData();
			sfx.name = fileName;

			if ( !System.IO.File.Exists(fullPath) ) return null;
			var loader = new WWW("file:///" + fullPath);
			instance.StartCoroutine(instance.c_LoadSFX(loader , sfx));
			return sfx;
		}

		public static SFXData LoadMusic(ESM.SOUNRecord record)
		{
			if ( record != null )
			{
				if ( record.FNAM != null )
				{
					var data = LoadMusic(GamePaths.PathCombine(TESUnity.MusicPath , record.FNAM.value));
					if ( data == null ) return null;
					if ( record.DATA != null ) data.volume = record.DATA.volume;
					return data;
				}
			}
			return null;
		}

		public static SFXData LoadMusic(string localPath)
		{
			var fullPath = GamePaths.PathCombine(TESUnity.MusicPath , localPath);
			var fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
			//check for cached sounds first
			if ( cachedSfx.ContainsKey(fileName) ) return cachedSfx[ fileName ];

			//otherwise load the new one and cache it
			var sfx = new SFXData();
			sfx.name = fileName;

			if ( !System.IO.File.Exists(fullPath) ) return null;
			var loader = new WWW("file:///" + fullPath);
			instance.StartCoroutine(instance.c_LoadSFX(loader , sfx));
			return sfx;
		}

		public IEnumerator c_LoadSFX(WWW loader , SFXData sfx)
		{
			cachedSfx.Add(sfx.name , sfx);
			while ( !loader.isDone )
				yield return loader;
			if ( !string.IsNullOrEmpty(loader.error) )
				Debug.Log(loader.error);
			sfx.clip = loader.GetAudioClip(false , false , AudioType.WAV);
			sfx.loaded = true;
			yield return new WaitForSeconds(1f);
			loader.Dispose();
		}

		//These play functions contain duplicate code. TODO: consolidate
		public static void PlaySFX(ESM.SOUNRecord record , params SFXOption[] options )
		{
			if ( record != null )
			{
				var sfx = LoadSFX(record);
				if ( sfx == null ) return;
				if ( sfx.loaded )
					InstantiateSFX(sfx , options);
				else
					instance.StartCoroutine(instance.c_WaitToPlaySfx(sfx , options));
			}
		}

		public static void PlaySFX(string fileName , params SFXOption[] options)
		{
			var sfx = LoadSFX(fileName);
			if ( sfx == null ) return;
			if ( sfx.loaded )
				InstantiateSFX(sfx , options);
			else
				instance.StartCoroutine(instance.c_WaitToPlaySfx(sfx , options));
		}

		public IEnumerator c_WaitToPlaySfx(SFXData sfx , params SFXOption[] options)
		{
			var t = 0f;
			while ( !sfx.loaded && t < 6f )
			{
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			if ( sfx.loaded )
				InstantiateSFX(sfx,options);
			else
				cachedSfx.Remove(sfx.name); //remove from cache if the sound failed to load after 3 seconds
		}


		static void PlayClipAtPoint(SFXData sfx , Vector3 position , params SFXOption[] options)
		{
			var temp = options.ToList();
			temp.Add(new Position(position));
			InstantiateSFX(sfx , temp.ToArray());
		}

		static void InstantiateSFX ( SFXData sfx , params SFXOption[] options )
		{
			var go = new GameObject("SFX_" + sfx.name);
			sfx.clip.name = sfx.name;
			var src = go.AddComponent<AudioSource>();
			src.clip = sfx.clip;
			src.volume = sfx.volume;
			src.rolloffMode = AudioRolloffMode.Custom;
			src.SetCustomCurve(AudioSourceCurveType.CustomRolloff , TESUnity.Settings.audio.sfxRolloff);
			src.spatialBlend = 1f;
			src.maxDistance = 20f;

			foreach ( SFXOption option in options )
			{
				if ( option is Position ) go.transform.position = ( option as Position ).value;
				else if ( option is SpatialBlend ) src.spatialBlend = ( option as SpatialBlend ).value;
				else if ( option is Looping ) src.loop = true;
				else if ( option is PlayOnAwake ) src.playOnAwake = true;
				else if ( option is DontPlayOnAwake ) src.playOnAwake = false;
				else if ( option is Volume ) src.volume = ( option as Volume ).value;
				else if ( option is Pitch ) src.pitch = ( option as Pitch ).value;
				else if ( option is Spread ) src.spread = ( option as Spread ).value;
				else if ( option is MinDistance ) src.minDistance = ( option as MinDistance ).value;
				else if ( option is MaxDistance ) src.maxDistance = ( option as MaxDistance ).value;
				else if ( option is DopplerLevel ) src.dopplerLevel = ( option as DopplerLevel ).value;
				else if ( option is ForceLogarithmicRolloff ) src.rolloffMode = AudioRolloffMode.Logarithmic;
				else if ( option is ForceLinearRolloff ) src.rolloffMode = AudioRolloffMode.Linear;
				else if ( option is Force2DSpace ) src.spatialBlend = 0;
			}
			if ( src.playOnAwake )
			{
				src.Play();
				Object.Destroy(go , src.pitch * src.clip.length);
			}
		}
	}
}