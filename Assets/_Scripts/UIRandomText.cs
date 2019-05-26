using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIRandomText : MonoBehaviour
{
	[SerializeField, Multiline] string[] texts;

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
		string targetText = texts.Length > 0 ? texts[Random.Range(0, texts.Length)] : "";

		if (langCtrl != null)
			langCtrl.UpdateTag(targetText);
		else
			text.text = targetText;
	}
}
