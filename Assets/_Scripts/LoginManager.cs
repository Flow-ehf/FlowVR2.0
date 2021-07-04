
//#define BYPASS_LOGIN

using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using Facebook.Unity;
//using Google;


public class LoginManager : MonoBehaviour
{
	//Duration before login cache is invalid, in seconds
	const int LOGIN_CACHE_LIFETIME = int.MaxValue;

	public UnityEvent LoggedIn;
	public UnityEvent LoggedOut;

	public bool useLastLogin = false;

	static LoginManager instance;
	//Any platform currently logged in
	public static bool IsLoggedIn => currentUser != null;

	public static event Action<bool> LoginChanged;

//	static FBLogin fbLogin = new FBLogin();
//	static GoogleLogin googleLogin = new GoogleLogin();
	static EmailLogin emailLogin = new EmailLogin();
	static FirebaseLogin firebaseLogin = new FirebaseLogin();

	static LoginBase currentLogin;
	static bool isStartUp = true;

	public static AccountBackend.User currentUser;

	public static bool IsLoggingIn { get; private set; }
	public static bool StartAtLogin { get; set; }

	[SerializeField] UIPanel loginPanel;

	public enum LoginMethod
	{
//		FB,
//		Google,
		Firebase,
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
		//		fbLogin.Initialize();
		//		googleLogin.Initialize();
		firebaseLogin.Initialize();

		if (isStartUp)
		{
			if (useLastLogin)
			{
				currentUser = AccountCache.GetLastLogin();

				//User logged in previously
				if (currentUser != null)
				{
					Debug.Log("Found cached login: " + currentUser);

					OnFetchedUserData();

					//Previously logged in to company account, show account selection
					//if (currentUser.isCompany && AccountCache.Count > 0)
					//	LevelLoader.LoadLevel("CompanyAccountSelection", false);
					//else
					//	LevelLoader.LoadLevel("MainMenu", false);
				}
			}
			isStartUp = false;
		}
		else
		{
			if (StartAtLogin)
			{
				loginPanel.SetActiveImmediately(true);
				StartAtLogin = false;
			}
		}
	}


	public static LoginBase GetLogin(LoginMethod loginMethod)
	{
		switch (loginMethod)
		{
			default:
				return null;
//			case LoginMethod.FB:
//				return fbLogin;
//			case LoginMethod.Google:
//				return googleLogin;
			case LoginMethod.Email:
				return emailLogin;
			case LoginMethod.Firebase:
				return firebaseLogin;
		}
	}


	public static void Logout(bool removeFromCache)
	{
		if (IsLoggedIn)
		{
			if (currentLogin != null)
				currentLogin.Logout(removeFromCache);
			else
				OnLoggedOut(removeFromCache);
		}
	}


	static void OnLoggedIn(LoginBase login)
	{
		Debug.Log("Logged in. Fetching user details");

		currentLogin = login;
		if (currentUser != null)
			OnFetchedUserData();
		else
			OnLogginFailed();
	}


	static void OnFetchedUserData()
	{
		IsLoggingIn = false;

		instance?.LoggedIn.Invoke();
		LoginChanged?.Invoke(true);
		AccountCache.AddToCache(currentUser);

		bool firstLogin = PlayerPrefs.GetInt("HasLogin", 0) == 0;

		PlayerPrefs.SetInt("HasLogin", 1);

		if (firstLogin && !currentUser.isSubscribed && !currentUser.isGuest)
			LevelLoader.LoadLevel("BuyDLC");
		else if(AccountCache.Count > 0 && currentUser.isCompany && LevelLoader.Level != "CompanyAccountSelection")
			LevelLoader.LoadLevel("CompanyAccountSelection", false);
		else
			LevelLoader.LoadLevel("MainMenu");
	}


	//Todo add data/error info
	static void OnLogginFailed()
	{
		Debug.LogError("Login Canceled/Failed");
		IsLoggingIn = false;
	}


