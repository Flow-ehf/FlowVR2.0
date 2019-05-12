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
	SerializedProperty resumeButton;
	SerializedProperty muteMusicButton;
	SerializedProperty muteGuidanceButton;
	SerializedProperty guidanceClips;
	SerializedProperty musicClips;
	SerializedProperty ambianceClips;

	private void OnEnable()
	{
		timerText = serializedObject.FindProperty(nameof(timerText));
		pauseButton = serializedObject.FindProperty(nameof(pauseButton));
		pausePanel = serializedObject.FindProperty(nameof(pausePanel));
		resumeButton = serializedObject.FindProperty(nameof(resumeButton));
		muteMusicButton = serializedObject.FindProperty(nameof(muteMusicButton));
		muteGuidanceButton = serializedObject.FindProperty(nameof(muteGuidanceButton));
		guidanceClips = serializedObject.FindProperty(nameof(guidanceClips));
		musicClips = serializedObject.FindProperty(nameof(musicClips));
		ambianceClips = serializedObject.FindProperty(nameof(ambianceClips));
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(timerText);
		EditorGUILayout.PropertyField(pauseButton);
		EditorGUILayout.PropertyField(pausePanel);
		EditorGUILayout.PropertyField(resumeButton);
		EditorGUILayout.PropertyField(muteMusicButton);
		EditorGUILayout.PropertyField(muteGuidanceButton);
		EditorGUILayout.Space();
		LanguageManager.Language[] allLang = Enum.GetValues(typeof(LanguageManager.Language)) as LanguageManager.Language[];
		EditorGUILayout.LabelField("Guidance Audio Clips");
		using (new EditorGUI.IndentLevelScope())
		{
			int index = 0;
			for (int i = 0; i < SessionSettings.AvailableDurations.Length; i++)
			{
				for (int j = 0; j < allLang.Length; j++)
				{
					if (guidanceClips.arraySize <= index)
						guidanceClips.InsertArrayElementAtIndex(index);
					SerializedProperty elm = guidanceClips.GetArrayElementAtIndex(index);
					EditorGUILayout.PropertyField(elm, new GUIContent("Guidance " + SessionSettings.AvailableDurations[i] / 60 + "m " + allLang[j]));

					index++;
				}
			}
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Music Audio Clips");
		using (new EditorGUI.IndentLevelScope())
		{
			for (int i = 0; i < SessionSettings.AvailableDurations.Length; i++)
			{
				if (musicClips.arraySize <= i)
					musicClips.InsertArrayElementAtIndex(i);
				SerializedProperty elm = musicClips.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(elm, new GUIContent("Music " + SessionSettings.AvailableDurations[i] / 60 + "m"));
			}
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Ambiance Audio Clips");
		using (new EditorGUI.IndentLevelScope())
		{
			for (int i = 0; i < SessionSettings.AvailableDurations.Length * 4; i++)
			{
				if (ambianceClips.arraySize <= i)
					ambianceClips.InsertArrayElementAtIndex(i);
				SerializedProperty elm = ambianceClips.GetArrayElementAtIndex(i);
				int durIndex = i / 4;
				int ambIndex = i % 4 + 1;
				EditorGUILayout.PropertyField(elm, new GUIContent("Ambiance"+ ambIndex + " " + SessionSettings.AvailableDurations[durIndex] / 60 + "m"));
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
