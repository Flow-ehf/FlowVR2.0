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
	[SerializeField] int perPageCapacity = 6;
	[SerializeField] Button nextPage, previousPage;

	Dictionary<string, DLCButton> buttons = new Dictionary<string, DLCButton>();
	DLCButton buyTarget;
	int currentPage = 0;

	List<ProductUIInfo> dlcs = new List<ProductUIInfo>();

	public static string targetDlcSKU = "";

    // Start is called before the first frame update
    void Start()
    {
		for (int i = container.childCount - 1; i >= 0; i--)
		{
			Destroy(container.GetChild(i).gameObject);
		}

		nextPage.interactable = false;
		nextPage.onClick.AddListener(GoNextPage);
		previousPage.interactable = false;
		previousPage.onClick.AddListener(GoPreviousPage);

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
			List<ProductUIInfo> list = new List<ProductUIInfo>();

			foreach (var product in result.GetProductList())
			{
				list.Add(new ProductUIInfo(product));
			}

			SetProducts(list);

			IAP.GetViewerPurchases().OnComplete(OnRetrieveExistingPurchases);
		}
	}

	void SetProducts(List<ProductUIInfo> list)
	{
		dlcs = list;
		dlcs.Sort((p1, p2) => p1.sku.CompareTo(p2.sku));

		for (int i = 0; i < list.Count; i++)
		{
			ProductUIInfo dlc = list[i];

			GameObject newEntry = Instantiate(prefabOrTemplate.gameObject, container);
			newEntry.SetActive(true);
			DLCButton info = newEntry.GetComponent<DLCButton>();
			info.nameText?.UpdateText(dlc.name);
			info.priceText?.UpdateText(dlc.price);
			info.descriptionText?.UpdateText(dlc.desc);
			info.sku = dlc.sku;
			info.buyButton.onClick.RemoveAllListeners();
			info.buyButton.onClick.AddListener(() => Buy(dlc.sku));
			buttons[dlc.sku] = info;
		}
		BuildDLCPage(0);

	}

	[ContextMenu("Go Next")]
	void GoNextPage()
	{
		BuildDLCPage(currentPage + 1);
	}

	[ContextMenu("Go Previous")]
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

		int maxPage = (dlcs.Count - 1) / perPageCapacity;

		if (page < 0)
			currentPage = maxPage;
		else if (maxPage > 0)
			currentPage = page % (maxPage + 1);
		else
			currentPage = 0;

		nextPage.interactable = maxPage > 0;
		previousPage.interactable = maxPage > 0;

		for (int i = currentPage * perPageCapacity; i < dlcs.Count && i < (currentPage + 1) * perPageCapacity; i++)
		{
			ProductUIInfo dlc = dlcs[i];

			buttons[dlc.sku].gameObject.SetActive(true);
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
			foreach (var dlc in result.GetPurchaseList())
			{
				UpdatePurchase(buttons[dlc.Sku]);
			}

			if(targetDlcSKU != "")
			{
				Buy(targetDlcSKU);
				targetDlcSKU = "";
			}
		}
	}

	void Buy(string sku)
	{
		IAP.LaunchCheckoutFlow(sku).OnComplete(Purchased);
		buyTarget = buttons[sku];
		buyTarget.buyButton.interactable = false;
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
			buyTarget.buyButton.interactable = true;
		}
		else
		{
			UpdatePurchase(buyTarget);
			Debug.Log("Successfully purchased dlc: " + purchase.Sku);
		}
	}


	void UpdatePurchase(DLCButton button)
	{
		button.buyButton.interactable = false;
		button.priceText?.UpdateText("<owned>");
	}

	[ContextMenu("Add test data")]
	void AddTestData()
	{
		List<ProductUIInfo> list = new List<ProductUIInfo>();

		for (int i = 0; i < dlcSKUValues.Length; i++)
		{
			list.Add(new ProductUIInfo(i.ToString(), dlcSKUValues[i], "desc " + i, i + "$"));
		}
		SetProducts(list);
	}

	class ProductUIInfo
	{
		public string sku;
		public string desc;
		public string name;
		public string price;

		public ProductUIInfo(Product product)
		{
			this.name = product.Name;
			this.sku = product.Sku;
			this.desc = product.Description;
			this.price = product.FormattedPrice;
		}

		public ProductUIInfo(string name, string sku, string desc, string price)
		{
			this.name = name;
			this.sku = sku;
			this.desc = desc;
			this.price = price;
		}
	}
}