	static void OnLoggedOut(bool removeFromCache)
	{
		instance?.LoggedOut.Invoke();
		LoginChanged?.Invoke(false);

		if(removeFromCache)
			AccountCache.RemoveFromCache(currentUser);

		bool wasCompanyAccount = currentUser.isCompany;

		currentLogin = null;
		currentUser = null;

		LoginManager.StartAtLogin = true;

		//Return to login screen when logging out
	//	if(wasCompanyAccount)
	//	{
	//		LevelLoader.LoadLevel("CompanyAccountSelection");
	//	}
	//	else
	//	{
	//		if (LevelLoader.Level != "LoginMenu" && LevelLoader.Level != "CompanyAccountSelection")
	//			LevelLoader.LoadLevel("LoginMenu");
	//	}
	}


	public static void LoginAsGuest()
	{
		var guestUser = new AccountBackend.User();
		guestUser.isGuest = true;
		guestUser.displayName = "Guest";
		LoginAsUser(guestUser);
	}


	public static void LoginAsUser(AccountBackend.User user)
	{
		if (!IsLoggingIn)
		{
			if (IsLoggedIn)
				Logout(false);
			currentUser = user;
			OnFetchedUserData();
		}
	}


	public abstract class LoginBase
	{
		public abstract bool IsInitialized { get; }
		public abstract bool IsLoggedIn { get; }
		public abstract string PlatformName { get; }
		public abstract string LoggedInEmail { get; }

		public ILoginDetails LoginDetails { protected get; set; }

		public event Action Initalized;

		public void Login()
		{
			//No platform is currently logged in and this platform is ready for login
			if (!LoginManager.IsLoggingIn && IsInitialized)
			{
				if (IsLoggedIn)
					Logout(false);

				IsLoggingIn = true;
#if UNITY_EDITOR && BYPASS_LOGIN
				OnLoggedIn(this);
#else
				DoLogin();
#endif

			}
		}

		public void Logout(bool removeFromCache)
		{
			if (IsLoggedIn)
				DoLogout(removeFromCache);
		}

		public abstract void Initialize();
		protected abstract void DoLogin();
		protected abstract void DoLogout(bool removeFromCache);

		protected virtual void OnInitialized()
		{
			Debug.Log(PlatformName + ": login ready!");

			Initalized?.Invoke();
		}
	}

	//	class GoogleLogin : LoginBase
	//	{
	//		const string webClientId = "";

	//		public override bool IsInitialized => isInitialized;

	//		public override bool IsLoggedIn => loggedInUser != null;

	//		public override string PlatformName => "Google";

	//		public override string LoggedInEmail => loggedInUser?.Email ?? "";

	//		bool isInitialized;
	////		GoogleSignInConfiguration configuration;
	////		GoogleSignInUser loggedInUser;

	//		public override void Initialize()
	//		{
	//			configuration = new GoogleSignInConfiguration
	//			{
	//				WebClientId = webClientId,
	//				RequestIdToken = true,

	//			};
	//			isInitialized = true;
	//			OnInitialized();
	//		}


	//		protected override void DoLogin()
	//		{
	//			GoogleSignIn.Configuration = configuration;
	//			GoogleSignIn.Configuration.UseGameSignIn = false;
	//			GoogleSignIn.Configuration.RequestIdToken = true;

	//			GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnLogin);
	//		}


	//		protected override void DoLogout()
	//		{
	//			loggedInUser = null;
	//			GoogleSignIn.DefaultInstance.SignOut();
	//			OnLoggedOut();
	//		}


	//		//void OnLogin(Task<GoogleSignInUser> result)
	//		//{
	//		//	if (result.IsFaulted || result.IsCanceled)
	//		//		OnlogginFailed();
	//		//	else
	//		//	{
	//		//		loggedInUser = result.Result;
	//		//		OnLoggedIn(this);
	//		//	}
	//		//}
	//	}

