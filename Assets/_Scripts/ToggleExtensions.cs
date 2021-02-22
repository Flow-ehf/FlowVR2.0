using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class ToggleExtensions
{
	public static void SetWithoutNotify(this Toggle toggle, bool isOn, UnityAction<bool> action)
	{
		toggle.onValueChanged.RemoveListener(action);
		toggle.isOn = isOn;
		toggle.onValueChanged.AddListener(action);
	}
}
