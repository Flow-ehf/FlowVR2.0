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
	[SerializeField] RectTransform container;
	[SerializeField] RectTransform prefabOrTemplate;
	[SerializeField] int perPageCapacity = 6;
	[SerializeField] Button nextPage, previousPage;
	[Header("Steam specific")]
	[SerializeField] UIPanel steamBuyMessagePanel;

	Dictionary<string, DLCButton> buttons = new Dictionary<string, DLCButton>();
	DLCButton buyTarget;
	int currentPage = 0;

	List<DLCInfo> dlcs;

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

		StartCoroutine(LoopFetchDLC());
    }

	private IEnumerator LoopFetchDLC()
	{
		while (true)
		{
#if STEAM_STORE
			OnRetrieveSteamDLCList(SteamApps.DlcInformation());
#elif OCULUS_STORE
			var dlcInfos = DLCInfo.GetDLCS();
			if (dlcInfos.Length > 0 && container != null && prefabOrTemplate != null)
			{
				string[] skus = new string[dlcInfos.Length];
				for (int i = 0; i < skus.Length; i++)
					skus[i] = dlcInfos[i].sku;
				IAP.GetProductsBySKU(skus).OnComplete(OnRetrievedOculusProductList);
			}
#endif
			yield return new WaitForSeconds(5);
		}
	}

#if OCULUS_STORE
	void OnRetrievedOculusProductList(Message<ProductList> result)
	{
		if(result.IsError)
		{
			Debug.LogError("Failed to retrieve DLC list: " + result.GetError().Message);
		}
		else
		{
			List<DLCInfo> list = new List<DLCInfo>();
			var availableDLCs = DLCInfo.GetDLCS();

			foreach (var product in result.GetProductList())
			{
				DLCInfo info = availableDLCs.Find((d) => d.sku == product.Sku);
				if (info)
					list.Add(info);
				else
					Debug.LogError("no matching DLCInfo for dlc " + product.Sku);
			}

			SetProducts(list.ToArray());

			IAP.GetViewerPurchases().OnComplete(OnRetrieveExistingPurchases);
		}
	}
#endif

#if STEAM_STORE
	void OnRetrieveSteamDLCList(IEnumerable<Steamworks.Data.DlcInformation> result)
	{
		SetProducts(DLCInfo.GetDLCS());

		if (result != null)
		{
			foreach (var purchased in result)
			{
				if(purchased.Available)
				{
					UpdatePurchase(buttons[purchased.AppId.Value.ToString()]);
				}
			}
		}
	}
#endif

	void SetProducts(DLCInfo[] list)
	{
		foreach (var button in buttons.Values)
		{
			Destroy(button.gameObject);
		}
		buttons.Clear();

		dlcs = new List<DLCInfo>(list);
		dlcs.Sort((p1, p2) => p1.sku.CompareTo(p2.sku));

		for (int i = 0; i < list.Length; i++)
		{
			DLCInfo dlc = list[i];

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

		if (targetDlcSKU != "")
		{
			var dlc = dlcs.Find((p) => p.sku == targetDlcSKU);
			if (dlc != null)
			{
				Buy(dlc);
				targetDlcSKU = "";
			}
		}

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
			DLCInfo dlc = dlcs[i];

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
		}
	}

	void Buy(DLCInfo info)
	{
#if STEAM_STORE
		UnityEngine.Application.OpenURL(info.steamURL);
		steamBuyMessagePanel.SetActive(true);
#elif OCULUS_STORE
		IAP.LaunchCheckoutFlow(info.sku).OnComplete(Purchased);
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
		if(result.IsError)
		{
			Debug.LogError("Failed to buy DLC: " + result.GetError());
			buyTarget.buyButton.interactable = true;
		}
		else
		{
			var purchase = result.GetPurchase();
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
		SetProducts(DLCInfo.GetDLCS());
	}
}
