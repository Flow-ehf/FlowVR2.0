using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Platform;

public class LevelLoader : MonoBehaviour
{
	const string LoaderScene = "LevelLoad";
	const string StartScene = "LoginMenu";
	const float FadeDuration = 1;

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(LevelLoader)).AddComponent<LevelLoader>();
		instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	static LevelLoader instance;
	public static string Level { get; private set; }
	public static string LevelBeingLoaded { get; private set; }
	public static bool IsLoading { get; private set; }


	void Start()
	{
		Oculus.Platform.Core.Initialize();
		Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(OnEntitlementCheckComplete);
	}

	void OnEntitlementCheckComplete(Message msg)
	{
		if(msg.IsError)
		{
			Debug.LogError("App not owned. Quitting!");
			UnityEngine.Application.Quit();
		}
		else
		{
			LoadLevel(StartScene);
		}
	}

	public static void LoadLevel(string level, bool transition = true)
	{
		instance.StartCoroutine(WaitLoadLevel(level, transition));
	}


	static IEnumerator WaitLoadLevel(string level, bool transition)
	{
		if (IsLoading)
			yield return new WaitUntil(() => !IsLoading);

		IsLoading = true;

		// Wait for assetbundle loaded
		if (!InitLoadBundle.IsBundlesLoaded)
			yield return new WaitUntil(() => InitLoadBundle.IsBundlesLoaded);

		if (!UnityEngine.Application.CanStreamedLevelBeLoaded(level))
		{
			Debug.LogError($"Failed to load level '{level}'. AssetBundle not loaded?");
			IsLoading = false;
			yield break;
		}

		LevelBeingLoaded = level;

		Time.timeScale = 0;
		if (transition)
		{
			ScreenFade.StartFade(FadeDuration, Color.black);
			yield return FadeVolumeOut(FadeDuration);
		}
		else
		{
			ScreenFade.StartFade(0, Color.black);
			AudioListener.volume = 0;
		}

		//Load loader scene
		yield return SceneManager.LoadSceneAsync(LoaderScene);
		if (transition)
		{
			//Fade in
			ScreenFade.StartFade(FadeDuration, Color.clear);
			yield return new WaitForSecondsRealtime(FadeDuration);
			//Begin load target scene
		}
		var sceneLoad = SceneManager.LoadSceneAsync(level);
		sceneLoad.allowSceneActivation = false;

		if (transition)
		{
			var loadingBar = GameObject.FindGameObjectWithTag("LoadingBar")?.GetComponent<UnityEngine.UI.Image>();
			//Loading scene shows for minimum 3 seconds
			float duration = 0;

			do
			{
				if (loadingBar != null)
				{
					loadingBar.fillAmount = sceneLoad.progress / 0.9f;
				}
				duration += Time.unscaledDeltaTime;
				yield return null;
			}
			while (sceneLoad.progress < 0.9f || duration < 3f);

			//When target scene is loaded, fade to black
			ScreenFade.StartFade(FadeDuration, Color.black);
			yield return new WaitForSecondsRealtime(1);
		}
		//Activate target scene
		sceneLoad.allowSceneActivation = true;
		Level = level;
		LevelBeingLoaded = null;
		yield return sceneLoad;
		//Wait for video to load
		yield return WaitForVideo();
		//Fade in
		ScreenFade.StartFade(FadeDuration, Color.clear);
		//Fade in volume 
		Time.timeScale = 1;
		yield return FadeVolumeIn(FadeDuration);
		IsLoading = false;
	}


	static IEnumerator FadeVolumeOut(float duration)
	{
		float time = duration;
		while (time > 0)
		{
			yield return null;
			AudioListener.volume = time / duration;
			time -= Time.unscaledDeltaTime;
		}
		AudioListener.volume = 0;
	}

	static IEnumerator FadeVolumeIn(float duration)
	{
		float time = duration;
		while (time > 0)
		{
			yield return null;
			AudioListener.volume = 1 - time / duration;
			time -= Time.unscaledDeltaTime;
		}
		AudioListener.volume = 1;
	}

	static IEnumerator WaitForVideo()
	{
		var player = FindObjectOfType<UnityEngine.Video.VideoPlayer>();
		if(player != null)
		{
			player.Prepare();
			player.Pause();
			yield return null;
			yield return new WaitWhile(() => !player.isPrepared);
			player.Play();
		}
	}
}
