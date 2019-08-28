using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoginUI : MonoBehaviour, ILoginDetails
{
	[SerializeField] LoginManager.LoginMethod loginMethod;
	[SerializeField] Text platformText;
	[Space]
	[SerializeField] bool requireEmailFormat = true;
	[SerializeField] InputField nameInput;
	[SerializeField] InputField passwordInput;
	[SerializeField] Text errorText;

	Button button;
	LoginManager.LoginBase login;
	float errorTimestamp;

	public string Username => nameInput.text;
	public string Password => passwordInput.text;
	public string FirstName => null;
	public string LastName => null;

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


	void OnEnable()
	{
		if (errorText != null)
		{
			errorText.text = "";
			errorText.color = errorText.color.WithAlpha(1);
			AccountBackend.Error += OnError;
		}
	}


	void OnError(AccountBackend.Result result)
	{
		if(result.method == AccountBackend.Result.Method.Login)
		{
			errorTimestamp = Time.realtimeSinceStartup;
			errorText.color = errorText.color.WithAlpha(1);
			errorText.text = result.GetError().GetMessage();
		}
	}


	void Update()
	{
		button.interactable = (login == null || login.IsInitialized) && !LoginManager.IsLoggedIn && !LoginManager.IsLoggingIn && EmailValid() && PasswordValid();

		if (errorText != null)
		{
			float a = errorText.color.a;
			if (a > 0 && errorTimestamp < Time.realtimeSinceStartup - 3)
			{
				a -= Time.unscaledDeltaTime;
				a = Mathf.Clamp01(a);
				errorText.color = errorText.color.WithAlpha(a);
			}
		}
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
			login.LoginDetails = this;
			login.Login();
			nameInput.text = "";
			passwordInput.text = "";
		}
		else
		{
			LoginManager.LoginAsGuest();
		}
	}


	bool EmailValid()
	{
		if (nameInput == null)
			return true;
		if (!requireEmailFormat)
			return nameInput.text.Length > 0;
		else
			return AccountBackend.IsEmailFormat(nameInput.text);
	}

	bool PasswordValid()
	{
		if (passwordInput == null)
			return true;
		return passwordInput.text.Length > 0;
	}


	void OnDisable()
	{
		AccountBackend.Error -= OnError;
	}


	//Called on component added
	private void Reset()
	{
		button = GetComponent<Button>();
		platformText = GetComponentInChildren<Text>();
	}
}
