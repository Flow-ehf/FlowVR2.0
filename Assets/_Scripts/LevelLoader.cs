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
		yield return new WaitForSecondsRealtime(1);
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
		yield return new WaitForSecondsRealtime(FadeDuration);
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
		yield return new WaitForSecondsRealtime(FadeDuration);
		yield return SceneManager.LoadSceneAsync(level);
		ScreenFade.instance.StartFade(FadeDuration, Color.clear);
		yield return new WaitForSecondsRealtime(FadeDuration);
		Time.timeScale = 1;
	}
}
