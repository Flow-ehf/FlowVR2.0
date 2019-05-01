using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageText : MonoBehaviour
{
	Text text;

	string tag;

	void Awake()
	{
		text = GetComponent<Text>();
		LanguageManager.LanguageChanged += LanguageChanged;
	}


	void Start()
	{
		tag = GetTag();
		if (tag != null)
			text.text = LanguageManager.GetStr(tag); 
	}


	void LanguageChanged(LanguageManager.Language lang)
	{ 
		if (tag != null)
			text.text = LanguageManager.GetStr(tag);
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
