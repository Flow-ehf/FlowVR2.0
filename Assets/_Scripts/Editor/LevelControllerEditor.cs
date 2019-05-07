using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
	SerializedProperty timerText;
	SerializedProperty pauseButton;
	SerializedProperty pausePanel;
	SerializedProperty resumeButton;// Start is called before the first frame update
    SerializedProperty musicObject;
	SerializedProperty guidanceObjects;

	private void OnEnable()
	{
		timerText = serializedObject.FindProperty(nameof(timerText));
		pauseButton = serializedObject.FindProperty(nameof(pauseButton));
		pausePanel = serializedObject.FindProperty(nameof(pausePanel));
		resumeButton = serializedObject.FindProperty(nameof(resumeButton));
		musicObject = serializedObject.FindProperty(nameof(musicObject));
		guidanceObjects = serializedObject.FindProperty(nameof(guidanceObjects));
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(timerText);
		EditorGUILayout.PropertyField(pauseButton);
		EditorGUILayout.PropertyField(pausePanel);
		EditorGUILayout.PropertyField(resumeButton);
		EditorGUILayout.PropertyField(musicObject);

		LanguageManager.Language[] allLang = Enum.GetValues(typeof(LanguageManager.Language)) as LanguageManager.Language[];
		EditorGUILayout.LabelField("Guidance objects");
		using (new EditorGUI.IndentLevelScope())
		{
			for (int i = 0; i < allLang.Length; i++)
			{
				if (guidanceObjects.arraySize <= i)
					guidanceObjects.InsertArrayElementAtIndex(i);
				SerializedProperty arrElm = guidanceObjects.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(arrElm, new GUIContent(allLang[i].ToString()));
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
