﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	[ContextMenu("Test")]
	void Test()
	{
		StartCoroutine(PerformTest());
	}

	[ContextMenu("TestLogin")]
	void Test2()
	{
		AccountBackend.AuthenticateEmail("a@abc.com", "TestPassword123", null);
	}

	[ContextMenu("TestCreate")]
	void Test3()
	{
		AccountBackend.RegistrerEmail("a@abs.com", "password123", null);
	}


	IEnumerator PerformTest()
	{
		string endpoint = "http://ec2-52-34-136-26.us-west-2.compute.amazonaws.com:3001/fabric/O7000002973/";
		string function = "isEmailSubscribed";

		string json = "{\"userEmail\":\"a@abc.com\"}";
		byte[] data = new System.Text.UTF8Encoding().GetBytes(json);

		UnityWebRequest request = new UnityWebRequest(endpoint + function, UnityWebRequest.kHttpVerbPOST);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.uploadHandler = new UploadHandlerRaw(data);

		request.SetRequestHeader("Authorization", "Bearer UEn0k5cz/bdm2bRcahPLrlgMFfO2qswnJ5+OF8d1s5XGQWdkBACoiS5a5ukCy4hPgrF5E0VvBd0lU2NLbBMTjuHwSMdM8ISML1l/rZz2VrxCu9J+sknzkvQYUUpVy3n39tg1KZwtK8FNTWOhKxtS/GZgOINda1pOkhNV/g0+VVPSpk1Vp48KLZ8xiCfaaVMgr7aeWd3EmNkJoGqz6wZgOMup2n8Na1vbfAXQu7WfV3vnV4lRALH9e5EUAOPdc6BScbpX4tjTo5CRVLzCsBMIH8S82F/73yUIJzUGCFWuo171lL0JxHN6Jg+Yx4Xplduy12dHsXnX7LYSfCW2NieeKCd+yT3Ac8cn6yIJ/zqCqLPiv4jaL/MIfHmHmjdtE2BUaPrVUO/iIQQaIhvjXAMyhEiV9J1GyDA9sDjwyWKej5csmdm1iZOQKnH+IlpUtMZbRRMT4GTuZ24LUWnVoA4kEspKa3XwHdtcAzNbd0jYvVjM790fLnib4djUVrAP1dG7Ds2/QUafjyLJe5nbKGBOX7lXJ0e32s9cakRN2YgLdCD4XM0lkNC88NHvy4E2UWkZGP7OsqAaHoV8kxNwQ2FSzFf+bvRgqa0mXcwNhdrtBwG5dFYQ3njFemwRr4Lyrx1dYzQqfPJFZEXjQkTBtFYEGI9VSPXgOURzCpOe2ooubmw=");
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if(request.isNetworkError || request.isHttpError)
		{
			Debug.LogError(request.error + " (" + request.responseCode + ")");
			Debug.LogError(request.downloadHandler.text);
		}
		else
		{
			Debug.Log(request.downloadHandler.text);
		}
	}
}
