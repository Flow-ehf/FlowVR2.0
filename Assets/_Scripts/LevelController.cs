using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
	const float PauseMenuCloseTime = 10;
	const string FallbackLevel = "Breath1";

	public static LevelController Instance { get; private set; }

	[SerializeField] Text timerText;
	[SerializeField] Button pauseButton;
	[SerializeField] Button resumeButton;
	[SerializeField] Toggle muteMusicButton;
	[SerializeField] Toggle muteGuidanceButton;
	[SerializeField] UIPanel pausePanel;
	[SerializeField] AudioClip[] guidanceClips;
	[SerializeField] AudioClip[] musicClips;
	//[SerializeField] AudioClip[] ambianceClips;

	AudioSource guidanceAudio;
	AudioSource musicAudio;
	//AudioSource ambiance1;
	//AudioSource ambiance2;
	//AudioSource ambiance3;
	//AudioSource ambiance4;
	float timeLeft;
	bool isPaused;
	float sessionStartTimestamp;
	float pauseMenuTimer;

	OVRRaycaster raycaster;
	Coroutine waitEnableUIInteractCoroutine;
	Coroutine waitPauseMenuCloseCoroutine;

	public event Action QuitMeditation;

	public float TimeLeft => timeLeft;
	public float PlayTime => Time.unscaledTime - sessionStartTimestamp;

	void Awake()
	{
		Instance = this;

		musicAudio = GameObject.Find("Music")?.GetComponent<AudioSource>();
		guidanceAudio = GameObject.Find("Guidance")?.GetComponent<AudioSource>();
		//ambiance1 = GameObject.Find("Ambiance1")?.GetComponent<AudioSource>();
		//ambiance2 = GameObject.Find("Ambiance2")?.GetComponent<AudioSource>();
		//ambiance3 = GameObject.Find("Ambiance3")?.GetComponent<AudioSource>();
		//ambiance4 = GameObject.Find("Ambiance4")?.GetComponent<AudioSource>();

		raycaster = FindObjectOfType<OVRRaycaster>();
		raycaster.enabled = false;

		sessionStartTimestamp = Time.unscaledTime;
	}


	// Start is called before the first frame update
	void Start()
    {
		if(muteGuidanceButton != null)
		{
			//Update initial state
			muteGuidanceButton.isOn = SessionSettings.PlayGuidance;
			muteGuidanceButton.onValueChanged.AddListener((isOn) =>
			{
				//Save preference
				SessionSettings.PlayGuidance = isOn;
				guidanceAudio.mute = !isOn;
			});
		}
		if (muteMusicButton != null)
		{
			//Update initial state
			muteMusicButton.isOn = SessionSettings.PlayMusic;
			muteMusicButton.onValueChanged.AddListener((isOn) =>
			{
				//Save preference
				SessionSettings.PlayMusic = isOn;
				musicAudio.mute = !isOn;
			});
		}

		int totalLanguages = System.Enum.GetValues(typeof(LanguageManager.Language)).Length;
		guidanceAudio.clip = guidanceClips[SessionSettings.DurationIndex * totalLanguages + (int)LanguageManager.CurrentLanguage];
		guidanceAudio.Play();
		guidanceAudio.mute = !SessionSettings.PlayGuidance;

		musicAudio.clip = musicClips[SessionSettings.DurationIndex];
		musicAudio.Play();
		musicAudio.mute = !SessionSettings.PlayMusic;

		//if(ambiance1 != null)
		//{
		//	ambiance1.clip = ambianceClips[SessionSettings.DurationIndex * 4];
		//	ambiance1.Play();
		//}
		//if(ambiance2 != null)
		//{
		//	ambiance2.clip = ambianceClips[SessionSettings.DurationIndex * 4 + 1];
		//	ambiance2.Play();
		//}
		//if(ambiance3 != null)
		//{
		//	ambiance3.clip = ambianceClips[SessionSettings.DurationIndex * 4 + 2];
		//	ambiance3.Play();
		//}
		//if (ambiance4 != null)
		//{
		//	ambiance4.clip = ambianceClips[SessionSettings.DurationIndex * 4 + 3];
		//	ambiance4.Play();
		//}

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
			if(timerText != null)
			{
				if (timeLeft >= 60)
					timerText.text = Mathf.Floor(timeLeft / 60) + "m " + timeLeft % 60 + "s";
				else
					timerText.text = timeLeft + "s";
			}
			yield return new WaitForSeconds(1);
			timeLeft--;
			if (isPaused)
				yield return new WaitWhile(() => isPaused);
		}
		ReturnToMenu();
	}


	void Update()
	{
		if(!isPaused)
		{
			//Connected controller is touch
			if(OVRInput.IsControllerConnected(OVRInput.Controller.Touch))
			{
				if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
					Pause(!isPaused);
			}
			//Conected controller is probably GO remote
			else
			{
				if(OVRInput.GetDown(OVRInput.Button.Back) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
					Pause(!isPaused);
			}
		}
		else
		{
			//Reset pause menu close on input
			if (OVRInput.GetDown(OVRInput.Button.One))
				pauseMenuTimer = 0;
		}
	}


	void Pause(bool pause)
	{
		isPaused = pause;

		pausePanel.SetActive(isPaused);
		//Prevents click from interacting with any ui when opening pause menu
		if(isPaused)
		{
			pauseMenuTimer = 0;
			waitEnableUIInteractCoroutine = StartCoroutine(WaitEnableUIInteract());
			waitPauseMenuCloseCoroutine = StartCoroutine(WaitClosePauseMenu());
		}
		else
		{
			StopCoroutine(waitEnableUIInteractCoroutine);
			StopCoroutine(waitPauseMenuCloseCoroutine);
			raycaster.enabled = false;
		}
	}


	IEnumerator WaitClosePauseMenu()
	{
		yield return new WaitForSeconds(PauseMenuCloseTime);
		Pause(false);
	}


	IEnumerator WaitEnableUIInteract()
	{
		yield return new WaitForSeconds(1);
		raycaster.enabled = true;
	}


	public void ReturnToMenu()
	{
		QuitMeditation?.Invoke();
		LevelLoader.LoadLevel("MainMenu");
	}


	void OnDestroy()
	{
		if(Instance == this)
			Instance = null;
	}


#if UNITY_EDITOR
	[ContextMenu("Reset Audio Clips")]
	void ResetAudio()
	{
		//Set sensible defaults
		AudioClip gClip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Guidance/Breath1_Guidance.wav", typeof(AudioClip)) as AudioClip;

		guidanceClips = new AudioClip[SessionSettings.AvailableDurations.Length * System.Enum.GetValues(typeof(LanguageManager.Language)).Length];
		for (int i = 0; i < guidanceClips.Length; i++)
		{
			guidanceClips[i] = gClip;
		}

		AudioClip mClip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Music/Breath1_Music.wav", typeof(AudioClip)) as AudioClip;

		musicClips = new AudioClip[SessionSettings.AvailableDurations.Length];
		for (int i = 0; i < musicClips.Length; i++)
		{
			musicClips[i] = mClip;
		}

		//AudioClip a1Clip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Ambiance/Breathe/Video 1/Breathe_App_v2_flat_mixdown_Audio 2.wav", typeof(AudioClip)) as AudioClip;
		//AudioClip a2Clip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Ambiance/Breathe/Video 1/Breathe_App_v2_flat_mixdown_Audio 3.wav", typeof(AudioClip)) as AudioClip;
		//AudioClip a3Clip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Ambiance/Breathe/Video 1/Breathe_App_v2_flat_mixdown_Audio 4.wav", typeof(AudioClip)) as AudioClip;
		//AudioClip a4Clip = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Audio/Ambiance/Breathe/Video 1/Breathe_App_v2_flat_mixdown_Audio 5.wav", typeof(AudioClip)) as AudioClip;

		//ambianceClips = new AudioClip[SessionSettings.AvailableDurations.Length * 4];
		//for (int i = 0; i < ambianceClips.Length; i++)
		//{
		//	if (i % 4 == 0)
		//		ambianceClips[i] = a1Clip;
		//	else if (i % 4 == 1)
		//		ambianceClips[i] = a2Clip;
		//	else if (i % 4 == 2)
		//		ambianceClips[i] = a3Clip;
		//	else if (i % 4 == 3)
		//		ambianceClips[i] = a4Clip;
		//}
	}
#endif
}
