using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public class SoundSource : MonoBehaviour
	{
		public bool looping = true;
		public SFXLoader.SFXData soundData;
		public AudioSource src;

		public IEnumerator c_Init()
		{
			while ( soundData == null )
				yield return new WaitForEndOfFrame();

			src = gameObject.AddComponent<AudioSource>();
			src.loop = looping;

			while ( !soundData.loaded || soundData.clip == null )
				yield return new WaitForEndOfFrame();

			src.clip = soundData.clip;
			src.volume = soundData.volume / 255f;
			src.volume *= src.volume;
			src.dopplerLevel = 0f;
			src.SetCustomCurve(AudioSourceCurveType.CustomRolloff , TESUnity.SFXRolloff);
			src.rolloffMode = AudioRolloffMode.Custom;
			src.maxDistance = 15f;
			src.spatialBlend = 1f;
			src.Play();
		}
	}
}