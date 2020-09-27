using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class LoadLevelButton : MonoBehaviour
{
	[SerializeField] string level;

	Button button;

	public LanguageText text;

    // Start is called before the first frame update
    void Awake()
    {
		button = GetComponent<Button>();
		button.onClick.AddListener(LoadLevel);
		text = GetComponentInChildren<LanguageText>();
    }


	public void SetTargetLevel(string level)
	{
		this.level = level;
	}

	
	void LoadLevel()
	{
		if (level != "")
		{
			LevelLoader.LoadLevel(level);
		}
	}
}
