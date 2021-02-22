using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Info")]
public class LevelInfo : ScriptableObject
{
	public string displayName;
	public string description;

	private static Dictionary<string, LevelInfo> levels;

	public static LevelInfo Get(string level)
	{
		if(levels == null)
		{
			levels = new Dictionary<string, LevelInfo>();
			foreach (var lvl in Resources.LoadAll<LevelInfo>("Levels"))
			{
				levels.Add(lvl.name, lvl);
			}
		}

		if (levels.TryGetValue(level, out var l))
			return l;
		else
			return null;
	}
}
