using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitLoadBundle 
{
//	static readonly int bundleVersionCode = 108;

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.108.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - assets1.108.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack2 - assets2.108.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack3 - assets3.108.com.flowmeditation.flowvr.obb");
#endif
	}
}