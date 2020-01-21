using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBatteryEmailNotify : MonoBehaviour
{
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

	// Start is called before the first frame update
	IEnumerator Start()
    {
        while(true)
		{
			if (LoginManager.IsLoggedIn && LoginManager.currentUser.isCompany)
			{
				byte batteryPerc = OVRInput.GetControllerBatteryPercentRemaining();

				if (batteryPerc <= NotifyThreshold && oldBatteryLvl > NotifyThreshold)
				{
					EmailClient email = new EmailClient("smtp.gmail.com", 587, "name@email.com", "password");
					string subject = "Flow VR: Controller low battery";
					string message = "Controller is low battery";
					email.SendMessage(LoginManager.currentUser.email, subject, message);
				}
				oldBatteryLvl = batteryPerc;
			}

			yield return waiter;
		}
    }
}
