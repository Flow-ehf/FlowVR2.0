using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccountBackend : MonoBehaviour
{
	const string EndPoint = "http://ec2-52-34-136-26.us-west-2.compute.amazonaws.com:3001/fabric/";
	const string Token = "UEn0k5cz/bdm2bRcahPLrlgMFfO2qswnJ5+OF8d1s5XGQWdkBACoiS5a5ukCy4hPgrF5E0VvBd0lU2NLbBMTjuHwSMdM8ISML1l/rZz2VrxCu9J+sknzkvQYUUpVy3n39tg1KZwtK8FNTWOhKxtS/GZgOINda1pOkhNV/g0+VVPSpk1Vp48KLZ8xiCfaaVMgr7aeWd3EmNkJoGqz6wZgOMup2n8Na1vbfAXQu7WfV3vnV4lRALH9e5EUAOPdc6BScbpX4tjTo5CRVLzCsBMIH8S82F/73yUIJzUGCFWuo171lL0JxHN6Jg+Yx4Xplduy12dHsXnX7LYSfCW2NieeKCd+yT3Ac8cn6yIJ/zqCqLPiv4jaL/MIfHmHmjdtE2BUaPrVUO/iIQQaIhvjXAMyhEiV9J1GyDA9sDjwyWKej5csmdm1iZOQKnH+IlpUtMZbRRMT4GTuZ24LUWnVoA4kEspKa3XwHdtcAzNbd0jYvVjM790fLnib4djUVrAP1dG7Ds2/QUafjyLJe5nbKGBOX7lXJ0e32s9cakRN2YgLdCD4XM0lkNC88NHvy4E2UWkZGP7OsqAaHoV8kxNwQ2FSzFf+bvRgqa0mXcwNhdrtBwG5dFYQ3njFemwRr4Lyrx1dYzQqfPJFZEXjQkTBtFYEGI9VSPXgOURzCpOe2ooubmw=";

	//createUser
	//authenticateUser
	//isEmailSubscribed
	//isEmailRegistrered
	//getUserDetails

	public static event Action<AccountBackend.Result> Error;


	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(AccountBackend)).AddComponent<AccountBackend>();
		instance.enabled = false;
		//instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	static AccountBackend instance;

	static IEnumerator BackendFunction<T>(string function, Dictionary<string,string> arguments, Action<string> callback) where T: Result, new()
	{
		Uri uri = new Uri(EndPoint + function);
		using (UnityWebRequest request = UnityWebRequest.Post(uri, arguments))
		{
			request.SetRequestHeader("Authorization", "Bearer " + Token);

			yield return request.SendWebRequest();

			if (request.isNetworkError || request.isHttpError)
			{
				Debug.LogError($"[Backend {function}] {request.error} ({request.responseCode})");
				Debug.Log($"[Backend {function}] {request.downloadHandler.text} {uri}");

				var result = new T();
				JsonUtility.FromJsonOverwrite(request.downloadHandler.text, request);
				result.method = Result.GetMethod(function);
				Error?.Invoke(result);
			}
			var json = request.downloadHandler.text;
			Debug.Log($"[Backend {function}] {json}");
			callback?.Invoke(json);
		}
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
		User user = null;
		yield return BackendFunction<AuthResult>("authenticateUser", args, (json) =>
		{
			var r = new AuthResult();
			JsonUtility.FromJsonOverwrite(json, r);
			r.method = Result.Method.Login;

			if (r.GetError() == null)
			{
				user = r.user;
			}
		});
		if (user != null)
			yield return WaitIsSubscribed(email, (subbed) =>
			{
				user.isSubscribed = subbed;
			});
		callback?.Invoke(user);
	}


	public static void IsRegistrered(string email, Action<bool> callback)
	{
		instance.StartCoroutine(WaitIsRegistrered(email, callback));
	}

	public static IEnumerator WaitIsRegistrered(string email, Action<bool> callback)
	{
		Dictionary<string, string> arg = new Dictionary<string, string>
		{
			["userEmail"] = email,
		};
		yield return BackendFunction<MethodResult>("isEmailRegistered", arg, (json) =>
		{
			var r = new MethodResult();
			JsonUtility.FromJsonOverwrite(json, r);
			r.method = Result.Method.UserDetails;
			callback?.Invoke(r.error == null);
		});
	}


	public static void IsSubscribed(string email, Action<bool> callback)
	{
		instance.StartCoroutine(WaitIsSubscribed(email, callback));
	}

	public static IEnumerator WaitIsSubscribed(string email, Action<bool> callback)
	{
		Dictionary<string, string> arg = new Dictionary<string, string>
		{
			["userEmail"] = email,
		};
		yield return BackendFunction<MethodResult>("isEmailSubscribed", arg, (json) =>
		{
			var r = new MethodResult();
			JsonUtility.FromJsonOverwrite(json, r);
			r.method = Result.Method.IsSubscribed;
			callback?.Invoke(r.error == null);
		});
	}


	public static void GetuserDetails(string email, Action<User> callback)
	{
		instance.StartCoroutine(WaitGetUserDetails(email, callback));
	}

	public static IEnumerator WaitGetUserDetails(string email, Action<User> callback)
	{
		Dictionary<string, string> arg = new Dictionary<string, string>
		{
			["userEmail"] = email,
		};
		User user = null;
		yield return BackendFunction<AuthResult>("getUserDetails", arg, (json) =>
		{
			var r = new AuthResult();
			JsonUtility.FromJsonOverwrite(json, r);
			r.method = Result.Method.UserDetails;
			if(r.GetError() == null)
				user = r.user;
		});
		if(user != null)
			yield return WaitIsSubscribed(email, (subbed) => 
			{
				user.isSubscribed = subbed;
			});
		callback?.Invoke(user);
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
		yield return BackendFunction<AuthResult>("createUser", args, (result) =>
		{
			var r = new AuthResult();
			JsonUtility.FromJsonOverwrite(result, r);
			r.method = Result.Method.Registrer;

			User user = r.user;
			callback?.Invoke(user);   
		});
	}


	[System.Serializable]
	public class User
	{
		public bool isSubscribed;
		public bool isCompanyAccount;
		public bool isGuest;
		public long lastLoginAt;
		public string displayName;
		public string email;
		public string uid;
		public string photoUrl;
		public string company;

		//In case api name is different
		public string Company => company;

		public override string ToString()
		{
			return $"subscribed: {isSubscribed}. company: {isCompanyAccount}. email {email}. uid {uid}.";
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

	[System.Serializable]
	public abstract class Result
	{
		public enum Method
		{
			Registrer,
			Login,
			UserDetails,
			IsSubscribed,
			UserExists,
			Unknown,
		}

		public Method method = Method.Unknown;

		public abstract Error GetError();

		[Serializable]
		public class Error
		{
			public enum ErrorCode
			{
				UserNotFound,
				UserAlreadyExists,
				InvalidEmail,
				Unknown,
			}

			public string code;
			public string message;

			public string GetMessage()
			{
				switch (GetCode())
				{
					case ErrorCode.UserNotFound:
						return "Invalid Login";
					case ErrorCode.UserAlreadyExists:
						return "Email already in use";
					default:
						return "There was an error";
				}
			}

			public ErrorCode GetCode()
			{
				switch (code)
				{
					case "auth/wrong-password":
					case "auth/user-not-found":
						return ErrorCode.UserNotFound;
					case "auth/email-already-in-use":
						return ErrorCode.UserAlreadyExists;
					case "Invalid Email":
						return ErrorCode.InvalidEmail;
					default:
						return ErrorCode.Unknown;
				}
			}

			public override string ToString()
			{
				return $"{GetCode()} - {GetMessage()}";
			}
		}

		public static Result.Method GetMethod(string function)
		{
			switch (function)
			{
				case "createUser":
					return Result.Method.Registrer;
				case "authenticateUser":
					return Result.Method.Login;
				case "isEmailSubscribed":
					return Result.Method.IsSubscribed;
				case "isEmailRegistrered":
					return Result.Method.UserExists;
				case "getUserDetails":
					return Result.Method.UserDetails;
				default:
					return Result.Method.Unknown;
			}
		}

	}

	[System.Serializable]
	public class MethodResult : Result
	{
		public string error;
		public string userEmail;

		public MethodResult() : base()
		{

		}

		public override Error GetError()
		{
			if (error == null || error == "")
				return null;
			else
				return new Error()
				{
					code = error,
				};		
		}
	}

	[System.Serializable]
	public class AuthResult : Result
	{

		public AuthResult() : base()
		{

		}

		public Result.Error error = null;
		public User user = null;

		public override Error GetError()
		{
			if (error.code == null || error.code == "")
				return null;
			else
				return error;
		}

		public override string ToString()
		{
			string s = $"[{method}] ";
			if (error != null)
			{
				s += Environment.NewLine;
				s += error.ToString();
			}
			if (user != null)
			{
				s += Environment.NewLine;
				s += user.ToString();
			}
			return s;
		}
	}
}
