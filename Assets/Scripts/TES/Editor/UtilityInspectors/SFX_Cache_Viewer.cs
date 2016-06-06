using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFX_Cache_Viewer : EditorWindow
{
	[MenuItem("TES Unity/SFX Cache Viewer")]
	static void Init()
	{
		SFX_Cache_Viewer w = GetWindow<SFX_Cache_Viewer>();
		w.titleContent = new GUIContent("SFX Cache");
		w.Show();
	}

	Vector2 scrollPos = Vector2.zero;

	void OnGUI ()
	{
		GUIStyle coloredLabel = new GUIStyle(GUI.skin.label);
		coloredLabel.alignment = TextAnchor.MiddleCenter;
		coloredLabel.normal.background = EditorGUIUtility.whiteTexture;

		if ( EditorApplication.isPlaying )
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			foreach ( KeyValuePair<string , SFXLoader.SFXData> kvp in SFXLoader.cachedSfx )
			{
				GUILayout.BeginHorizontal();

				if ( kvp.Value != null )
				{
					if ( GUILayout.Button(kvp.Key) )
						Selection.activeObject = kvp.Value.clip;
					GUI.backgroundColor = Color.Lerp(( kvp.Value.loaded ) ? Color.green : Color.red , Color.white , 0.5f);
					GUILayout.Label(( kvp.Value.loaded ) ? "Loaded" : "Unloaded" , coloredLabel , GUILayout.Width(60f));
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = Color.red;
					GUILayout.Label(kvp.Key , coloredLabel);
					GUI.backgroundColor = Color.white;
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();

		}
		else
		{
			GUI.backgroundColor = Color.Lerp(Color.yellow , Color.red , 0.5f);
			GUILayout.Label("Waiting For Play Mode" + EditorGUIHelpers.Ellipsis , coloredLabel);
			GUI.backgroundColor = Color.white;
		}
		Repaint();
	}
}
