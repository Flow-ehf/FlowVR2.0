using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	[SerializeField] int perPageCapacity = 7;
	[SerializeField] Button nextPage, previousPage;

	Dictionary<string, DLCButton> buttons = new Dictionary<string, DLCButton>();
	int currentPage = 0;

	List<Product> dlcs = new List<Product>();

	public static string targetDlcSKU = "";

    // Start is called before the first frame update
    void Start()
    {
		for (int i = container.childCount - 1; i >= 0; i--)
		{
			Destroy(container.GetChild(i).gameObject);
		}

		nextPage.interactable = false;
		previousPage.interactable = false;

		if (dlcSKUValues.Length > 0 && container != null && prefabOrTemplate != null)
			IAP.GetProductsBySKU(dlcSKUValues).OnComplete(OnRetrievedProductList);
    }

    void OnRetrievedProductList(Message<ProductList> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to retrieve DLC list: " + result.GetError().Message);
		}
		else
		{
			dlcs = result.GetProductList().ToList();

			for (int i = 0; i < dlcs.Count; i++)
			{
				Product dlc = dlcs[i];

				GameObject newEntry = Instantiate(prefabOrTemplate.gameObject, container);
				newEntry.SetActive(true);
				DLCButton info = newEntry.GetComponent<DLCButton>();
				info.nameText?.UpdateText(dlc.Name);
				info.priceText?.UpdateText(dlc.FormattedPrice);
				info.descriptionText?.UpdateText(dlc.Description);
				info.sku = dlc.Sku;
				info.buyButton.onClick.RemoveAllListeners();
				info.buyButton.onClick.AddListener(() => Buy(dlc));
				buttons[dlc.Sku] = info;
			}
			IAP.GetViewerPurchases().OnComplete(OnRetrieveExistingPurchases);
		}
	}

	void GoNextPage()
	{
		BuildDLCPage(currentPage + 1);
	}

	void GoPreviousPage()
	{
		BuildDLCPage(currentPage - 1);
	}

	void BuildDLCPage(int page)
	{
		for (int i = container.childCount - 1; i >= 0; i--)
		{
			container.GetChild(i).gameObject.SetActive(false);
		}

		int maxPage = dlcs.Count / perPageCapacity;

		if (page < 0)
			currentPage = maxPage;
		else if (maxPage > 0)
			currentPage = page % maxPage;
		else
			currentPage = 0;

		nextPage.interactable = maxPage > 0;
		previousPage.interactable = maxPage > 0;

		for (int i = currentPage * perPageCapacity; i < dlcs.Count && i < (currentPage + 1) * perPageCapacity; i++)
		{
			Product dlc = dlcs[i];

			buttons[dlc.Sku].gameObject.SetActive(true);
		}
	}

	void OnRetrieveExistingPurchases(Message<PurchaseList> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to retrieve purchased dLDC list: " + result.GetError().Message);
		}
		else
		{
			BuildDLCPage(0);

			foreach (var dlc in result.GetPurchaseList())
			{
				UpdatePurchase(dlc.Sku);
			}

			if(targetDlcSKU != "")
			{
				Buy(dlcs.Find((dlc) => dlc.Sku == targetDlcSKU));
				targetDlcSKU = "";
			}
		}
	}

	void Buy(Product product)
	{
		IAP.LaunchCheckoutFlow(product.Sku).OnComplete(Purchased);
		buttons[product.Sku].buyButton.interactable = false;
	}

	void Purchased(Message<Purchase> result)
	{
		if(result == null)
		{
			Debug.LogError("Failed to buy DLC: result was null for some reason");
			return;
		}
		var purchase = result.GetPurchase();
		if(result.IsError)
		{
			Debug.LogError("Failed to buy DLC: " + result.GetError());
			buttons[purchase.Sku].buyButton.interactable = true;
		}
		else
		{
			UpdatePurchase(purchase.Sku);
			Debug.Log("Successfully purchased dlc: " + purchase.Sku);
		}
	}


	void UpdatePurchase(string sku)
	{
		if(buttons.TryGetValue(sku, out DLCButton info))
		{
			info.buyButton.interactable = false;
			info.priceText?.UpdateText("<purchased>");
		}
	}
}
