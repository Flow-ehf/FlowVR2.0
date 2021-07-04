using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DLC", menuName = "DLC Info")]
public class DLCInfo : ScriptableObject
{
	public uint steamAppid;
	public string sku;
	public string desc;
	public string name;
	public string steamURL;
	public string price;
	public bool alwaysUnlocked = false;

	private static DLCInfo[] dlcs;

	public static DLCInfo[] GetDLCS()
	{
		if (dlcs == null)
			dlcs = Resources.LoadAll<DLCInfo>("DLC");
		return dlcs;
	}

	public static DLCInfo FindSKu(string sku)
	{
		return dlcs.Find((d) => d.sku == sku);
	}

#if STEAM_STORE
	public static DLCInfo FindSteamAPPId(Steamworks.AppId appid)
	{
		return dlcs.Find((d) => d.steamAppid == appid.Value);
	}
#endif
}
