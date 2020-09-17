using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPlatformEnabler : MonoBehaviour
{
	[SerializeField] Behaviour script;
 	[Space]
	[SerializeField] bool goEnabled;
	[SerializeField] bool questEnabled;
	[SerializeField] bool riftEnabled;

	private void Start()
	{
		if(script != null)
		{
			switch(PlatformInfo.platform)
			{
				case Platform.GO:
					script.enabled = goEnabled;
					break;
				case Platform.Quest:
					script.enabled = questEnabled;
					break;
				case Platform.Rift:
					script.enabled = riftEnabled;
					break;
			}
		}
	}
}
