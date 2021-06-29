using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLoadBundle : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
#if UNITY_ANDROID
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.123.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pone.123.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/ptwo.123.com.flowmeditation.flowvr.obb");
#endif
	}
}