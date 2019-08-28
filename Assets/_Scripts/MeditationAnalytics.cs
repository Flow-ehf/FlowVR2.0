using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeditationAnalytics
{
	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		Application.quitting += OnQuit;
	}


	static void OnQuit()
	{
		currentSession.totalPlayTime = Time.unscaledTime;
	}

	static PlaySessionData currentSession;
	static List<MeditationSessionData> meditationSessions = new List<MeditationSessionData>();

	public static MeditationSessionData CurrentMeditationSession => meditationSessions.Count > 0 ? meditationSessions[Random.Range(0, meditationSessions.Count)] : null;

	public static void AddMeditationSession(MeditationSessionData data)
	{
		meditationSessions.Add(data ?? new MeditationSessionData());
	}

    public struct PlaySessionData
	{
		public float totalPlayTime;
	}

	public class MeditationSessionData
	{
		public MeditationSessionDataPoint<string> level;
		public MeditationSessionDataPoint<float> selectedDuration;
		public MeditationSessionDataPoint<float> playTime;
		public MeditationSessionDataPoint<bool> quitEarly;
		public MeditationSessionDataPoint<bool> initialMusicEnabled;
		public MeditationSessionDataPoint<bool> initialGuidanceEnabled;
		public MeditationSessionDataPoint<string> hmdId;
	}


	public struct MeditationSessionDataPoint<T>
	{
		public T data;
		public float playtime;
		public System.DateTime timeStamp;
		public string tag;

		public MeditationSessionDataPoint(T data, string tag = "")
		{
			this.data = data;
			this.tag = tag;

			this.timeStamp = System.DateTime.UtcNow;
			this.playtime = Time.unscaledTime;
		}


		public static implicit operator MeditationSessionDataPoint<T>(T data)
		{
			return new MeditationSessionDataPoint<T>(data);
		}
	}
}

