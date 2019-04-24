using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoginUI : MonoBehaviour
{
	[SerializeField] LoginManager.LoginMethod loginMethod;
	[SerializeField] Text platformText;

	Button button;
	LoginManager.LoginBase login;

	void Awake()
	{
		Debug.Log("UI awake");
		login = LoginManager.Getlogin(loginMethod);
		if (login != null)
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(login.Login);
			button.interactable = false;

			if(platformText != null)
			{
				platformText.text = login.PlatformName;
			}

			if (!login.IsInitialized)
				login.Initalized += LoginInitialized;
			else
				LoginInitialized();
		}
		else
			Debug.LogError("LoginUI failed to find login method: " + loginMethod);
	}


	[ContextMenu("Login")]
	void EditorLogin()
	{
		login.Login();
	}


	void LoginInitialized()
	{
		button.interactable = true;
		LoginManager.LoginChanged += LoginChanged;
	}


	void LoginChanged(bool loggedIn)
	{
		button.interactable = !loggedIn;
	}


	void OnDestroy()
	{
		if (login != null)
			login.Initalized -= LoginInitialized;

		LoginManager.LoginChanged -= LoginChanged;
	}


	//Called on component added
	private void Reset()
	{
		button = GetComponent<Button>();
		platformText = GetComponentInChildren<Text>();
	}
}
