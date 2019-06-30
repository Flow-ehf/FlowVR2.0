
//#define BYPASS_LOGIN

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
	//Duration before login cache is invalid, in seconds
	const int LOGIN_CACHE_LIFETIME = int.MaxValue;

	public UnityEvent LoggedIn;
	public UnityEvent LoggedOut;

	static LoginManager instance;
	//Any platform currently logged in
	public static bool IsLoggedIn => currentLogin != null && currentLogin.IsLoggedIn;

	public static event Action<bool> LoginChanged;

	static FBLogin fbLogin = new FBLogin();
	static GoogleLogin googleLogin = new GoogleLogin();
	static EmailLogin emailLogin = new EmailLogin();

	static LoginBase currentLogin;

	public static AccountBackend.User currentUser;

	public static bool IsLoggingIn { get; private set; }

	public enum LoginMethod
	{
		FB,
		Google,
		Email, 
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
		if (currentUser != null)
		{
			//Login is outdated
			if ((DateTime.UtcNow - currentUser.LastLogin).Seconds > LOGIN_CACHE_LIFETIME)
				currentUser = null;
			else
			{
				//Already logged in to non company account, skip to menu
				if (!currentUser.IsSubscribed)
					LevelLoader.LoadLevel("MainMenu");
			}
		}
	}


	public static LoginBase GetLogin(LoginMethod loginMethod)
	{
		switch (loginMethod)
		{
			default:
			case LoginMethod.FB:
				return fbLogin;
			case LoginMethod.Google:
				return googleLogin;
			case LoginMethod.Email:
				return emailLogin;
		}
	}


	public static void Logout()
	{
		if (IsLoggedIn)
		{
			currentLogin.Logout();
		}
	}


	static void OnLoggedIn(LoginBase login)
	{
		currentLogin = login;
		instance.StartCoroutine(WaitGetLoggedInUserData());
	}


	static IEnumerator WaitGetLoggedInUserData()
	{
		yield return AccountBackend.WaitFetchUserDetails(currentLogin.LoggedInEmail, (user) =>
		{
			currentUser = user;
			OnFetchedUserData();
		});
		if (currentUser == null)
		{
			Debug.LogError("Failed to fetch user login data");
		}
	}


	static void OnFetchedUserData()
	{
		instance?.LoggedIn.Invoke();
		LoginChanged?.Invoke(true);

		bool firstLogin = PlayerPrefs.GetInt("HasLogin", 0) == 0;

		PlayerPrefs.SetInt("HasLogin", 1);

		if (firstLogin && !currentUser.IsSubscribed)
			LevelLoader.LoadLevel("BuySubscription");
		else
			LevelLoader.LoadLevel("MainMenu");
	}


	//Todo add data/error info
	static void OnlogginFailed()
	{
		Debug.LogError("Login Canceled/Failed");
	}


	static void OnLoggedOut()
	{
		instance?.LoggedOut.Invoke();
		LoginChanged?.Invoke(false);

		currentLogin = null;
		currentUser = null;

		//Return to login screen when logging out
		if (LevelLoader.Level != "LoginMenu")
			LevelLoader.LoadLevel("LoginMenu");
	}


	static AccountBackend.User GetCachedUser()
	{
		return null;
	}


	public abstract class LoginBase
	{
		public abstract bool IsInitialized { get; }
		public abstract bool IsLoggedIn { get; }
		public abstract string PlatformName { get; }
		public abstract string LoggedInEmail { get; }

		public event Action Initalized;

		public void Login()
		{
			//No platform is currently logged in and this platform is ready for login
			if (!LoginManager.IsLoggingIn && !LoginManager.IsLoggedIn && IsInitialized)
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

		protected virtual void OnInitialized()
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

		public override string LoggedInEmail => loggedInUser?.Email ?? "";

		bool isInitialized;
		GoogleSignInConfiguration configuration;
		GoogleSignInUser loggedInUser;


		public override void Initialize()
		{
			configuration = new GoogleSignInConfiguration
			{
				WebClientId = webClientId,
				RequestIdToken = true,
				
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

		public override string LoggedInEmail => loggedInEmail;

		string loggedInEmail = "";


		public override void Initialize()
		{
			FB.Init(OnInitialized);
		}


		protected override void OnInitialized()
		{
			FB.ActivateApp();
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
			{
				FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, OnFBLogin);
			}
		}


		void OnFBLogin(ILoginResult result)
		{
			if (IsLoggedIn)
			{
				Debug.Log(result.AccessToken.UserId);
				foreach (var res in result.ResultDictionary)
				{
					Debug.Log(res.Key + " " + res.Value);
				}
				loggedInEmail = "";
				OnLoggedIn(this);
			}
			else
				OnlogginFailed();
		}


		protected override void DoLogout()
		{
			FB.LogOut();
			OnLoggedOut();
		}
	}

	class EmailLogin : LoginBase, IRequireLoginDetails
	{
		public override bool IsInitialized => true;
		public override bool IsLoggedIn => currentUser != null;

		public override string PlatformName => "Email";

		public override string LoggedInEmail => throw new NotImplementedException();

		public string LoginUserName { get; set; }
		public string LoginPassword { get; set; }

		public override void Initialize()
		{

		}


		protected override void DoLogin()
		{
			if (LoginPassword != "" && LoginUserName != "")
			{
				AccountBackend.AuthenticateEmail(LoginUserName, LoginPassword, (user) =>
				{
					OnLoggedIn(this);
				});
			}
			else
				Debug.LogError("Email login failed. Username or password field is empty");
		}


		protected override void DoLogout()
		{
			OnLoggedOut();
		}
	}


	public interface IRequireLoginDetails
	{
		string LoginUserName { get; set; }
		string LoginPassword { get; set; }
	}
}
