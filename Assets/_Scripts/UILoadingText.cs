using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UILoadingText : MonoBehaviour
{
	[SerializeField] LevelText[] texts;
	[SerializeField] string fallbackText;

	Text text;
	LanguageText langCtrl;

    // Start is called before the first frame update
    void Awake()
    {
		text = GetComponent<Text>();
		langCtrl = GetComponent<LanguageText>();
    }


	void Start()
	{
		UpdateText();
	}


	public void UpdateText()
	{
		//Find quote for this level
		string targetText = GetText();

		if (langCtrl != null)
			langCtrl.UpdateText(targetText);
		else
			text.text = targetText;
	}


	string GetText()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			for (int j = 0; j < texts[i].levels.Length; j++)
			{
				if (texts[i].levels[j] == LevelLoader.LevelBeingLoaded)
				{
					return texts[i].randomTexts.RandomElement();
				}
			}
		}
		return fallbackText;
	}


	[System.Serializable]
	class LevelText
	{
		public string[] levels;
		[Multiline]
		public string[] randomTexts;
	}
}
