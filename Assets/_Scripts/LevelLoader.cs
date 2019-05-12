using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	const float FadeDuration = 1;

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(LevelLoader)).AddComponent<LevelLoader>();
		instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	static LevelLoader instance;


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
		Time.timeScale = 0;
		ScreenFade.instance.StartFade(FadeDuration, Color.black);
		yield return FadeVolumeOut(FadeDuration);
		yield return SceneManager.LoadSceneAsync(level);
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
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
