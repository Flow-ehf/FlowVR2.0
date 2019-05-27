
#define BYPASS_LOGIN

using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Facebook.Unity;
using Google;


public class LoginManager : MonoBehaviour
{
	public UnityEvent LoggedIn;
	public UnityEvent LoggedOut;

	static LoginManager instance;
	//Any platform currently logged in
	public static bool IsLoggedIn => currentLogin != null && currentLogin.IsLoggedIn;

	public static event Action<bool> LoginChanged;

	static FBLogin fbLogin = new FBLogin();
	static GoogleLogin googleLogin = new GoogleLogin();

	static LoginBase currentLogin;


	public enum LoginMethod
	{
		FB,
		Google,
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
		googleLogin.Initialize();
	}


	public static LoginBase GetLogin(LoginMethod loginMethod)
	{
		switch(loginMethod)
		{
			default:
			case LoginMethod.FB:
				return fbLogin;
			case LoginMethod.Google:
				return googleLogin;
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

		LevelLoader.LoadLevel("MainMenu");
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
		if (LevelLoader.CurrentLevel != "LoginMenu")
			LevelLoader.LoadLevel("LoginMenu");
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

	class GoogleLogin : LoginBase
	{
		const string webClientId = "";

		public override bool IsInitialized => isInitialized;

		public override bool IsLoggedIn => loggedInUser != null;

		public override string PlatformName => "Google";

		bool isInitialized;
		GoogleSignInConfiguration configuration;
		GoogleSignInUser loggedInUser;


		public override void Initialize()
		{
			configuration = new GoogleSignInConfiguration
			{
				WebClientId = webClientId,
				RequestIdToken = true
			};
			isInitialized = true;
			OnInitialized();
		}


		protected override void DoLogin()
		{
			GoogleSignIn.Configuration = configuration;
			GoogleSignIn.Configuration.UseGameSignIn = false;
			GoogleSignIn.Configuration.RequestIdToken = true;

			GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnLogin);
		}


		protected override void DoLogout()
		{
			loggedInUser = null;
			GoogleSignIn.DefaultInstance.SignOut();
			OnLoggedOut();
		}


		void OnLogin(Task<GoogleSignInUser> result)
		{
			if (result.IsFaulted || result.IsCanceled)
				OnlogginFailed();
			else
			{
				loggedInUser = result.Result;
				OnLoggedIn(this);
			}
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
