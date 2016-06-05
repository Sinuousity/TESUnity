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

	public static void FancyListScrollView(ref Vector2 scrollPos , string label , List<string> list , ref List<int> selected , int maxVertical = 20)
	{
		GUIStyle boxed = new GUIStyle(GUI.skin.scrollView);
		boxed.normal.background = GUI.skin.box.normal.background;
		boxed.border = GUI.skin.box.border;
		boxed.padding = GUI.skin.box.padding;
		boxed.overflow = new RectOffset(1,0,0,0);
		boxed.clipping = TextClipping.Overflow;

		GUIStyle colored = new GUIStyle(GUI.skin.label);
		colored.normal.background = EditorGUIUtility.whiteTexture;
		boxed.padding = new RectOffset(0 , 0 , 0 , 0);
		boxed.margin = new RectOffset(0 , 0 , 0 , 0);
		colored.overflow = new RectOffset(3 , 3 , 1 , 1);

		EditorGUILayout.PrefixLabel(label);
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos , boxed , GUILayout.MaxHeight(maxVertical * 18f + 17f));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical(boxed);

		int y = 0;
		for ( int i = 0; i < list.Count ; i++ )
		{
			GUI.backgroundColor = ( i % 2 == 0 ) ? Color.white : Color.grey* 1.7f;
			if ( selected.Contains(i) )
			{
				GUI.backgroundColor = new Color(0.2f , 0.2f , 0.2f , 1f);
				colored.normal.textColor = Color.white;
			}
			if ( GUILayout.Button(list[ i ] , colored , GUILayout.MaxWidth(130f)) )
			{
				if ( selected.Contains(i) ) selected.Remove(i); else selected.Add(i);
			}
			GUI.backgroundColor = Color.white;
			colored.normal.textColor = Color.black;

			y++;
			if ( y >= maxVertical )
			{
				y = 0;
				GUILayout.EndVertical();
				GUILayout.BeginVertical(boxed);
			}
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
}