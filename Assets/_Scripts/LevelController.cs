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
	[SerializeField] GameObject guidanceObject;
	[SerializeField] GameObject musicObject;

	float timeLeft;
	bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
		guidanceObject?.SetActive(SessionSettings.PlayGuidance);
		musicObject?.SetActive(SessionSettings.PlayMusic);

		pauseButton?.onClick.AddListener(() => Pause(true));
		resumeButton?.onClick.AddListener(() => Pause(false));

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


	void Pause(bool pause)
	{
		isPaused = pause;
	}


	void ReturnToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
