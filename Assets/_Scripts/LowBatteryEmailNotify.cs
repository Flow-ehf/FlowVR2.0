using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBatteryEmailNotify : MonoBehaviour
{
	const string BatteryLvlPref = "BatteryLvl";
	const byte NotifyThreshold = 30;

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
			if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
			{
				byte ctrlBatteryLvl = OVRInput.GetControllerBatteryPercentRemaining();
				byte hmdBatteryLvl = (byte)(OVRManager.batteryLevel * 100f);
				byte batteryPerc = ctrlBatteryLvl < hmdBatteryLvl ? ctrlBatteryLvl : hmdBatteryLvl;

				if (batteryPerc <= NotifyThreshold && oldBatteryLvl > NotifyThreshold)
				{
					EmailClient email = new EmailClient("smtp.gmail.com", 587, "magnus@flow.is", "90.Modelin");
					string subject = "Flow VR: Low power";
					string message = "Either your headset or controller battery level is low, please check this at your earliest convenience\n" +
									 "Headset id: " + HMDUtils.DeviceID;
					email.SendMessage("magnus@flow.is", subject, message);
				}
				oldBatteryLvl = batteryPerc;
			}
			yield return waiter;
		}
	}

	private void OnDestroy()
	{
		PlayerPrefs.SetInt(BatteryLvlPref, oldBatteryLvl);
	}
}
