using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum Platform
{
	Quest,
	Rift,
}

public static class PlatformInfo
{
	static PlatformInfo()
	{
#if UNITY_STANDALONE
		platform = Platform.Rift;
#else
			platform = Platform.Quest;
#endif
	}

	public static readonly Platform platform;
}
