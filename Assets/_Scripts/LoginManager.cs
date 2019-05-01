﻿
#define BYPASS_LOGIN

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Facebook.Unity;


public class LoginManager : MonoBehaviour
{
	public UnityEvent LoggedIn;
	public UnityEvent LoggedOut;

	static LoginManager instance;
	//Any platform currently logged in
	public static bool IsLoggedIn => currentLogin != null && currentLogin.IsLoggedIn;

	public static event Action<bool> LoginChanged;

	static FBLogin fbLogin = new FBLogin();

	static LoginBase currentLogin;


	public enum LoginMethod
	{
		FB,
	}


	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			DestroyImmediate(this);
	}


	void Start()
	{
		fbLogin.Initialize();
	}


	public static LoginBase GetLogin(LoginMethod loginMethod)
	{
		switch(loginMethod)
		{
			default:
			case LoginMethod.FB:
				return fbLogin;
		}
	}


	public static void Logout()
	{
		if(IsLoggedIn)
		{
			currentLogin.Logout();
		}
	}


	static void OnLoggedIn(LoginBase login)
	{
		currentLogin = login;
		instance?.LoggedIn.Invoke();
		LoginChanged?.Invoke(true);

		SceneManager.LoadScene("MainMenu");
	}


	//Todo add data/error info
	static void OnlogginFailed()
	{

	}


	static void OnLoggedOut()
	{
		instance?.LoggedOut.Invoke();
		LoginChanged?.Invoke(false);

		//Return to login screen when logging out
		if (SceneManager.GetActiveScene().name != "LoginMenu")
			SceneManager.LoadScene("LoginMenu");
	}

	public abstract class LoginBase
	{
		public abstract bool IsInitialized { get; }
		public abstract bool IsLoggedIn { get; }
		public abstract string PlatformName { get; }

		public event Action Initalized;

		public void Login()
		{
			//No platform is currently logged in and this platform is ready for login
			if (!LoginManager.IsLoggedIn && IsInitialized)
			{
#if UNITY_EDITOR && BYPASS_LOGIN
				OnLoggedIn(this);
#else
				DoLogin();
#endif

			}
		}

		public void Logout()
		{
			if (IsLoggedIn)
				DoLogout();
		}

		public abstract void Initialize();
		protected abstract void DoLogin();
		protected abstract void DoLogout();

		protected void OnInitialized()
		{
			Debug.Log(PlatformName + ": login ready!");

			Initalized?.Invoke();
		}
	}

	class FBLogin : LoginBase
	{
		public override bool IsInitialized => FB.IsInitialized;
		public override bool IsLoggedIn => FB.IsLoggedIn;

		public override string PlatformName { get; } = "Facebook";


		public override void Initialize()
		{
			FB.Init(OnInitialized);
		}


		protected override void DoLogin()
		{
			if (!IsInitialized)
			{
				Debug.LogError("Login failed! FB not initialized.");
				OnlogginFailed();
			}
			else if (LoginManager.IsLoggedIn)
			{
				Debug.LogError("Login failed! Already logged in");
				OnlogginFailed();
			}
			else
				FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, OnFBLogin);
		}


		void OnFBLogin(ILoginResult result)
		{
			if (IsLoggedIn)
				OnLoggedIn(this);
			else
				OnlogginFailed();
		}


		protected override void DoLogout()
		{
			FB.LogOut();
			OnLoggedOut();
		}
	}
}
