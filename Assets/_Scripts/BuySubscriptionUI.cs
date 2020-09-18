//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class BuySubscriptionUI : MonoBehaviour
//{
//	[SerializeField] Button buySubscriptionButton;
//	[SerializeField] Button continueButton;

//    // Start is called before the first frame update
//    void Start()
//    {
//		if (buySubscriptionButton != null)
//			buySubscriptionButton.onClick.AddListener(OnClickBuySubs);
//		if (continueButton != null)
//			continueButton.onClick.AddListener(OnClickContinue);
//    }


//    void OnClickBuySubs()
//	{
//		OnClickContinue();
//	}


//	void OnClickContinue()
//	{
//		LevelLoader.LoadLevel("MainMenu");
//	}
//}
