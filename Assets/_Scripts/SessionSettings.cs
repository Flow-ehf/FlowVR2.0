﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SessionSettings 
{
	static bool playMusic = false;
	static bool playGuidance = false;
	static int durationIndex = 0;

	static int[] availableSessionDurations = new int[]
	{
		240,
		480,
		720,
	};


	//Fetch settings from previous session, or defaults
	static SessionSettings()
	{
		playMusic = PlayerPrefs.GetInt("PlayMusic", 0) != 0;
		playGuidance = PlayerPrefs.GetInt("PlayGuidance", 0) != 0;
		durationIndex = PlayerPrefs.GetInt("DurationIndex", 0);
	}


	public static bool PlayMusic
	{
		get {
			return playMusic;
		}
		set {
			playMusic = value;
			PlayerPrefs.SetInt("PlayMusic", value ? 1 : 0);
		}
	}

	public static bool PlayGuidance
	{
		get {
			return playGuidance;
		}
		set {
			playGuidance = value;
			PlayerPrefs.SetInt("PlayGuidance", value ? 1 : 0);
		}
	}

	public static int Duration
	{
		get {
			return availableSessionDurations[durationIndex];
		}
	}

	public static int DurationIndex
	{
		get {
			return durationIndex;
		}
		set {
			durationIndex = Mathf.Clamp(value, 0, availableSessionDurations.Length - 1);
			PlayerPrefs.SetInt("DurationIndex", durationIndex);
		}
	}

	public static IEnumerable<int> AvailableDurations
	{
		get
		{
			foreach (var time in availableSessionDurations)
			{
				yield return time;
			}
		}
	}
}