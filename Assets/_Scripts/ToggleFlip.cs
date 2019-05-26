using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleFlip : MonoBehaviour
{
	[SerializeField] GameObject onObject;
	[SerializeField] GameObject offObject;

	Toggle toggle;

    void Awake()
    {
		toggle = GetComponent<Toggle>();
		if (toggle != null)
		{
			toggle.graphic = null;
			toggle.onValueChanged.AddListener(OnToggled);
			OnToggled(toggle.isOn);
		}
	}

    
	void OnToggled(bool isOn)
	{
		onObject?.SetActive(isOn);
		offObject?.SetActive(!isOn);
		toggle.targetGraphic = null;
		toggle.targetGraphic = isOn ? onObject?.GetComponentInChildren<Graphic>() : offObject?.GetComponentInChildren<Graphic>();
	}
}
