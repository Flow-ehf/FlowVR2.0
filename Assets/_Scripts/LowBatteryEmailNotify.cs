using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBatteryEmailNotify : MonoBehaviour
{
	const string BatteryLvlPref = "BatteryLvl";
	const byte NotifyThreshold = 15;

	static LowBatteryEmailNotify instance;

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		instance = new GameObject(nameof(LowBatteryEmailNotify)).AddComponent<LowBatteryEmailNotify>();
		//instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(instance.gameObject);
	}

	WaitForSeconds waiter = new WaitForSeconds(5);
	byte oldBatteryLvl;

	IEnumerator Start()
	{
		oldBatteryLvl = (byte)PlayerPrefs.GetInt(BatteryLvlPref);

		while (true)
		{
			//if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
			//{
#if UNITY_EDITOR || UNITY_STANDALONE

			byte ctrlBatteryLvl;
			if (OVRInput.IsControllerConnected(OVRInput.Controller.Touch))
				ctrlBatteryLvl = (byte)Mathf.Min(
											OVRInput.GetControllerBatteryPercentRemaining(OVRInput.Controller.LTouch),
											OVRInput.GetControllerBatteryPercentRemaining(OVRInput.Controller.RTouch));
			else
				ctrlBatteryLvl = 100;
#else
			if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
				ctrlBatteryLvl = OVRInput.GetControllerBatteryPercentRemaining(OVRInput.Controller.LTrackedRemote);
			else if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
				ctrlBatteryLvl = OVRInput.GetControllerBatteryPercentRemaining(OVRInput.Controller.RTrackedRemote);
			else
				ctrlBatteryLvl = 100;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
			byte hmdBatteryLvl = 100;
#else
			byte hmdBatteryLvl = (byte)(OVRManager.batteryLevel * 100f);
#endif
			byte batteryPerc = (byte)Mathf.Min(ctrlBatteryLvl, hmdBatteryLvl);

			if (batteryPerc <= NotifyThreshold && oldBatteryLvl > NotifyThreshold)
			{
				EmailClient email = new EmailClient("smtp.gmail.com", 587, "batteryatflow@gmail.com", "gowiththeflow");
				string subject = "Flow VR: Low power";
				string message = "Either your headset or controller battery level is low, please check this at your earliest convenience\n" +
								 "Headset id: " + HMDUtils.DeviceID + " Charge:" + batteryPerc;
				email.SendMessage("batteryatflow@gmail.com", subject, message);
			}

			if (oldBatteryLvl > NotifyThreshold && oldBatteryLvl <= NotifyThreshold)
			{
				EmailClient email = new EmailClient("smtp.gmail.com", 587, "batteryatflow@gmail.com", "gowiththeflow");
				string subject = "Flow VR: Power fixed";
				string message = "This headset has power now \n" +
								 "Headset id: " + HMDUtils.DeviceID + " Charge:" + batteryPerc; ;
				email.SendMessage("batteryatflow@gmail.com", subject, message);
			}
			oldBatteryLvl = batteryPerc;
			//}
			yield return waiter;
		}
	}

	private void OnDestroy()
	{
		PlayerPrefs.SetInt(BatteryLvlPref, oldBatteryLvl);
	}
}
