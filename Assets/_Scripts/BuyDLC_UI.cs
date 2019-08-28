using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Platform;
using Oculus.Platform.Models;

public class BuyDLC_UI : MonoBehaviour
{
	[Header("DLC identifiers go here")]
	[SerializeField] string[] dlcSKUValues;
	[Space]
	[SerializeField] RectTransform container;
	[SerializeField] RectTransform prefabOrTemplate;

	Dictionary<string, DLCButton> buttons = new Dictionary<string, DLCButton>();

	public static string targetDlcSKU = "";

    // Start is called before the first frame update
    void Start()
    {
		for (int i = container.childCount - 1; i >= 0; i--)
		{
			Destroy(container.GetChild(i).gameObject);
		}

		if (dlcSKUValues.Length > 0 && container != null && prefabOrTemplate != null)
			IAP.GetProductsBySKU(dlcSKUValues).OnComplete(OnRetrievedProductList);
    }

    void OnRetrievedProductList(Message<ProductList> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to retrieve DLC list: " + result.GetError());
		}
		else
		{
			foreach (var dlc in result.GetProductList())
			{
				GameObject newEntry = Instantiate(prefabOrTemplate.gameObject, container);
				newEntry.SetActive(true);
				DLCButton info = newEntry.GetComponent<DLCButton>();
				info.nameText?.UpdateText(dlc.Name);
				info.priceText?.UpdateText(dlc.FormattedPrice);
				info.descriptionText?.UpdateText(dlc.Description);
				info.sku = dlc.Sku;
				info.buyButton.interactable = false;
				info.buyButton.onClick.AddListener(() => Buy(info));
				buttons.Add(dlc.Sku, info);
			}
			IAP.GetViewerPurchases().OnComplete(OnRetrieveExistingPurchases);
		}
	}

	void OnRetrieveExistingPurchases(Message<PurchaseList> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to retrieve purchased dLDC list: " + result.GetError());
		}
		else
		{
			foreach (var info in buttons.Values)
			{
				info.buyButton.interactable = true;
			}
			foreach (var dlc in result.GetPurchaseList())
			{
				UpdatePurchase(dlc.Sku);
			}

			if(targetDlcSKU != "")
			{
				Buy(buttons[targetDlcSKU]);
				targetDlcSKU = "";
			}
		}
	}

	void Buy(DLCButton button)
	{
		IAP.LaunchCheckoutFlow(button.sku).OnComplete(Purchased);
		button.buyButton.interactable = false;
	}

	void Purchased(Message<Purchase> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to buy DLC: " + result.GetError());
		}
		else
		{
			var purchase = result.GetPurchase();
			UpdatePurchase(purchase.Sku);
		}
	}


	void UpdatePurchase(string sku)
	{
		buttons.TryGetValue(sku, out DLCButton info);
		if (info == null)
			Debug.LogError($"SKU '{sku}' not found included in build");
		else
		{
			info.buyButton.interactable = false;
			info.priceText?.UpdateText("<purchased>");
		}
	}
}
