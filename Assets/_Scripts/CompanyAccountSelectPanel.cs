using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CompanyAccountSelectPanel : MonoBehaviour
{
	[SerializeField] GameObject accountEntryPrefab;
	[SerializeField] RectTransform content;

	private void Awake()
	{
		LoginManager.LoginChanged += LoginChanged;
	}

	private void Start()
	{
		BuildAccountPanel();
	}

	private void LoginChanged(bool login)
	{
		if(!login)
		{
			ClearAccountPanel();
		}
	}

	private void BuildAccountPanel()
	{
		List<AccountBackend.User> availableUsers = new List<AccountBackend.User>();
		for (int i = 0; i < AccountCache.Count; i++)
		{
			var cachedUser = AccountCache.GetLoginAtIndex(i);
			if (cachedUser.uid != LoginManager.currentUser.uid)
				availableUsers.Add(cachedUser);
		}

		for (int i = content.childCount - 1; i >= 0; i--)
		{
			Destroy(content.GetChild(i).gameObject);
		}

		for (int i = 0; i < availableUsers.Count; i++)
		{
			AccountBackend.User user = availableUsers[i];

			GameObject inst = Instantiate(accountEntryPrefab, content);
			AccountButton info = inst.GetComponent<AccountButton>();
			if(info == null)
			{
				Debug.LogError("Account panel prefab must have a script of type 'AccountButton'. Info will not be set");
				continue;
			}
			info.selectAccountButton.onClick.AddListener(() => Login(user));
			if(!string.IsNullOrEmpty(user.photoUrl))
				StartCoroutine(LoadAvatar(user.photoUrl, info.avatarImg));
			if (!string.IsNullOrEmpty(user.displayName))
				info.nameText.text = user.displayName;
			else if (!string.IsNullOrEmpty(user.email))
				info.nameText.text = user.email;
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
