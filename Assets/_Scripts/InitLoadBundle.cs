using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLoadBundle : MonoBehaviour
{
	//	const int bundleVersionCode = 108;

	private static readonly string[] bundles =
	{
		"/sdcard/Android/obb/com.flowmeditation.flowvr/main.108.com.flowmeditation.flowvr.obb",
		"/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - assets1.108.com.flowmeditation.flowvr.obb",
		"/sdcard/Android/obb/com.flowmeditation.flowvr/pack2 - assets2.108.com.flowmeditation.flowvr.obb",
		"/sdcard/Android/obb/com.flowmeditation.flowvr/pack3 - assets3.108.com.flowmeditation.flowvr.obb",
	};

	public static bool IsBundlesLoaded { get; private set; }

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
		InitLoadBundle instance = new GameObject(nameof(InitLoadBundle)).AddComponent<InitLoadBundle>();
		instance.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance);
	}

	private void Start()
	{
#if UNITY_ANDROID
		StartCoroutine(LoadBundles());
#else
		IsBundlesLoaded = true;
#endif

	}

	private IEnumerator LoadBundles()
	{
		Debug.Log("[LoadBundle] Begin loading bundles");
		for (int i = 0; i < bundles.Length; i++)
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundles[i]);
			yield return request;
			if (request.assetBundle == null)
				Debug.LogError("[LoadBundle] Error: Failed to load bundle " + bundles[i]);
			else
				Debug.Log("[LoadBundle] Loaded " + bundles[i]);
		}
		Debug.Log("[LoadBundle] Finished loading bundles");

		IsBundlesLoaded = true;
	}
}