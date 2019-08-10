using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Registrer with email
[RequireComponent(typeof(Button))]
public class RegistrerLoginUI : MonoBehaviour
{
	const int MinPasswordLength = 1;

	[SerializeField] InputField emailInput;
	[SerializeField] InputField password1Input;
	[SerializeField] InputField password2Input;
	[SerializeField] Text registrationFailedText;
	[SerializeField] UnityEngine.Events.UnityEvent RegistrationSuccess;

	LoginManager.ICanRegistrer registrer;
	Coroutine fadeTextCoroutine;
	Button registrerButton;

	// Start is called before the first frame update
	void Start()
    {
		registrer = LoginManager.GetLogin(LoginManager.LoginMethod.Email) as LoginManager.ICanRegistrer;
		registrer.RegistrationComplete += OnRegistrationComplete;

		registrerButton = GetComponent<Button>();

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
		registrerButton.interactable = !LoginManager.IsLoggedIn && !LoginManager.IsLoggingIn && IsPasswordInputValid() && IsEmailInputValid();
	}


	void Registrer()
	{
		if (IsEmailInputValid() && IsPasswordInputValid())
			registrer.Registrer(emailInput.text, password1Input.text);
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
		if (password1Input == null || password2Input == null)
			return false;
		if (password1Input.text.Length + password2Input.text.Length < MinPasswordLength * 2)
			return false;
		if (password1Input.text != password1Input.text)
			return false;
		return true;
	}


	bool IsEmailInputValid()
	{
		if (emailInput == null)
			return false;
		return AccountBackend.IsEmailFormat(emailInput.text);
	}
}
