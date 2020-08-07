using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitLoadBundle 
{
//	static readonly int bundleVersionCode = 106;

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.106.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack1 - assets1.106.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack2 - assets2.106.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pack3 - assets3.106.com.flowmeditation.flowvr.obb");
#endif
	}
}