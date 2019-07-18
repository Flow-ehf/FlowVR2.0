using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoginUI : MonoBehaviour
{
	[SerializeField] LoginManager.LoginMethod loginMethod;
	[SerializeField] Text platformText;
	[Space]
	[SerializeField] bool requireEmailFormat = true;
	[SerializeField] InputField nameInput;
	[SerializeField] InputField passwordInput;

	Button button;
	LoginManager.LoginBase login;

	void Awake()
	{
		Debug.Log("UI awake");
		login = LoginManager.GetLogin(loginMethod);

		button = GetComponent<Button>();
		button.onClick.AddListener(DoLogin);
		button.interactable = false;

		if(platformText != null)
		{
			platformText.text = login?.PlatformName ?? "Guest";
		}
	}


	void Update()
	{
		button.interactable = (login == null || login.IsInitialized) && !LoginManager.IsLoggedIn && !LoginManager.IsLoggingIn && ((login == null || !(login is LoginManager.IRequireLoginDetails)) || (EmailValid() && passwordInput != null && passwordInput.text.Length > 0));
	}


	[ContextMenu("Login")]
	void EditorLogin()
	{
		DoLogin();
	}


	void DoLogin()
	{
		if (login != null)
		{
			var loginDetails = login as LoginManager.IRequireLoginDetails;
			if (loginDetails != null)
			{
				loginDetails.LoginUserName = nameInput.text;
				loginDetails.LoginPassword = passwordInput.text;

				nameInput.text = "";
				passwordInput.text = "";
			}
			login.Login();
		}
		else
		{
			LoginManager.LoginAsGuest();
		}
	}


	bool EmailValid()
	{
		if (nameInput == null)
			return false;
		if (!requireEmailFormat)
			return nameInput.text.Length > 0;
		else
			return AccountBackend.IsEmailFormat(nameInput.text);
	}


	//Called on component added
	private void Reset()
	{
		button = GetComponent<Button>();
		platformText = GetComponentInChildren<Text>();
	}
}
