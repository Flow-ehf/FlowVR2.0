
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
	const int LOGIN_CACHE_LIFETIME = int.MaxValue;

	public UnityEvent LoggedIn;
	public UnityEvent LoggedOut;

	static LoginManager instance;
	//Any platform currently logged in
	public static bool IsLoggedIn => currentLogin != null && currentLogin.IsLoggedIn;

	public static event Action<bool> LoginChanged;

	static FBLogin fbLogin = new FBLogin();
	static GoogleLogin googleLogin = new GoogleLogin();

	static LoginBase currentLogin;

	public static User currentUser;

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

		currentUser = GetCachedUser();
		//User logged in previously
		if(currentUser != null)
		{
			//Login is outdated
			if (currentUser.LoginDuration > LOGIN_CACHE_LIFETIME)
				currentUser = null;
			else
			{
				//Already logged in to non company account, skip to menu
				if (!currentUser.IsCompanyAccount)
					LevelLoader.LoadLevel("MainMenu");
			}
		}
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
		currentUser = login.GetUser();
		instance?.LoggedIn.Invoke();
		LoginChanged?.Invoke(true);

		bool firstLogin = PlayerPrefs.GetInt("HasLogin", 0) == 0;

		PlayerPrefs.SetInt("HasLogin", 1);

		if (firstLogin && !currentUser.IsCompanyAccount)
			LevelLoader.LoadLevel("BuySubscription");
		else
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
		if (LevelLoader.Level != "LoginMenu")
			LevelLoader.LoadLevel("LoginMenu");
	}


	static User GetCachedUser()
	{
		return null;
	}


	public class User
	{
		bool isCompanyAccount;
		DateTime lastLogin;

		public bool IsCompanyAccount => isCompanyAccount;
		public DateTime LastLoginTime => lastLogin;
		public int LoginDuration => (int)DateTime.UtcNow.Subtract(lastLogin).TotalSeconds;

		public User(bool isCompanyAccount)
		{
			this.isCompanyAccount = isCompanyAccount;
			this.lastLogin = DateTime.UtcNow;
		}
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
		public abstract User GetUser();
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


		public override User GetUser()
		{
			return new User(false);
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


		public override User GetUser()
		{
			return new User(false);
		}
	}
}
