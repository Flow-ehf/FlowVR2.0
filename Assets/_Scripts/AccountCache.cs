using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AccountCache : MonoBehaviour
{
	const string FILENAME = "AccountCache.json";

	const string CacheCount = "Cache_Count";
	const string CacheEntry = "Cache_User_";

	static readonly string path = Application.persistentDataPath + "/" + FILENAME;

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		GameObject go = new GameObject("AccountCache");
		go.AddComponent<AccountCache>();
		go.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(go);

#if UNITY_ANDROID
		UnityEngine.Android.Permission.RequestUserPermission("READ_EXTERNAL_STORAGE");
#endif

		Application.quitting += OnQuit;
	}

	private void Awake()
	{
		//#if AccountCachePlayerPrefs

		//#else
		//		if (File.Exists(path))
		//		{
		//			byte[] bytes = File.ReadAllBytes(path);
		//			string data = System.Text.Encoding.ASCII.GetString(bytes);		
		//			cache = JsonUtility.FromJson<Cache>(data);
		//		}
		//#endif
		if (cache == null)
			cache = new Cache();
		if (cache.users == null)
			cache.users = new List<AccountBackend.User>();

		int count = PlayerPrefs.GetInt(CacheCount, 0);
		for (int i = 0; i < count; i++)
		{
			string userData = PlayerPrefs.GetString(CacheEntry + i, null);
			AccountBackend.User user = JsonUtility.FromJson<AccountBackend.User>(userData);
			cache.AddUser(user);
		}

		Debug.Log("Loaded " + Count + " cached users");
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
#if AccountCachePlayerPrefs

#else
		//string data = JsonUtility.ToJson(cache);
		//byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);

		//if (!Directory.Exists(Application.persistentDataPath))
		//	Directory.CreateDirectory(Application.persistentDataPath);
		//if (!File.Exists(path))
		//{
		//	File.CreateText(path);
		//}
		//File.WriteAllBytes(path, bytes);

		PlayerPrefs.SetInt(CacheCount, Count);
		for (int i = 0; i < Count; i++)
		{
			string userData = JsonUtility.ToJson(cache.users[i]);
			PlayerPrefs.SetString(CacheEntry + i, userData);
		}

		Debug.Log("Saved " + Count + " cached accounts to cache");
#endif
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



