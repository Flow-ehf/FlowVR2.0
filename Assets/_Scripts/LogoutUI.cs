using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LogoutUI : MonoBehaviour
{
	Button button;

    // Start is called before the first frame update
    void Awake()
    {
		LoginManager.LoginChanged += LoginChanged;

		button = GetComponent<Button>();
		button.onClick.AddListener(LogOut);
		button.interactable = LoginManager.IsLoggedIn;
    }


	void LoginChanged(bool isLogin)
	{
		button.interactable = isLogin;
	}

    
	void LogOut()
	{
		LoginManager.Logout(true);
	}


	void OnDestroy()
	{
		LoginManager.LoginChanged -= LoginChanged;
	}
}
