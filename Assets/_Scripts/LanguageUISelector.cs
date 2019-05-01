using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageUISelector : MonoBehaviour
{
	[SerializeField] LanguageManager.Language targetLanguage;
	[SerializeField] Button button;


    // Start is called before the first frame update
    void Awake()
    {
		if (button != null)
		{
			button.onClick.AddListener(() => LanguageManager.SetLangauge(targetLanguage));
		}
    }


	void Reset()
	{
		button = GetComponent<Button>();
	}
}
