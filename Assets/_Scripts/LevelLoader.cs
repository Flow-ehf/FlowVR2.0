using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	const string LoaderScene = "LevelLoad";
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


	void Awake()
	{
		Level = SceneManager.GetActiveScene().name;
	}


	IEnumerator Start()
	{
		Time.timeScale = 0;
		AudioListener.volume = 0;
		yield return new WaitForSecondsRealtime(1);
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
		yield return FadeVolumeIn(FadeDuration);
		Time.timeScale = 1;
	}


	public static void LoadLevel(string level)
	{
		instance.StartCoroutine(WaitLoadLevel(level));
	}


	static IEnumerator WaitLoadLevel(string level)
	{
		LevelBeingLoaded = level;

		Time.timeScale = 0;
		//Fade to black
		ScreenFade.instance.StartFade(FadeDuration, Color.black);
		//Fade volume out
		yield return FadeVolumeOut(FadeDuration);
		//Load loader scene
		yield return SceneManager.LoadSceneAsync(LoaderScene);
		//Fade in
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
		yield return new WaitForSecondsRealtime(FadeDuration);
		//Begin load target scene
		var sceneLoad = SceneManager.LoadSceneAsync(level);
		sceneLoad.allowSceneActivation = false;

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
		ScreenFade.instance.StartFade(FadeDuration, Color.black);
		yield return new WaitForSecondsRealtime(1);
		//Activate target scene
		sceneLoad.allowSceneActivation = true;
		Level = level;
		LevelBeingLoaded = null;
		yield return sceneLoad;
		//Fade in
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
		//Fade in volume 
		yield return FadeVolumeIn(FadeDuration);
		Time.timeScale = 1;
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

}
