using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public class Material_Cache_Viewer : EditorWindow
	{
		[MenuItem("TES Unity/Material Cache Viewer")]
		static void Init()
		{
			Material_Cache_Viewer w = GetWindow<Material_Cache_Viewer>();
			w.titleContent = new GUIContent("Material Cache");
			w.Show();
		}

		Vector2 scrollPos = Vector2.zero;
		List<int> selectedIndices = new List<int>();
		List<string> materialNameList = new List<string>();

		void UpdateList ()
		{
			List<string> list = new List<string>();
			foreach ( KeyValuePair<MWMaterialProps , Material> kvp in MaterialManager.existingMaterials )
			{
				if ( kvp.Value.mainTexture != null ) list.Add(kvp.Value.mainTexture.name.Normalize());
			}
			materialNameList = list;
		}

		void OnGUI()
		{
			GUIStyle coloredLabel = new GUIStyle(GUI.skin.label);
			coloredLabel.alignment = TextAnchor.MiddleCenter;
			coloredLabel.normal.background = EditorGUIUtility.whiteTexture;

			if ( EditorApplication.isPlaying )
			{
				if ( MaterialManager.changed )
				{
					UpdateList();
					MaterialManager.changed = false;
				}
				EditorGUIHelpers.FancyListArea(ref scrollPos , materialNameList.Count + " Cached Materials:" , materialNameList , ref selectedIndices , 30);
			}
			else
			{
				GUI.backgroundColor = Color.Lerp(Color.yellow , Color.red , 0.5f);
				GUILayout.Label("Waiting For Play Mode" + EditorGUIHelpers.Ellipsis , coloredLabel);
				GUI.backgroundColor = Color.white;
				selectedIndices.Clear();
			}
			Repaint();
		}
	}
}
