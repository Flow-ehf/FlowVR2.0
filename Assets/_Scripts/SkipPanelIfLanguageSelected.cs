using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class SkipPanelIfLanguageSelected : MonoBehaviour
{
	[SerializeField] UIPanel[] targetPanel;

	UIPanel thisPanel;

	void Awake()
	{
		thisPanel = GetComponent<UIPanel>();
	}


    // Start is called before the first frame update
    void Start()
    {
        if(LanguageManager.HasSetLanguage() && targetPanel.Length > 0)
		{
			for (int i = 0; i < targetPanel.Length; i++)
			{
				targetPanel[i].SetActiveImmediately(true);
			}
		}
    }
}
