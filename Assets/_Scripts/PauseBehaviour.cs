using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR;

public class PauseBehaviour : MonoBehaviour
{
	public static bool IsPaused { get; private set; }

	[RuntimeInitializeOnLoadMethod]
	static void Init()
    {
		PauseBehaviour instance = new GameObject(nameof(PauseBehaviour)).AddComponent<PauseBehaviour>();
		instance.hideFlags = HideFlags.HideAndDontSave;
		DontDestroyOnLoad(instance.gameObject);
    }

	private static VideoPlayer b_video;
	private static VideoPlayer Video
	{
		get
		{
			if (b_video == null)
				b_video = FindObjectOfType<VideoPlayer>();
			return b_video;
		}
	}

	private UserPresenceState prev_present = UserPresenceState.Present;

	private void Update()
	{
		if(prev_present != XRDevice.userPresence)
		{
			prev_present = XRDevice.userPresence;
			PauseApp(XRDevice.userPresence == UserPresenceState.NotPresent);
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		PauseApp(!focus);
	}

	private void OnApplicationPause(bool pause)
	{
		PauseApp(pause);
	}

	public static void PauseApp(bool pause)
	{
		Debug.Log("App pause: " + pause);

		Time.timeScale = pause ? 0 : 1;

		var vid = Video;
		if(vid != null)
		{
			vid.playbackSpeed = Time.timeScale;
		}

		IsPaused = pause;
	}
}
