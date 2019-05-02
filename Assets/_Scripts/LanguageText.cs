using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageText : MonoBehaviour
{
	Text text;

	string langTag;

	void Awake()
	{
		text = GetComponent<Text>();
		LanguageManager.LanguageChanged += LanguageChanged;
	}


	void Start()
	{
		langTag = GetTag();

		if (langTag != null)
			text.text = LanguageManager.GetStr(langTag); 
	}


	void LanguageChanged(LanguageManager.Language lang)
	{ 
		if (langTag != null)
			text.text = LanguageManager.GetStr(langTag);
	}


	string GetTag()
	{
		string t = text.text;
		int tagStart = t.IndexOf('<');
		if(tagStart > -1)
		{
			int tagEnd = t.IndexOf('>');
			if(tagEnd > tagStart)
			{
				return t.Substring(tagStart + 1, tagEnd - tagStart - 1);
			}
		}
		return null;
	}


	void OnDestroy()
	{
		LanguageManager.LanguageChanged -= LanguageChanged;
	}
}
