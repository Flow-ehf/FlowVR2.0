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
	//SerializedProperty guidanceOff;
	//SerializedProperty normal;

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
		//guidanceOff = serializedObject.FindProperty(nameof(guidanceOff));
		//normal = serializedObject.FindProperty(nameof(normal));
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
		//EditorGUILayout.PropertyField(guidanceOff);
		//EditorGUILayout.PropertyField(normal);
		EditorGUILayout.Space();
		LanguageManager.Language[] allLang = Enum.GetValues(typeof(LanguageManager.Language)) as LanguageManager.Language[];
		EditorGUILayout.LabelField("Guidance Audio Clips");
		using (new EditorGUI.IndentLevelScope())
		{
			int index = 0;
			for (int i = 0; i < MeditationQueue.availableSessionDurations.Length; i++)
			{
				for (int j = 0; j < allLang.Length; j++)
				{
					if (guidanceClips.arraySize <= index)
						guidanceClips.InsertArrayElementAtIndex(index);
					SerializedProperty elm = guidanceClips.GetArrayElementAtIndex(index);
					EditorGUILayout.PropertyField(elm, new GUIContent("Guidance " + MeditationQueue.availableSessionDurations[i] / 60 + "m " + allLang[j]));

					index++;
				}
			}
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Music Audio Clips");
		using (new EditorGUI.IndentLevelScope())
		{
			for (int i = 0; i < MeditationQueue.availableSessionDurations.Length; i++)
			{
				if (musicClips.arraySize <= i)
					musicClips.InsertArrayElementAtIndex(i);
				SerializedProperty elm = musicClips.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(elm, new GUIContent("Music " + MeditationQueue.availableSessionDurations[i] / 60 + "m"));
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
}
