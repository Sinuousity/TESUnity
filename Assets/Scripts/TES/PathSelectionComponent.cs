﻿using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity
{
	public class PathSelectionComponent : MonoBehaviour
	{
		private string defaultMWDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

		private void Start()
		{
			TESUnity.TryFindSettings();
			string existingPath = TESUnity.MWDataPath;
			if (!string.IsNullOrEmpty(existingPath) && Directory.Exists(existingPath))
			{
				TESUnity.instance.enabled = true;
				Destroy(this);
			}

			camera = GameObjectUtils.CreateMainCamera(Vector3.zero, Quaternion.identity);
			eventSystem = GUIUtils.CreateEventSystem();
			canvas = GUIUtils.CreateCanvas();

			inputField = GUIUtils.CreateInputField(defaultMWDataPath, Vector3.zero, new Vector2(620, 30), canvas);

			var button = GUIUtils.CreateTextButton("Load World", canvas);
			button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
			button.GetComponent<Button>().onClick.AddListener(LoadWorld);
		}
		private void OnDestroy()
		{
			Destroy(canvas);
			Destroy(eventSystem);
			Destroy(camera);
		}
		private void LoadWorld()
		{
			var MWDataPath = inputField.GetComponent<InputField>().text;


			if(Directory.Exists(MWDataPath))
			{
				LocalSettingsObject.dataPathOverride = MWDataPath;
				var TESUnityComponent = GetComponent<TESUnity>();
				TESUnityComponent.dataPath = MWDataPath;

				TESUnityComponent.enabled = true;
				Destroy(this);
			}
			else
			{
				Debug.Log("Invalid path.");
			}
		}

		private new GameObject camera;
		private GameObject eventSystem, canvas, inputField;
	}
}