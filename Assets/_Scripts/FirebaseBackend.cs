using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public static class FirebaseBackend
{
	public static bool IsInitialized => app != null;

	private static FirebaseApp app;
	private static FirebaseAuth auth;

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
		throw new System.NotImplementedException();
	}

	public static void AuthenticateAccount(string email, string password, Action<AccountBackend.User> onCompleted)
	{
		if (auth != null)
		{
			auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith((task) =>
			{
				AccountBackend.User user = null;

				if (task.IsCanceled)
					Debug.LogError($"Firebase login canceled");
				else if (task.IsFaulted)
					Debug.LogException(task.Exception);
				else
				{
					FirebaseUser fUser = task.Result;
					user = new AccountBackend.User();
					user.email = fUser.Email;
					user.displayName = fUser.DisplayName;
					user.uid = fUser.UserId;
				}
				onCompleted?.Invoke(user);
			});
		}
	}

	public static void Logout()
	{
		if (auth != null)
		{
			auth.SignOut();
		}
	}

	public static void RegisterAccount(string email, string password, Action<AccountBackend.User> onCompleted)
	{
		if (auth != null)
		{
			auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith((task) =>
			{
				AccountBackend.User user = null;

				if (task.IsCanceled)
					Debug.LogError($"Firebase login canceled");
				else if (task.IsFaulted)
					Debug.LogException(task.Exception);
				else
				{
					FirebaseUser fUser = task.Result;
					user = new AccountBackend.User();
					user.email = fUser.Email;
					user.displayName = fUser.DisplayName;
					user.uid = fUser.UserId;
				}
				onCompleted?.Invoke(user);
			});
		}
	}
}
