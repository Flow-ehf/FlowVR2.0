using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionAnalyticsCollector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string lvl = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

#if UNITY_EDITOR || UNITY_STANDALONE
		string deviceId = "PC_" + SystemInfo.deviceUniqueIdentifier; //Unique per pc
#elif UNITY_ANDROID
		AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
		string hmdId = jo.GetStatic<string>("SERIAL");
#endif

		MeditationAnalytics.MeditationSessionData data = new MeditationAnalytics.MeditationSessionData()
		{
			level = lvl,
			selectedDuration = SessionSettings.Duration,
			initialMusicEnabled = SessionSettings.PlayMusic,
			initialGuidanceEnabled = SessionSettings.PlayGuidance,
			//hmdId = deviceId,
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
