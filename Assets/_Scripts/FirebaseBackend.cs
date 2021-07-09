using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Runtime.CompilerServices;

public static class FirebaseBackend
{
	public static bool IsInitialized => app != null;

	private static FirebaseApp app;
	private static FirebaseAuth auth;
	private static FirebaseDatabase db;

	public static void Initialize()
	{
		CheckDependencies();
	}

	static async void CheckDependencies()
	{
		DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

		if (status == DependencyStatus.Available)
		{
			app = FirebaseApp.DefaultInstance;
			auth = FirebaseAuth.DefaultInstance;
			db = FirebaseDatabase.DefaultInstance;

			auth.StateChanged += Auth_StateChanged;
			Auth_StateChanged(null, null);
		}
		else
		{
			Debug.LogError("Failed to resolve FirebaseSDK: " + status);
			app = null;
		}
	}

	private static void Auth_StateChanged(object sender, System.EventArgs e)
	{
		
	}

	private static async Task<bool> SafeTask(Task task, [CallerMemberName] string name = null)
	{
		await task;

		if (task.IsCanceled)
			Debug.LogError($"Firebase " + name + " canceled");
		else if (task.IsFaulted)
			Debug.LogException(task.Exception);
		else
			return true;
		return false;
	}

	private static async Task<T> SafeTask<T>(Task<T> task, [CallerMemberName] string name = null) where T : class
	{
		await task;

		if (task.IsCanceled)
		{
			Debug.LogError($"Firebase " + name + " canceled");
			return null;
		}
		else if (task.IsFaulted)
		{
			Debug.LogException(task.Exception);
			return null;
		}

		return task.Result;
	}

	public static async void AuthenticateAccount(string email, string password, Action<AccountBackend.User> onCompleted)
	{
		if (auth != null)
		{
			FirebaseUser fUser = await SafeTask(auth.SignInWithEmailAndPasswordAsync(email, password));
			AccountBackend.User user = null;

			if (fUser != null)
			{
				user = new AccountBackend.User();
				user.email = fUser.Email;
				user.displayName = fUser.DisplayName;
				user.uid = fUser.UserId;

				await GetUserDetails(user);
			}
			onCompleted?.Invoke(user);
		}
	}

	private static async Task GetUserDetails(AccountBackend.User user)
	{
		DataSnapshot userData = await SafeTask(db.GetReference("users/ac" + user.uid).GetValueAsync());

		if (userData != null && userData.Exists)
		{
			user.isSubscribed = !string.IsNullOrEmpty(userData.Child("userPremiumCode").Value as string);
		}
		else
			Debug.LogError("FirebaseBackend GetUserDetails no userdetails for user " + user.uid);

		string emailDomain = user.email.Substring(user.email.IndexOf('@'));

#if CORPORATE_ACCOUNT_UNLOCKED
		DataSnapshot corpData = await SafeTask(db.GetReference("corporate-contracts").OrderByChild("email").EqualTo(emailDomain).GetValueAsync());

		user.isCompany = corpData != null && corpData.Exists;
#endif
	}

	public static void Logout()
	{
		if (auth != null)
		{
			auth.SignOut();
		}
	}

	public static async void RegisterAccount(string email, string password, string firstName, Action<AccountBackend.User> onCompleted)
	{
		if (auth != null)
		{
			FirebaseUser fUser = await SafeTask(auth.CreateUserWithEmailAndPasswordAsync(email, password));
			
			if (fUser == null)
			{
				Debug.LogError("Failed to signup user. Failed to create user. Aborting");
				onCompleted?.Invoke(null);
			}

			AccountBackend.User user = new AccountBackend.User();
			user.email = fUser.Email;
			user.displayName = fUser.DisplayName;
			user.uid = fUser.UserId;

			string json = JsonUtility.ToJson(new RegisterJSON()
			{
				created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
				userEmail = user.email,
				userFirstName = firstName,
				userId = "ac" + user.uid,
				userLastName = "",
				userPremiumCode = "",
			});

			bool success = await SafeTask(db.GetReference("users/ac" + user.uid).SetRawJsonValueAsync(json));

			if (!success)
			{
				Debug.LogError("Failed to signup user. Failed to post user data. Aborting");
				onCompleted?.Invoke(null);
			}

			Debug.Log("Registered user: " + user.uid);
			onCompleted?.Invoke(user);
		}
	}

	[Serializable]
	class RegisterJSON
	{
		public long created;
		public string userEmail;
		public string userFirstName;
		public string userId;
		public string userLastName;
		public string userPremiumCode;
	}
}
