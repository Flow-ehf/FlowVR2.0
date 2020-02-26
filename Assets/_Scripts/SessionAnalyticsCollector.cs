using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HMDUtils
{
	public static string DeviceID
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return "PC_" + SystemInfo.deviceUniqueIdentifier; //Unique per pc
#elif UNITY_ANDROID
			AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
			return jo.GetStatic<string>("SERIAL");
#else
			return "N/A";
#endif
		}
	}
}

public class SessionAnalyticsCollector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string lvl = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

		string deviceId = HMDUtils.DeviceID;

		MeditationAnalytics.MeditationSessionData data = new MeditationAnalytics.MeditationSessionData()
		{
			level = lvl,
			selectedDuration = SessionSettings.Duration,
			initialMusicEnabled = SessionSettings.PlayMusic,
			initialGuidanceEnabled = SessionSettings.PlayGuidance,
			hmdId = deviceId,
		};
		MeditationAnalytics.AddMeditationSession(data);

		LevelController.Instance.QuitMeditation += QuitMeditiation;
    }


	public void QuitMeditiation()
	{
		var session = MeditationAnalytics.CurrentMeditationSession;
		session.playTime = LevelController.Instance.PlayTime;
		session.quitEarly = LevelController.Instance.TimeLeft > 0;
		MeditationAnalytics.SendCurrentSession();
	}


	void OnDestroy()
	{
		if(LevelController.Instance != null)
			LevelController.Instance.QuitMeditation -= QuitMeditiation;
	}
}
