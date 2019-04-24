using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Platform;
using Oculus.Platform.Models;

[RequireComponent(typeof(Button))]
public class LevelSelectButton : MonoBehaviour
{
	const float OwnershipCheckInterval = 5;

	[SerializeField] LoadLevelButton targetButton;
	[SerializeField] string level;
	[SerializeField] string productId;
	[SerializeField] bool editorIgnoreOwnership = false;

	Button button;

	void Awake()
	{
		if (targetButton == null)
			targetButton = FindObjectOfType<LoadLevelButton>();

		button = GetComponent<Button>();
		button.interactable = false;
	}


	void OnEnable()
	{
#if UNITY_EDITOR
		if (editorIgnoreOwnership)
			ProductOwned();
		else
#endif
		{
			if (productId == "")
				ProductOwned();
			else
				StartCoroutine(WaitCheckOwnership());
		}
	}	


	IEnumerator WaitCheckOwnership()
	{
		while(enabled)
		{
			IAP.GetViewerPurchases().OnComplete(OnFetchedPurchases);
			yield return new WaitForSeconds(OwnershipCheckInterval);
		}
	}


	void OnFetchedPurchases(Message<PurchaseList> msg)
	{
		if (msg.IsError)
			Debug.LogError("Failed to fetch dlc purchase");
		else
		{
			foreach (var purchase in msg.GetPurchaseList())
			{
				if(purchase.Sku == productId)
				{
					ProductOwned();
					break;
				}
			}
		}
	}


	void ProductOwned()
	{
		button.interactable = true;
		button.onClick.AddListener(ClickedButton);
		//No need to keep checking ownership
		enabled = false;
	}


	void ClickedButton()
	{
		if (targetButton != null)
			targetButton.SetTargetLevel(level);
	}




    void Reset()
	{
		level = name;
	}
}
