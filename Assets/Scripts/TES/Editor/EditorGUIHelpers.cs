using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class EditorGUIHelpers
{
	public static string Ellipsis
	{
		get
		{
			return new string('.' , ( int )Mathf.Repeat(( int )( EditorApplication.timeSinceStartup * 2f ) , 4));
		}
	}

	public static void FancyListArea(ref Vector2 scrollPos , string label , List<string> list , ref List<int> selected , int maxVertical = 20)
	{
		var padlessBox = new GUIStyle(GUI.skin.box);
		padlessBox.stretchHeight = true;
		padlessBox.padding = new RectOffset(0 , 0 , 0 , 0);

		var boxed = new GUIStyle(GUI.skin.box);
		boxed.normal.background = GUI.skin.box.normal.background;
		boxed.stretchHeight = true;
		boxed.stretchWidth = true;

		var colored = new GUIStyle(GUI.skin.label);
		colored.normal.background = EditorGUIUtility.whiteTexture;
		colored.stretchHeight = true;
		colored.stretchWidth = true;

		if ( !string.IsNullOrEmpty(label) ) EditorGUILayout.PrefixLabel(label);
		GUILayout.BeginVertical(padlessBox , GUILayout.MinHeight(maxVertical * 18f + 13f));

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical(boxed);

		int y = 0;
		int j = 0;
		for ( int i = 0 ; i < list.Count ; i++ )
		{
			GUI.backgroundColor = ( ( i + j ) % 2 == 0 ) ? Color.white : Color.grey * 1.7f;
			if ( selected.Contains(i) )
			{
				GUI.backgroundColor = new Color(0.3f , 0.3f , 0.3f , 1f);
				colored.normal.textColor = Color.white;
			}
			if ( GUILayout.Button(list[ i ] , colored) )
			{
				if ( selected.Contains(i) ) selected.Remove(i); else selected.Add(i);
			}
			GUI.backgroundColor = Color.white;
			colored.normal.textColor = Color.black;

			y++;
			if ( y >= maxVertical && i + 1 < list.Count )
			{
				y = 0;
				j++;
				GUILayout.EndVertical();
				GUILayout.BeginVertical(boxed);
			}
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}
}