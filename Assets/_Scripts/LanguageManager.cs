using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageManager 
{
    public enum Language
	{
		English,
		Icelandic,
	}

	public static Language CurrentLanguage => currentInfo?.language ?? Language.English;

	public static event Action<Language> LanguageChanged;

	static LanguageInfo[] allInfo;
	static LanguageInfo currentInfo;


	static LanguageManager()
	{
		Language language = (Language)PlayerPrefs.GetInt("Language", 0);
		allInfo = Resources.LoadAll<LanguageInfo>("LanguageInfo");
		currentInfo = GetLanguageInfo(language);
	}


	public static void SetLangauge(Language newLang)
	{
		if(currentInfo == null || currentInfo.language != newLang)
		{
			Debug.Log( $"Set language '{newLang}'");
			PlayerPrefs.SetInt("Language", (int)newLang);
			currentInfo = GetLanguageInfo(newLang);
			
			LanguageChanged?.Invoke(newLang);
		}
	}


	static LanguageInfo GetLanguageInfo(Language language)
	{
		for (int i = 0; i < allInfo.Length; i++)
		{
			if (allInfo[i].language == language)
				return allInfo[i];
		}
		Debug.LogError($"Info file for '{language}' does not exist!");
		return null;
	}


	public static bool HasSetLanguage()
	{
		return PlayerPrefs.HasKey("Language");
	}


	public static string GetStr(string tag)
	{
		tag = tag.ToLower();
		for (int i = 0; i < currentInfo.langStr.Length; i++)
		{
			if (currentInfo.langStr[i].tag == tag)
				return currentInfo.langStr[i].str;
		}
		Debug.LogWarning($"Tag '{tag}' not found for language '{currentInfo.language}'");
		return "";
	}
}
