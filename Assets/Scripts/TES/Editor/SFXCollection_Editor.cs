using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SFXCollection))]
public class SFXCollection_Editor : Editor
{
	SFXCollection collection;
	Vector2 scrollPos = Vector2.zero;
	SerializedProperty localPath;
	List<string> fileList;

	void OnEnable ()
	{
		collection = serializedObject.targetObject as SFXCollection;
		localPath = serializedObject.FindProperty("localPath");
		fileList = collection.fileNames;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(localPath);

		if ( TESUnity.TESUnity.Settings == null )
			GUILayout.Label("NO DATA PATH - NO SETTINGS FILE FOUND");
		else
		{
			if ( GUILayout.Button("Add Files At Path") ) ( serializedObject.targetObject as SFXCollection ).FindFiles();
			if ( GUILayout.Button("Clear Files") ) ( serializedObject.targetObject as SFXCollection ).fileNames.Clear();
			if ( GUILayout.Button("Remove Highlighted Files") ) ( serializedObject.targetObject as SFXCollection ).RemoveSelected();
		}
		if ( fileList.Count > 0 ) EditorGUIHelpers.FancyListArea(ref scrollPos , fileList.Count + "Files:" , fileList , ref collection.highlighted , 20);

		serializedObject.ApplyModifiedProperties();
	}
}