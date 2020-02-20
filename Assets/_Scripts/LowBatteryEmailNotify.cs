using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBatteryEmailNotify : MonoBehaviour
{
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

	// Start is called before the first frame update
	IEnumerator Start()
	{
		while (true)
		{
			//if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
			//{
			byte batteryPerc = OVRInput.GetControllerBatteryPercentRemaining();

			//	if (batteryPerc <= NotifyThreshold && oldBatteryLvl > NotifyThreshold)
			//	{
			EmailClient email = new EmailClient("smtp.gmail.com", 587, "magnus@flow.is", "90.Modelin");
			string subject = "Flow VR: Low power";
			string message = "Either your headset or controller battery level is low, please check this at your earliest convenience";
			email.SendMessage("magnus@flow.is", subject, message);
			//	}
			oldBatteryLvl = batteryPerc;
			//}

			yield return waiter;
		}
	}
}
