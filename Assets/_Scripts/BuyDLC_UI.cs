using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Platform;
using Oculus.Platform.Models;
#if STEAM_STORE
using Steamworks;
#endif


public class BuyDLC_UI : MonoBehaviour
{
	[Header("DLC identifiers go here")]
	[SerializeField] ProductUIInfo[] dlcInfos;
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
#if STEAM_STORE
		OnRetrieveSteamDLCList(SteamApps.DlcInformation());
#elif OCULUS_STORE
		if (dlcInfos.Length > 0 && container != null && prefabOrTemplate != null)
		{
			string[] skus = new string[dlcInfos.Length];
			for (int i = 0; i < skus.Length; i++)
				skus[i] = dlcInfos[i].sku;
			IAP.GetProductsBySKU(skus).OnComplete(OnRetrievedOculusProductList);
		}
#endif
    }

    void OnRetrievedOculusProductList(Message<ProductList> result)
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

#if STEAM_STORE
	void OnRetrieveSteamDLCList(IEnumerable<Steamworks.Data.DlcInformation> result)
	{
		List<ProductUIInfo> list = new List<ProductUIInfo>();

		foreach (var dlc in dlcInfos)
		{
			ProductUIInfo info = dlc;
			info.sku = dlc.steamAppid.ToString();
			list.Add(info);
		}

		SetProducts(list);

		if (result != null)
		{
			foreach (var purchased in result)
			{
				UpdatePurchase(buttons[purchased.AppId.Value.ToString()]);
			}
		}
	}
#endif

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
			info.buyButton.onClick.AddListener(() => Buy(dlc));
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
				var dlc = dlcs.Find((p) => p.sku == targetDlcSKU);
				if (dlc != null)
				{
					Buy(dlc);
					targetDlcSKU = "";
				}
			}
		}
	}

	void Buy(ProductUIInfo info)
	{
#if STEAM_STORE
		UnityEngine.Application.OpenURL(info.steamURL);
#elif OCULUS_STORE
		IAP.LaunchCheckoutFlow(sku).OnComplete(Purchased);
		buyTarget = buttons[info.sku];
		buyTarget.buyButton.interactable = false;
#endif
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
		List<ProductUIInfo> list = new List<ProductUIInfo>(dlcInfos);

		SetProducts(list);
	}

	[System.Serializable]
	class ProductUIInfo
	{
		public uint steamAppid;
		public string sku;
		public string desc;
		public string name;
		public string steamURL;
		public string price;

		public ProductUIInfo(Product product)
		{
			this.name = product.Name;
			this.sku = product.Sku;
			this.desc = product.Description;
			this.price = product.FormattedPrice;
		}

		#if STEAM_STORE
		public ProductUIInfo(Steamworks.Data.DlcInformation dlc)
		{
			this.steamAppid = dlc.AppId.Value;
			this.sku = dlc.AppId.ToString();
			this.name = dlc.Name;
		}
		#endif

		public ProductUIInfo(string name, string sku, string desc, string price)
		{
			this.name = name;
			this.sku = sku;
			this.desc = desc;
			this.price = price;
		}
	}
}
