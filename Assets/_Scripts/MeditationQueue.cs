using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeditationQueue
{
	public static readonly int[] availableSessionDurations = new int[]
{
		240,
		480,
	};

	public static string DurationFormatted(int index)
	{
		int dur = (int)availableSessionDurations[index];
		int min = dur / 60;
		int sec = dur % 60;
		if (sec == 0)
			return min + " min";
		else
			return min + ":" + sec + " min";
	}

	public struct Session
	{
		public LevelInfo level;
		public bool playMusic;
		public bool playGuidance;
		public int durationIndex;

		public float Duration => availableSessionDurations[durationIndex];

		public string DurationFormatted => DurationFormatted(durationIndex);

		public Session(LevelInfo level)
		{
			this.level = level;
			this.playMusic = true;
			this.playGuidance = true;
			this.durationIndex = availableSessionDurations.Length - 1;
		}
	}

	private static Session currentSession = new Session(null);
	private static Queue<Session> sessionQueue = new Queue<Session>();

	public static Session CurrentSession => currentSession;

	public static int QueuedSessions => sessionQueue.Count;

	public static float TotalQueuedDuration
	{
		get
		{
			float t = 0;
			foreach (var session in sessionQueue)
				t += session.Duration;
			return t;
		}
	}

	public static bool TryStartNewSession(out Session session)
	{
		if (sessionQueue.Count > 0)
		{
			currentSession = sessionQueue.Dequeue();
			session = currentSession;
			return true;
		}
		else
		{
			currentSession = new Session(null);
			session = currentSession;
			return false;
		}
	}

	public static void ClearQueue()
	{
		currentSession = new Session(null);
		sessionQueue.Clear();
	}

	public static void SetQueue(Session session)
	{
		SetQueue(new List<Session>() { session });
	}

	public static void SetQueue(List<Session> list)
	{
		sessionQueue.Clear();
		foreach (var session in list)
			sessionQueue.Enqueue(session);
	}
}
