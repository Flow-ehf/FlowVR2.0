using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageUISelector : MonoBehaviour
{
	[SerializeField] LanguageManager.Language targetLanguage;
	[SerializeField] bool allowSelectCurrentLanguage = false;
	[SerializeField] Button button;


    // Start is called before the first frame update
    void Awake()
    {
		if (button != null)
		{
			button.onClick.AddListener(() => LanguageManager.SetLangauge(targetLanguage));
			if(!allowSelectCurrentLanguage)
				button.interactable = LanguageManager.CurrentLanguage != targetLanguage;
			LanguageManager.LanguageChanged += LanguageChanged;
		}
    }


	void LanguageChanged(LanguageManager.Language lang)
	{
		if(!allowSelectCurrentLanguage)
			button.interactable = LanguageManager.CurrentLanguage != targetLanguage;
	}


	void OnDestroy()
	{
		LanguageManager.LanguageChanged -= LanguageChanged;
	}


	void Reset()
	{
		button = GetComponent<Button>();
	}
}
