using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AccountCache : MonoBehaviour
{
	const string FILENAME = "AccountCache.json";

	static readonly string path = Path.Combine(Application.persistentDataPath, FILENAME);

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		GameObject go = new GameObject("AccountCache");
		go.AddComponent<AccountCache>();
		go.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(go);

		Application.quitting += OnQuit;
	}

	private void Awake()
	{
		if (File.Exists(path))
		{
			string data = File.ReadAllText(path);			
			cache = JsonUtility.FromJson<Cache>(data);
		}
		if (cache == null)
			cache = new Cache();
		if (cache.users == null)
			cache.users = new List<AccountBackend.User>();

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
		Save();
	}

	public static void RemoveFromCache(AccountBackend.User user)
	{
		cache.RemoveUser(user);
		Save();
	}

	public static int Count => cache.users.Count;

	private static void OnQuit()
	{
		Save();
	}

	public static void Save()
	{
		string data = JsonUtility.ToJson(cache);

		if (!Directory.Exists(Application.persistentDataPath))
			Directory.CreateDirectory(Application.persistentDataPath);
		if (!File.Exists(path))
			File.Create(path).Close();

		File.WriteAllText(path, data, System.Text.Encoding.ASCII);
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



