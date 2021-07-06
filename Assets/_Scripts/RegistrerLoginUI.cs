using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Registrer with email
[RequireComponent(typeof(Button))]
public class RegistrerLoginUI : MonoBehaviour, ILoginDetails
{
	const int MinPasswordLength = 1;

	[SerializeField] LoginManager.LoginMethod registerMethod = LoginManager.LoginMethod.Email;
	[SerializeField] InputField firsNameText;
	[SerializeField] InputField lastNameText;
	[SerializeField] InputField emailInput;
	[SerializeField] InputField password1Input;
	[SerializeField] InputField password2Input;
	[SerializeField] Text registrationFailedText;
	[SerializeField] UnityEngine.Events.UnityEvent RegistrationSuccess;

	IRegisterHandler registrer;
	Coroutine fadeTextCoroutine;
	Button registerButton;

	public string Username => emailInput.text;
	public string Password => password1Input.text;
	public string FirstName => firsNameText.text;
	public string LastName => lastNameText.text;

	// Start is called before the first frame update
	void Start()
    {
		registrer = LoginManager.GetLogin(registerMethod) as IRegisterHandler;

		registerButton = GetComponent<Button>();
		registerButton.onClick.AddListener(Registrer);

		if (registrationFailedText != null)
			registrationFailedText.text = "";
    }


	void OnEnable()
	{
		AccountBackend.Error += OnError;
	}


	void OnDisable()
	{
		AccountBackend.Error -= OnError;
	}


	void OnError(AccountBackend.Result result)
	{
		if (result.method == AccountBackend.Result.Method.Registrer)
			DisplayErrorText(result.GetError().GetMessage());
	}


    // Update is called once per frame
    void Update()
    {
		registerButton.interactable = !LoginManager.IsLoggedIn && !LoginManager.IsLoggingIn && IsPasswordInputValid() && IsEmailInputValid();// && IsNameInputValid();
	}


	void Registrer()
	{
		if (IsEmailInputValid() && IsPasswordInputValid())
		{
			registrer.Registrer(this, OnRegistrationComplete);
		}
	}


	void OnRegistrationComplete(bool success)
	{
		if (!success)
			DisplayErrorText("Failed to registrer");
		else
			RegistrationSuccess?.Invoke();
	}


	void DisplayErrorText(string text)
	{
		StopCoroutine(fadeTextCoroutine);
		fadeTextCoroutine = StartCoroutine(WaitErrorText(text));
	}

	IEnumerator WaitErrorText(string text)
	{
		if(registrationFailedText != null)
		{
			registrationFailedText.text = text;

			float duration = 5;
			float time = duration;

			while(time > 0)
			{
				registrationFailedText.color = registrationFailedText.color.WithAlpha(time / duration);
				time -= Time.deltaTime;
				yield return null;
			}
			registrationFailedText.color = registrationFailedText.color.WithAlpha(0);
		}
	}

	
	bool IsPasswordInputValid()
	{
		if (password1Input == null)
			return false;
		if (password1Input.text.Length < MinPasswordLength)
			return false;
		if(password2Input != null && password1Input.text != password1Input.text)
			return false;
		return true;
	}


	bool IsEmailInputValid()
	{
		if (emailInput == null)
			return false;
		return AccountBackend.IsEmailFormat(emailInput.text);
	}

	bool IsNameInputValid()
	{
		if (firsNameText == null || lastNameText == null)
			return true;
		return firsNameText.text.Length > 0 && lastNameText.text.Length > 0;
	}
}
