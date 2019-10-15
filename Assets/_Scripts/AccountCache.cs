using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AccountCache 
{
	const string FILENAME = "AccountCache.json";
	static readonly string path = Path.Combine(Application.persistentDataPath, FILENAME);

	static AccountCache()
	{
		if (File.Exists(path))
		{
			string data = File.ReadAllText(path);
			cache = JsonUtility.FromJson<Cache>(data);
		}
		if(cache == null)
			cache = new Cache();
		if (cache.users == null)
			cache.users = new List<AccountBackend.User>();

		Application.quitting += OnQuit;
	}

	public static AccountBackend.User GetLastLogin()
	{
		if (Count == 0)
			return null;

		int index = 0;
		long lastLogin = 0;
		for (int i = 0; i < Count; i++)
		{
			if(cache.users[i].lastLoginAt > lastLogin)
			{
				lastLogin = cache.users[i].lastLoginAt;
				index = i;
			}
		}
		return cache.users[index];
	}

	public static AccountBackend.User GetLoginAtIndex(int index)
	{
		if (index < 0 || index >= Count)
			return null;

		return cache.users[index];
	}

	public static void AddToCache(AccountBackend.User user)
	{
		if (user.isGuest)
			return;

		cache.AddUser(user);
	}

	public static void RemoveFromCache(AccountBackend.User user)
	{
		cache.RemoveUser(user);
	}

	public static int Count => cache.users.Count;

	private static void OnQuit()
	{
		Save();
	}

	public static void Save()
	{
		string data = JsonUtility.ToJson(cache);
		File.WriteAllText(path, data);
	}


	static Cache cache;

	[System.Serializable]
    class Cache
	{
		public List<AccountBackend.User> users;

		public void AddUser(AccountBackend.User user)
		{
			int index = users.FindIndex((u) => u.uid == user.uid);
			if (index > -1)
			{
				users[index] = user;
			}
			else
			{
				users.Add(user);
			}
		}

		public void RemoveUser(AccountBackend.User user)
		{
			int index = users.FindIndex((u) => u.uid == user.uid);
			if (index > -1)
				users.RemoveAt(index);
		}
	}
}



