using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitLoadBundle 
{
//	static readonly int bundleVersionCode = 74;

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
#if UNITY_ANDROID
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.74.com.flowmeditation.flowvr.obb");
		//AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - assets.74.com.flowmeditation.flowvr.obb");
		//AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - scenes.74.com.flowmeditation.flowvr.obb");
#endif
	}
}