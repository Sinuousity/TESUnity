using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public enum LightAnimMode { None, Flicker, FlickerSlow, Pulse, PulseSlow, Fire }//None is included as courtesy
	public class LightAnim : MonoBehaviour
	{
		public LightAnimMode mode = LightAnimMode.None;
		new Light light;
		float baseIntensity = 1f;

		void Start ()
		{
			//Debug.Log("Animated Light Created: " + mode);
			light = GetComponent<Light>();
			baseIntensity = light.intensity;
		}

		void Update ()
		{
			var value = 1f;
			var lerpSpeed = 25f;
			switch (mode)
			{
				case LightAnimMode.None:
					break;
				case LightAnimMode.Flicker:
					value = Mathf.Round(Random.value);
					lerpSpeed = 10f;
					break;
				case LightAnimMode.FlickerSlow:
					value = Mathf.Round(Random.value);
					lerpSpeed = 2f;
					break;
				case LightAnimMode.Pulse:
					value = Mathf.Sin(Time.time) * 0.5f + 0.5f;
					lerpSpeed = 40f;
					break;
				case LightAnimMode.PulseSlow:
					value = Mathf.Sin(Time.time * 0.5f) * 0.5f + 0.5f;
					lerpSpeed = 40f;
					break;
				case LightAnimMode.Fire:
					value = Mathf.PerlinNoise(Time.time * 0.8f , transform.position.x+transform.position.z * 7.9253618f);
					value = 1f - value;
					value = value * value * value;
					value = 1f - value;
					break;
			}
			light.intensity = Mathf.Lerp(light.intensity , Mathf.Lerp(0.2f * baseIntensity , baseIntensity , value) , Time.deltaTime * lerpSpeed);
		}
	}
}