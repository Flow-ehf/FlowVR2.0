using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	[SerializeField] Text timerText;
	[SerializeField] Button pauseButton;
	[SerializeField] Button resumeButton;
	[SerializeField] UIPanel pausePanel;
	[SerializeField] GameObject[] guidanceObjects;
	[SerializeField] GameObject musicObject;

	float timeLeft;
	bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
		//Activate guidance for target language
		if(guidanceObjects != null)
		{
			for (int i = 0; i < guidanceObjects.Length; i++)
			{
				if(guidanceObjects[i] != null )
					guidanceObjects[i].SetActive(SessionSettings.PlayGuidance && (int)LanguageManager.CurrentLanguage == i);
			}
		}
		musicObject?.SetActive(SessionSettings.PlayMusic);

		pauseButton?.onClick.AddListener(() => Pause(true));
		resumeButton?.onClick.AddListener(() => Pause(false));

		pausePanel?.SetActiveImmediately(false);

		timeLeft = SessionSettings.Duration;

		StartCoroutine(WaitDuration());
    }


	IEnumerator WaitDuration()
	{
		while(timeLeft >= 0)
		{
			yield return new WaitForSeconds(1);
			if(timerText != null)
			{
				if (timeLeft >= 60)
					timerText.text = Mathf.Floor(timeLeft / 60) + "m " + timeLeft % 60 + "s";
				else
					timerText.text = timeLeft + "s";
			}
			if (isPaused)
				yield return new WaitWhile(() => isPaused);
		}
		ReturnToMenu();
	}


	void Update()
	{
		if(!isPaused && OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
		{
			Pause(!isPaused);
			
		}
	}


	void Pause(bool pause)
	{
		isPaused = pause;

		pausePanel.SetActive(isPaused);

	}


	public void ReturnToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
