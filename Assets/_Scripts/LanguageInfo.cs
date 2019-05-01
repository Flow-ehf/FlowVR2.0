using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lang", menuName = "Language Info")]
public class LanguageInfo : ScriptableObject
{
	public LanguageManager.Language language;
	public LangStr[] langStr = new LangStr[]
	{
		new LangStr("Continue")
	};

	[System.Serializable]
	public struct LangStr
	{
		public string tag;
		public string str;

		public LangStr(string tag)
		{
			this.tag = tag;
			this.str = "";
		}
	}
}
