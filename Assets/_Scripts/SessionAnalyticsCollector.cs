using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionAnalyticsCollector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string lvl = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

		MeditationAnalytics.MeditationSessionData data = new MeditationAnalytics.MeditationSessionData()
		{
			level = lvl,
			selectedDuration = SessionSettings.Duration,
			initialMusicEnabled = SessionSettings.PlayMusic,
			initialGuidanceEnabled = SessionSettings.PlayGuidance,
		};
		MeditationAnalytics.AddMeditationSession(data);

		LevelController.Instance.QuitMeditation += QuitMeditiation;
    }


	public void QuitMeditiation()
	{
		var session = MeditationAnalytics.CurrentMeditationSession;
		session.playTime = LevelController.Instance.PlayTime;
		session.quitEarly = LevelController.Instance.TimeLeft > 0;
	}


    void OnDestroy()
	{
		if(LevelController.Instance != null)
			LevelController.Instance.QuitMeditation -= QuitMeditiation;
	}
}
