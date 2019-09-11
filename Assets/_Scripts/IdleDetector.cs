using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class IdleDetector : MonoBehaviour
{
	const float IDLE_KICK_TIME = 10 * 60;


	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(IdleDetector)).AddComponent<IdleDetector>();
		instance.enabled = false;
		//instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	static IdleDetector instance;


	[SerializeField] float idleTime = 0;


	void Awake()
	{
		SceneManager.activeSceneChanged += OnLevelLoad;
		LoginManager.LoginChanged += OnLoginChanged;
	}

	void Update()
	{
		if (XRDevice.userPresence != UserPresenceState.Present)
		{
			idleTime += Time.unscaledDeltaTime;
			if (idleTime > IDLE_KICK_TIME)
			{
				idleTime = 0;
				LevelLoader.LoadLevel("LoginMenu");
			}
		}
		else
			idleTime = 0;
	}


	void OnLevelLoad(Scene oldScene, Scene newScene)
	{
		if (newScene.name == "LoginMenu" || !LoginManager.currentUser.isSubscribed)
			enabled = false;
		else
			enabled = true;
		idleTime = 0;
	}


	void OnLoginChanged(bool loggedIn)
	{
		if (!loggedIn)
			enabled = false;
	}
}
