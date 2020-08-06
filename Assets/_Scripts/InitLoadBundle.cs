using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitLoadBundle 
{
//	static readonly int bundleVersionCode = 88;

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.88.com.flowmeditation.flowvr.obb");
		//AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - assets.88.com.flowmeditation.flowvr.obb");
		//AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - scenes.88.com.flowmeditation.flowvr.obb");
#endif
	}
}