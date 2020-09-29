using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLoadBundle : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
#if UNITY_ANDROID
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/main.121.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/pone.121.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/ptwo.121.com.flowmeditation.flowvr.obb");
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.flowmeditation.flowvr/ptre.121.com.flowmeditation.flowvr.obb");
#endif
	}
}