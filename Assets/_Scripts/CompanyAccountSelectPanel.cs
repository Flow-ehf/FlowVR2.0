using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CompanyAccountSelectPanel : MonoBehaviour
{
	[SerializeField] GameObject accountEntryPrefab;
	[SerializeField] RectTransform content;
	[SerializeField] Button nextPage, previousPage;
	[SerializeField] Color[] backgroundColors;
	[SerializeField] int perPageCapacity;

	private int currentPage = 0;

	private void Awake()
	{
		LoginManager.LoginChanged += LoginChanged;
	}

	private void Start()
	{
		nextPage.onClick.AddListener(GoNextPage);
		previousPage.onClick.AddListener(GoPreviousPage);

		BuildAccountPanel(0);
	}

	private void LoginChanged(bool login)
	{
		if(!login)
		{
			ClearAccountPanel();
		}
	}

	private void GoNextPage()
	{
		BuildAccountPanel(currentPage + 1);
	}

	private void GoPreviousPage()
	{
		BuildAccountPanel(currentPage - 1);
	}

	private void BuildAccountPanel(int page)
	{
		List<AccountBackend.User> availableUsers = new List<AccountBackend.User>();

		for (int i = 0; i < AccountCache.Count; i++)
		{
			AccountBackend.User cachedUser = AccountCache.GetLoginAtIndex(i);
			availableUsers.Add(cachedUser);
		}

		int maxPage = availableUsers.Count / perPageCapacity;

		if (page < 0)
			currentPage = maxPage;
		else if (maxPage > 0)
			currentPage = page % maxPage;
		else
			currentPage = 0;

		nextPage.interactable = maxPage > 0;
		previousPage.interactable = maxPage > 0;

		for (int i = content.childCount - 1; i >= 0; i--)
		{
			Destroy(content.GetChild(i).gameObject);
		}

		int count = Mathf.Min(availableUsers.Count, (page + 1) * perPageCapacity) + 1;
	
		for (int i = page * perPageCapacity; i < count; i++)
		{
			GameObject inst = Instantiate(accountEntryPrefab, content);

			AccountButton info = inst.GetComponent<AccountButton>();
			if (info == null)
			{
				Debug.LogError("Account panel prefab must have a script of type 'AccountButton'. Info will not be set");
				continue;
			}

			if (i < count - 1)
			{
				AccountBackend.User user = availableUsers[i];

				info.selectAccountButton.onClick.RemoveAllListeners();
				info.selectAccountButton.onClick.AddListener(() => Login(user));
				//if(!string.IsNullOrEmpty(user.photoUrl))
				//	StartCoroutine(LoadAvatar(user.photoUrl, info.avatarImg));
				info.avatarImg.color = backgroundColors[i % backgroundColors.Length];
				if (!string.IsNullOrEmpty(user.displayName))
					info.nameText.text = user.displayName;
				else if (!string.IsNullOrEmpty(user.email))
					info.nameText.text = user.email;
			}
			else
			{
				info.selectAccountButton.onClick.RemoveAllListeners();
				info.selectAccountButton.onClick.AddListener(() =>
				{
					LoginManager.StartAtLogin = true;
					LevelLoader.LoadLevel("LoginMenu");
				});

				info.avatarImg.color = backgroundColors[i % backgroundColors.Length];
				info.nameText.text = "Add user";
			}
		}
	}

	IEnumerator LoadAvatar(string url, Image target)
	{
		using (var request = UnityWebRequestTexture.GetTexture(url))
		{
			yield return request.SendWebRequest();
			
			if(request.isNetworkError || request.isHttpError)
			{
				Debug.LogWarning($"Failed to load avatar at '{url}'. Error '{request.error} ({request.responseCode})'");
			}
			else
			{
				var textureHandler = (DownloadHandlerTexture)request.downloadHandler;
				var texture = textureHandler.texture;
				if(texture != null)
				{
					Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
					target.sprite = sprite;
				}
			}
		}
	}

	private void Login(AccountBackend.User user)
	{
		LoginManager.LoginAsUser(user);
	}

	private void ClearAccountPanel()
	{
		//No caching for now
		for (int i = content.childCount - 1; i >= 0; i--)
		{
			Destroy(content.GetChild(i).gameObject);
		}
	}

	private void OnDestroy()
	{
		LoginManager.LoginChanged -= LoginChanged;
	}
}
