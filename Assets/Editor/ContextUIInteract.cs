using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ContextUIInteract 
{
	[MenuItem("CONTEXT/Button/Click")]
    static void ContextButtonClick(MenuCommand cmd)
	{
		var button = (Button)cmd.context;
		button.onClick.Invoke();
	}

	[MenuItem("CONTEXT/InputField/Select")]
	static void SelectInputField(MenuCommand cmd)
	{
		var input = (InputField)cmd.context;
		input.Select();
	}

	[MenuItem("CONTEXT/Toggle/Do Toggle")]
	static void ToggleToggle(MenuCommand cmd)
	{
		Toggle toggle = (Toggle)cmd.context;
		toggle.isOn = !toggle.isOn;
	}
}