	//class FBLogin : LoginBase
	//{
	//	public override bool IsInitialized => FB.IsInitialized;
	//	public override bool IsLoggedIn => FB.IsLoggedIn;

	//	public override string PlatformName { get; } = "Facebook";

	//	public override string LoggedInEmail => loggedInEmail;

	//	string loggedInEmail = "";


	//	public override void Initialize()
	//	{
	//		FB.Init(OnInitialized);
	//	}


	//	protected override void OnInitialized()
	//	{
	//		FB.ActivateApp();
	//	}


	//	protected override void DoLogin()
	//	{
	//		if (!IsInitialized)
	//		{
	//			Debug.LogError("Login failed! FB not initialized.");
	//			OnlogginFailed();
	//		}
	//		else if (LoginManager.IsLoggedIn)
	//		{
	//			Debug.LogError("Login failed! Already logged in");
	//			OnlogginFailed();
	//		}
	//		else
	//		{
	//			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, OnFBLogin);
	//		}
	//	}


	//void OnFBLogin(ILoginResult result)
	//{
	//	if (IsLoggedIn)
	//	{
	//		Debug.Log(result.AccessToken.UserId);
	//		foreach (var res in result.ResultDictionary)
	//		{
	//			Debug.Log(res.Key + " " + res.Value);
	//		}
	//		loggedInEmail = "";
	//		OnLoggedIn(this);
	//	}
	//	else
	//		OnlogginFailed();
	//}


	//	protected override void DoLogout()
	//	{
	//	FB.LogOut();
	//		OnLoggedOut();
	//	}
	//}

	class FirebaseLogin : LoginBase, IRegisterHandler
	{
		public override bool IsInitialized => FirebaseBackend.IsInitialized;

		public override bool IsLoggedIn => currentUser != null;

		public override string PlatformName => "Email";

		public override string LoggedInEmail => currentUser?.email ?? "";

		public override void Initialize()
		{
			FirebaseBackend.Initialize();
		}

		public void Registrer(ILoginDetails details, Action<bool> onRegistered)
		{
			FirebaseBackend.RegisterAccount(details.Username, details.Password, (user) =>
			{
				onRegistered?.Invoke(user != null);
			});
		}

		protected override void DoLogin()
		{
			FirebaseBackend.AuthenticateAccount(LoginDetails.Username, LoginDetails.Password, (user) =>
			{
				if (user != null)
				{
					currentUser = user;
				}
				OnLoggedIn(this);
			});
		}

		protected override void DoLogout(bool removeFromCache)
		{
			FirebaseBackend.Logout();
			OnLoggedOut(removeFromCache);
		}
	}

	class EmailLogin : LoginBase, IRegisterHandler
	{
		public override bool IsInitialized => true;
		public override bool IsLoggedIn => currentUser != null;

		public override string PlatformName => "Email";

		public override string LoggedInEmail => currentUser?.email ?? "";

		public override void Initialize()
		{

		}


		protected override void DoLogin()
		{
			if (LoginDetails != null)
			{
				AccountBackend.AuthenticateEmail(LoginDetails.Username, LoginDetails.Password, (user) =>
				{
					Debug.Log(user);
					if (user != null)
					{
						currentUser = user;
					}
					OnLoggedIn(this);
				});
			}
			else
				Debug.LogError("Email login failed. Username or password field is empty");
		}


		protected override void DoLogout(bool removeFromCache)
		{
			OnLoggedOut(removeFromCache);
		}


		public void Registrer(ILoginDetails details, Action<bool> onRegistered)
		{
			AccountBackend.RegistrerEmail(details.Username, details.Password, (u) =>
			{
				onRegistered?.Invoke(u != null);
			});
		}
	}
}

public interface ILoginDetails
{
	string Username { get; }
	string Password { get; }
	string FirstName { get; }
	string LastName { get; }
}

public interface IRegisterHandler
{
	void Registrer(ILoginDetails details, Action<bool> onRegistered);
}
