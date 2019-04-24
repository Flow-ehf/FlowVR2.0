using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Button))]
public class LoadLevelButton : MonoBehaviour
{
	[SerializeField] string level;

	Button button;

    // Start is called before the first frame update
    void Awake()
    {
		button = GetComponent<Button>();
		button.onClick.AddListener(LoadLevel);
    }


	public void SetTargetLevel(string level)
	{
		this.level = level;
	}

	
	void LoadLevel()
	{
		if (level != "")
		{
			SceneManager.LoadScene(level);
		}
	}
}
