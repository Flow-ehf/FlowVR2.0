using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class ToggleOnOffEvents : MonoBehaviour
{
	public UnityEvent OnClickOn;
	public UnityEvent OnClickOff;

	Toggle toggle;

	void Awake()
	{
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(OnValueChanged);
	}
    
	void OnValueChanged(bool isOn)
	{
		if (isOn)
			OnClickOn?.Invoke();
		else
			OnClickOff?.Invoke();
	}
}
