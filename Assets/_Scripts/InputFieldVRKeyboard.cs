using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Weelco.VRKeyboard;

[RequireComponent(typeof(InputField))]
public class InputFieldVRKeyboard : MonoBehaviour, ISelectHandler
{
	[SerializeField] VRKeyboardFull keyboard;
	[SerializeField] bool enterIsNewline = false;
	[SerializeField] Button buttonClickOnEnter;

	InputField input;

	static InputField lastSelected;

	void OnValidate()
	{
		if (keyboard == null)
			keyboard = FindObjectOfType<VRKeyboardFull>(); 
	}

	void Awake()
	{
		input = GetComponent<InputField>();
	}

    // Start is called before the first frame update
    void Start()
    {
		if (keyboard != null)
		{
			keyboard.OnVRKeyboardBtnClick += HandleInput;
			keyboard.Init();
		}
    }

	void HandleInput(string key)
	{
		if (lastSelected == input)
		{
			if (key == VRKeyboardData.BACK)
			{
				if(input.text.Length > 0)
					input.text = input.text.Substring(0, input.text.Length - 1);
			}
			else if(key == VRKeyboardData.ENTER)
			{
				if (enterIsNewline)
					input.text += "\n";
				else
					buttonClickOnEnter?.onClick?.Invoke();

			}
			else
				input.text += key;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		lastSelected = input;
	}
}
