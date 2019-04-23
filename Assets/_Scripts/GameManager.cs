using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using Facebook.Unity;

public class GameManager : MonoBehaviour
{

	public static GameManager instance = null;

	Scene scene;
	AsyncOperation operation;

	public GameObject[] lvl;
	public string activeScene;
	public string selectedScene;
	public GameObject MusicToggle;
	public GameObject GuidanceToggle;
	public GameObject TimeDropDown;
	public GameObject LanguageDropDown;
	public GameObject LogOutButton;
	public GameObject ResumeButton;
	public GameObject PlayButton;
	public GameObject ToMenuButton;
	public GameObject PauseMenu;
	public GameObject Video;
	public int DropDownValue;
	public int LanguageValue;
	public bool playMusic;
	public bool playGuidance;
	public GameObject Music;
	public GameObject GuidanceIce;
	public GameObject GuidanceEng;
	public GameObject AmbianceIce;
	public GameObject AmbianceEng;
	public float timeLeft;
	public bool timePaused;

	void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Music = GameObject.Find("Music");
		GuidanceIce = GameObject.Find("GuidanceIce");
		GuidanceEng = GameObject.Find("GuidanceEng");
		AmbianceIce = GameObject.Find("AmbianceIce");
		AmbianceEng = GameObject.Find("AmbianceEng");
		MusicToggle = GameObject.Find("/MenuManager/Canvas/Video_Panel/Music_Toggle");
		GuidanceToggle = GameObject.Find("/MenuManager/Canvas/Video_Panel/Guidance_Toggle");
		TimeDropDown = GameObject.Find("/MenuManager/Canvas/Video_Panel/Time_DropDown");
		PlayButton = GameObject.Find("/MenuManager/Canvas/Video_Panel/Play");
		LogOutButton = GameObject.Find("/MenuManager/Canvas/Main_Panel/LogOut");
		PauseMenu = GameObject.Find("/MenuManager");
		Video = GameObject.Find("/Video");
		scene = SceneManager.GetActiveScene();
		activeScene = scene.name;
		timePaused = false;

		//make sure that we only have a single instance of the scene manager
		if (instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		if (playMusic == false && Music)
		{
			Music.SetActive(false);
		}
		else if (Music)
		{
			Music.SetActive(true);
		}

		if (playGuidance == false && GuidanceEng || GuidanceIce)
		{
			GuidanceEng.SetActive(false);
			GuidanceIce.SetActive(false);
		}

		if(playGuidance == true && GuidanceEng || GuidanceIce)
		{
			if (LanguageValue == 0)
			{
				GuidanceIce.SetActive(true);
			}
			if (LanguageValue == 1)
			{
				GuidanceEng.SetActive(true);
			}
		}

		if (LanguageValue == 0 && AmbianceEng || AmbianceIce)
		{
			AmbianceIce.SetActive(true);
			AmbianceEng.SetActive(false);
		}
		if (LanguageValue == 1 && AmbianceEng || AmbianceIce)
		{
			AmbianceIce.SetActive(false);
			AmbianceEng.SetActive(true);
		}

		if (activeScene != "MainMenu")
		{
			selectedScene = "MainMenu";
			if (DropDownValue == 0)
			{
				timeLeft = 240f;
			}
			if (DropDownValue == 1)
			{
				timeLeft = 480f;
			}
			if (DropDownValue == 2)
			{
				timeLeft = 720f;
			}
		}

		if (activeScene == "MainMenu" && LogOutButton)
		{
			LogOutButton.GetComponent<Button>().onClick.AddListener(LogOut);
		}

		if (activeScene == "MainMenu" && PlayButton)
		{
			PlayButton.GetComponent<Button>().onClick.AddListener(PlaySelectedScene);
		}
	}

	void Update()
	{
		lvl = GameObject.FindGameObjectsWithTag("Levels");
		for (int i = 0; i < lvl.Length; i++)
		{
			Button lvls = lvl[i].GetComponent<Button>();
			lvls.onClick.AddListener(() => lvlClicked(lvls));
		}

		if (activeScene != "MainMenu" && activeScene != "LoginMenu")
		{
			if(timePaused == false)
			{
				timeLeft -= Time.deltaTime;
			}
			else if(timePaused)
			{
				timeLeft = timeLeft;
			}

			if (timeLeft < 0)
			{
				SceneManager.LoadScene(selectedScene);
			}
		}


		if (MusicToggle)
		{
			if (MusicToggle.GetComponent<Toggle>().isOn == true && MusicToggle)
			{
				playMusic = true;
			}
			else
			{
				playMusic = false;
			}
		}


		if (GuidanceToggle)
		{
			if (GuidanceToggle.GetComponent<Toggle>().isOn == true && GuidanceToggle)
			{
				playGuidance = true;
			}
			else
			{
				playGuidance = false;
			}
		}

		if (TimeDropDown)
		{
			DropDownValue = TimeDropDown.GetComponent<Dropdown>().value;
			if (Input.GetKeyUp("0"))
			{
				if (activeScene != "MainMenu")
				{
					PauseMenu.transform.GetChild(0).gameObject.SetActive(true);
					ToMenuButton = GameObject.Find("/MenuManager/Canvas/Menu");
					ResumeButton = GameObject.Find("/MenuManager/Canvas/Resume");
					ToMenuButton.GetComponent<Button>().onClick.AddListener(toMenuClicked);
					ResumeButton.GetComponent<Button>().onClick.AddListener(resumeClicked);
					Video.GetComponent<VideoPlayer>().Stop();
					//timePaused = true;
				}
			}
		}
	}

	public void changeLevelDropDownValue()
	{
		LanguageValue = LanguageDropDown.GetComponent<Dropdown>().value;
	}

	void lvlClicked(Button button)
	{
		selectedScene = button.name;
		PlayButton.GetComponent<Button>().onClick.AddListener(PlaySelectedScene);
	}

	public void toMenuClicked()
	{
		SceneManager.LoadScene(selectedScene);
	}

	public void resumeClicked()
	{
		PauseMenu.transform.GetChild(0).gameObject.SetActive(false);
		Video.GetComponent<VideoPlayer>().Play();
		timePaused = false;
	}

	public void PlaySelectedScene()
	{
		SceneManager.LoadScene(selectedScene);
	}

	public void LogOut()
	{
		FB.LogOut();
		SceneManager.LoadScene("LoginMenu");
	}
}

