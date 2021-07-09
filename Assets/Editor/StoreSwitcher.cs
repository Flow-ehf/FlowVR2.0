using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class StoreSwitcher
{
	const string STEAMSTORE = "STEAM_STORE";
	const string OCULUSSTORE = "OCULUS_STORE";
	const string CORPORATE_ACCOUNT_UNLOCKED = "CORPORATE_ACCOUNT_UNLOCKED";

	[MenuItem("FlowVR/Set Steam store")]
	static void SetSteamStore()
	{
		string def = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		List<string> defs = new List<string>(def.Split(';'));
		defs.Remove(OCULUSSTORE);
		if (!defs.Contains(STEAMSTORE))
			defs.Add(STEAMSTORE);
		def = string.Join(";", defs);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, def);
	}

	[MenuItem("FlowVR/Set Oculus store")]
	static void SetOculusStore()
	{
		string def = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		List<string> defs = new List<string>(def.Split(';'));
		defs.Remove(STEAMSTORE);
		if (!defs.Contains(OCULUSSTORE))
			defs.Add(OCULUSSTORE);
		def = string.Join(";", defs);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, def);
	}

	[MenuItem("FlowVR/Toggle corporate account unlock dlc")]
	static void ToggleCorporateAccountUnlocked()
	{
		string def = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		List<string> defs = new List<string>(def.Split(';'));
		bool exists = defs.Remove(CORPORATE_ACCOUNT_UNLOCKED);
		if (!exists)
			defs.Add(CORPORATE_ACCOUNT_UNLOCKED);
		def = string.Join(";", defs);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, def);
	}

}
