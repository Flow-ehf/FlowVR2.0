//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using Facebook.Unity;

//public class FBHolder : MonoBehaviour
//{
//	public bool notLoggedIn;

//	void Awake()
//	{
//		FB.Init(SetInit);
//	}

//	private void SetInit()
//	{
//		Debug.Log("FB init done");

//		if (FB.IsLoggedIn)
//		{
//			Debug.Log("FB logged in");
//			SceneManager.LoadScene("MainMenu");
//		}
//		else
//		{
//			Debug.Log("FB not logged in");
//		}
//	}

//	public void Anonlogin()
//	{
//		SceneManager.LoadScene("MainMenu");
//	}

//	public void FBlogin()
//	{
//		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallback);
//	}

//	void AuthCallback(ILoginResult result)
//	{
//		if (FB.IsLoggedIn)
//		{
//			Debug.Log("FB login worked");
//			SceneManager.LoadScene("MainMenu");
//		}
//		else
//		{
//			Debug.Log("FB login failed");
//		}
//	}
//}
