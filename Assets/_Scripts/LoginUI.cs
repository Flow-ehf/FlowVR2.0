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
		if (login != null)
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(DoLogin);
			button.interactable = false;

			if(platformText != null)
			{
				platformText.text = login.PlatformName;
			}
		}
		else
			Debug.LogError("LoginUI failed to find login method: " + loginMethod);
	}


	void Update()
	{
		button.interactable = login != null && login.IsInitialized && !LoginManager.IsLoggedIn && !LoginManager.IsLoggingIn && (!(login is LoginManager.IRequireLoginDetails) || (NameInputValid() && passwordInput != null && passwordInput.text.Length > 0));
	}


	[ContextMenu("Login")]
	void EditorLogin()
	{
		DoLogin();
	}


	void DoLogin()
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


	bool NameInputValid()
	{
		if (nameInput == null)
			return false;
		if (!requireEmailFormat)
			return nameInput.text.Length > 0;
		else
		{
			string text = nameInput.text.Trim();
			//Email is minimum 5 characters (a@b.c)
			if (text.Length < 5)
				return false;
			//must have an '@' at min second character
			int at = text.IndexOf('@');
			if (at < 1)
				return false;
			//Must have a period 
			int period = text.IndexOf('.');
			//Period must be after at +1 character and must have character after it
			if (period <= at + 1 || period == text.Length - 1)
				return false;
			return true;
		}
	}


	//Called on component added
	private void Reset()
	{
		button = GetComponent<Button>();
		platformText = GetComponentInChildren<Text>();
	}
}
