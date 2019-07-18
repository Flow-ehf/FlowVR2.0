using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccountBackend : MonoBehaviour
{
	const string EndPoint = "http://ec2-52-34-136-26.us-west-2.compute.amazonaws.com:3001/fabric/O7000002973/";
	const string Token = "UEn0k5cz/bdm2bRcahPLrlgMFfO2qswnJ5+OF8d1s5XGQWdkBACoiS5a5ukCy4hPgrF5E0VvBd0lU2NLbBMTjuHwSMdM8ISML1l/rZz2VrxCu9J+sknzkvQYUUpVy3n39tg1KZwtK8FNTWOhKxtS/GZgOINda1pOkhNV/g0+VVPSpk1Vp48KLZ8xiCfaaVMgr7aeWd3EmNkJoGqz6wZgOMup2n8Na1vbfAXQu7WfV3vnV4lRALH9e5EUAOPdc6BScbpX4tjTo5CRVLzCsBMIH8S82F/73yUIJzUGCFWuo171lL0JxHN6Jg+Yx4Xplduy12dHsXnX7LYSfCW2NieeKCd+yT3Ac8cn6yIJ/zqCqLPiv4jaL/MIfHmHmjdtE2BUaPrVUO/iIQQaIhvjXAMyhEiV9J1GyDA9sDjwyWKej5csmdm1iZOQKnH+IlpUtMZbRRMT4GTuZ24LUWnVoA4kEspKa3XwHdtcAzNbd0jYvVjM790fLnib4djUVrAP1dG7Ds2/QUafjyLJe5nbKGBOX7lXJ0e32s9cakRN2YgLdCD4XM0lkNC88NHvy4E2UWkZGP7OsqAaHoV8kxNwQ2FSzFf+bvRgqa0mXcwNhdrtBwG5dFYQ3njFemwRr4Lyrx1dYzQqfPJFZEXjQkTBtFYEGI9VSPXgOURzCpOe2ooubmw=";

	//createUser
	//authenticateUser
	//isEmailSubscribed
	//isEmailRegistrered
	//getUserDetails

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(AccountBackend)).AddComponent<AccountBackend>();
		instance.enabled = false;
		//instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	static AccountBackend instance;

	static IEnumerator BackendFunction(string function, Dictionary<string,string> arguments, Action<string> callback)
	{
		Uri uri = new Uri(EndPoint + function);
		UnityWebRequest request = UnityWebRequest.Post(uri, arguments);

		request.SetRequestHeader("Authorization", "Bearer " + Token);

		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Debug.Log(request.downloadHandler.text + " " + uri);
			Debug.LogError(request.error + " (" + request.responseCode + ")");
		}
		else
		{
			callback?.Invoke(request.downloadHandler.text);
		}
		request.Dispose();
	}


	public static void AuthenticateEmail(string email, string password, Action<User> callback)
	{
		instance.StartCoroutine(WaitAuthenticateEmail(email, password, callback));
	}


	public static IEnumerator WaitAuthenticateEmail(string email, string password, Action<User> callback)
	{
		Dictionary<string, string> args = new Dictionary<string, string>
		{
			["userEmail"] = email,
			["userPassword"] = password,
		};
		yield return BackendFunction("authenticateUser", args, (result) =>
		{
			Debug.Log(result);
			callback?.Invoke(null);
		});
	}


	public static void RegistrerEmail(string email, string password, Action<User> callback)
	{
		instance.StartCoroutine(WaitRegistrerEmail(email, password, callback));
	}


	public static IEnumerator WaitRegistrerEmail(string email, string password, Action<User> callback)
	{
		Dictionary<string, string> args = new Dictionary<string, string>
		{
			["userEmail"] = email,
			["userPassword"] = password,
		};
		yield return BackendFunction("createUser", args, (result) =>
		{
			Debug.Log(result);
			callback?.Invoke(null);
		});
	}

	public static void FetchUserDetails(string email, Action<User> callback)
	{
		instance.StartCoroutine(WaitFetchUserDetails(email, callback));
	}

	public static IEnumerator WaitFetchUserDetails(string email, Action<User> callback)
	{
		Dictionary<string, string> args = new Dictionary<string, string>
		{
			["userEmail"] = email,
		};

		yield return BackendFunction("getUserDetails", args, (result) =>
		{
			Debug.Log(result);
		});
		yield return BackendFunction("isEmailSubscribed", args, (result) =>
		{
			Debug.Log(result);
		});

		User user = new User(false, false, "");

		callback?.Invoke(user);
	}


	public class User
	{
		bool isSubscribed;
		bool isGuest;
		long lastLoginTime;
		string email;

		public bool IsSubscribed => isSubscribed;
		public bool IsGuest => isGuest;
		public DateTime LastLogin => new DateTime(lastLoginTime);

		public User(bool isSubscribed, bool isGuest, string email)
		{
			this.isSubscribed = isSubscribed;
			this.isGuest = isGuest;
			this.email = email;
			this.lastLoginTime = DateTime.UtcNow.Ticks;
		}

		public override string ToString()
		{
			return $"subscribed: {IsSubscribed}. last login: {LastLogin}";
		}
	}


	public static bool IsEmailFormat(string email)
	{
		string text = email.Trim();
		//Email is minimum 5 characters (a@b.c)
		if (text.Length < 5)
			return false;
		//must have an '@' at min second character
		int at = text.IndexOf('@');
		if (at < 1)
			return false;
		//Must have a period 
		int period = text.IndexOf('.');
		//Period must be after at +1 character and must have character after it
		if (period <= at + 1 || period == text.Length - 1)
			return false;
		return true;
	}
}
