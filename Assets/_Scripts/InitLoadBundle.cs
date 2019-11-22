using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class InitLoadBundle 
{
//	static readonly int bundleVersionCode = 69;

	[RuntimeInitializeOnLoadMethod]
    static void Init()
	{
//#if UNITY_ANDROID
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.69.com.flowmeditation.flowvr.obb");
//#endif
	}
}