using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	/// <summary>
	/// A ScriptableObject for instances that will hold local environment settings.
	/// 
	/// Implemented to solve the issue of different developers having different data files install directories.
	/// Instances created using this object are meant to be per-developer and should not be pushed to the repository.
	/// 
	/// This file itself should be pushed to the repository.
	/// 
	/// It is important that all variables in this file are assigned default values.
	/// </summary>
	[CreateAssetMenu(fileName = "TES Unity Settings", menuName = "TES Unity/Local Settings", order = 1)]
	public class LocalSettingsObject : ScriptableObject
	{
		public static string dataPathOverride = "";

		[System.Serializable]
		public class Engine
		{
			public string dataFilesPath = "";//Default path should be blank so that the path selection UI is shown.
			public bool useKinematicRigidbodies = true;
			public bool showObjectNames = true;
			public bool useSphereCast = true;
		}
		public Engine engine = new Engine();

		[System.Serializable]
		public class Graphics
		{
			public RenderingPath preferredRenderMode = RenderingPath.Forward;
			public bool exteriorCellLights = false;
			public float ambientIntensity = 1.5f;
			public bool sunShadows = true;
			public bool lightShadows = false;
			public bool animatedLights = false;
		}
		public Graphics graphics = new Graphics();

		[System.Serializable]
		public class Audio
		{
			public bool enableMusic = true;
		}
		public Audio audio = new Audio();
	}
}