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

	private static DLCInfo[] dlcs;

	public static DLCInfo[] GetDLCS()
	{
		if (dlcs == null)
			dlcs = Resources.LoadAll<DLCInfo>("DLC");
		return dlcs;
	}
}
