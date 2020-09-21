//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR;

//public enum Platform
//{
//	Quest,
//	GO,
//	Rift,
//}

//public static class PlatformInfo
//{
//	static PlatformInfo()
//	{
//#if UNITY_STANDALONE
//		platform = Platform.Rift;
//#else
//		if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Go)
//			platform = Platform.GO;
//		else
//			platform = Platform.Quest;
//#endif
//	}

//	public static readonly Platform platform;
//}
