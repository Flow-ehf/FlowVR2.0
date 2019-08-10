
//#define BYPASS_LOGIN

using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
	public static bool IsLoggedIn => currentUser != null;

	public static event Action<bool> LoginChanged;

	static FBLogin fbLogin = new FBLogin();
	static GoogleLogin googleLogin = new GoogleLogin();
	static EmailLogin emailLogin = new EmailLogin();

	static LoginBase currentLogin;

	public static AccountBackend.User currentUser;

	public static bool IsLoggingIn { get; private set; }


	//Login menu
	[SerializeField] UIPanel mainMenu;

	public enum LoginMethod
	{
		FB,
		Google,
		Email, 
		Guest,
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
			//Already logged in to non company account, skip to menu
			if (!currentUser.isCompanyAccount)
				LevelLoader.LoadLevel("MainMenu");
		}
	}


	public static LoginBase GetLogin(LoginMethod loginMethod)
	{
		switch (loginMethod)
		{
			default:
				return null;
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
			if (currentLogin != null)
				currentLogin.Logout();
			else
				OnLoggedOut();
		}
	}


	static void OnLoggedIn(LoginBase login)
	{
		Debug.Log("Logged in. Fetching user details");

		currentLogin = login;
		AccountBackend.GetuserDetails(currentLogin.LoggedInEmail, (user) =>
		{
			currentUser = user;
			OnFetchedUserData();
		});
	}


	static void OnFetchedUserData()
	{
		IsLoggingIn = false;

		instance?.LoggedIn.Invoke();
		LoginChanged?.Invoke(true);

		bool firstLogin = PlayerPrefs.GetInt("HasLogin", 0) == 0;

		PlayerPrefs.SetInt("HasLogin", 1);

		if (firstLogin && !currentUser.isSubscribed && !currentUser.isGuest && !currentUser.isCompanyAccount)
			LevelLoader.LoadLevel("BuySubscription");
		else
			LevelLoader.LoadLevel("MainMenu");
	}


	//Todo add data/error info
	static void OnlogginFailed()
	{
		Debug.LogError("Login Canceled/Failed");
		IsLoggingIn = false;
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


	public static void LoginAsGuest()
	{
		if (!IsLoggedIn && !IsLoggingIn)
		{
			currentUser = new AccountBackend.User();
			currentUser.isGuest = true;
			currentUser.displayName = "Guest";
			OnFetchedUserData();
		}
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
				IsLoggingIn = true;
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

	class EmailLogin : LoginBase, IRequireLoginDetails, ICanRegistrer
	{
		public override bool IsInitialized => true;
		public override bool IsLoggedIn => currentUser != null;

		public override string PlatformName => "Email";

		string email;
		public override string LoggedInEmail => email;

		public string LoginUserName { get; set; }
		public string LoginPassword { get; set; }

		public event Action<bool> RegistrationComplete;

		public override void Initialize()
		{

		}


		protected override void DoLogin()
		{
			if (LoginPassword != "" && LoginUserName != "")
			{
				AccountBackend.AuthenticateEmail(LoginUserName, LoginPassword, (user) =>
				{
					if (user != null)
					{
						email = user.email;
						OnLoggedIn(this);
					}
				});
			}
			else
				Debug.LogError("Email login failed. Username or password field is empty");
		}


		protected override void DoLogout()
		{
			OnLoggedOut();
		}


		public void Registrer(string username, string password)
		{
			AccountBackend.RegistrerEmail(username, password, (u) =>
			{
				RegistrationComplete?.Invoke(u != null);
			});
		}
	}


	public interface IRequireLoginDetails
	{
		string LoginUserName { get; set; }
		string LoginPassword { get; set; }
	}

	public interface ICanRegistrer
	{
		event Action<bool> RegistrationComplete;
		void Registrer(string username, string password);
	}
}
