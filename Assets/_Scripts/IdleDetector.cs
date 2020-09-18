//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR;
//using UnityEngine.SceneManagement;

//public class IdleDetector : MonoBehaviour
//{
//	const float IDLE_TIME = 60;

//	[RuntimeInitializeOnLoadMethod]
//	static void Init()
//	{
//		instance = new GameObject(nameof(IdleDetector)).AddComponent<IdleDetector>();
//		//instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
//		DontDestroyOnLoad(instance.gameObject);
//	}

//	static IdleDetector instance;

//	Coroutine waitLogout;

//	void Start()
//	{
//		OVRManager.HMDMounted += OnHeadsetAdded;
//		OVRManager.HMDUnmounted += OnHeadsetRemoved;
//	}

//	void OnHeadsetRemoved()
//	{ 
//		if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
//			LoginManager.Logout(false);

//		if (waitLogout != null)
//			StopCoroutine(waitLogout);

//		waitLogout = StartCoroutine(WaitLogout());
//	}

//	void OnHeadsetAdded()
//	{
//		if (waitLogout != null)
//			StopCoroutine(waitLogout);
//	}

//	IEnumerator WaitLogout()
//	{
//		yield return new WaitForSeconds(IDLE_TIME);

//		if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
//			LoginManager.Logout(false);
//	}
//}
