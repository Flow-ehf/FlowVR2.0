using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LoadLevelButton))]
public class LoadLevelButtonEditor : Editor
{
	SerializedProperty level;
	int index;

	private void OnEnable()
	{
		level = serializedObject.FindProperty(nameof(level));
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		string[] levels = EditorBuildSettings.scenes.Select((s) => Path.GetFileNameWithoutExtension(s.path)).ToArray();
		index = levels.IndexOf(level.stringValue);
		int newIndex = EditorGUILayout.Popup(index, levels);
		if(newIndex != index)
		{
			level.stringValue = levels[newIndex];
			index = newIndex;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
