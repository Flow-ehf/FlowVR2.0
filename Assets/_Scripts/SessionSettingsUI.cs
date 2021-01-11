﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SessionSettingsUI : MonoBehaviour
{
	[SerializeField] Toggle playMusicToggle;
	[SerializeField] Toggle playGuidanceToggle;
	[SerializeField] GameObject durationButtonTemplate;
	[SerializeField] RectTransform durationContainer;
	[SerializeField] bool saveSessionSettings;
	[SerializeField] LoadLevelButton playButton;
	[SerializeField] LoadLevelButton buyButton;
	[Space]
	[SerializeField] VideoPlayer player;

	Material defaultSkybox;
	VideoClip defaultClip;
	UIPanel panel;
	List<Toggle> durationToggles = new List<Toggle>();

	void Awake()
	{
		panel = GetComponent<UIPanel>();
	}


	// Start is called before the first frame update
	void Start()
    {
		if (!saveSessionSettings)
			SessionSettings.Reset();

		defaultSkybox = RenderSettings.skybox;
		defaultClip = player.clip;

		if(playMusicToggle != null)
		{
			playMusicToggle.isOn = SessionSettings.PlayMusic;
			playMusicToggle.onValueChanged.AddListener(TogglePlayMusic);
		}
		if(playGuidanceToggle != null)
		{
			playGuidanceToggle.isOn = SessionSettings.PlayGuidance;
			playGuidanceToggle.onValueChanged.AddListener(TogglePlayGuidance);
		}
		if(durationButtonTemplate != null && durationContainer != null)
		{
			ToggleGroup group = durationContainer.GetComponent<ToggleGroup>();
			if(group == null)
				group = durationContainer.gameObject.AddComponent<ToggleGroup>();
			group.allowSwitchOff = false;

			float xPos = 0;
			for (int i = 0; i < SessionSettings.AvailableDurations.Length; i++)
			{
				int min = SessionSettings.AvailableDurations[i] / 60;
				RectTransform rectTrans = Instantiate(durationButtonTemplate).GetComponent<RectTransform>();
				rectTrans.SetParent(durationContainer, false);
				rectTrans.anchoredPosition = new Vector2(xPos, 0);
				Text text = rectTrans.GetComponentInChildren<Text>();
				if (text != null)
					text.text = min + " \nmin";
				Toggle toggle = rectTrans.GetComponentInChildren<Toggle>();
				if(toggle != null)
				{
					durationToggles.Add(toggle);
					int index = i;
					toggle.isOn = SessionSettings.DurationIndex == index;
					toggle.onValueChanged.AddListener((isOn) =>
					{
						if(isOn)
							DurationIndexChanged(index);
					});
					toggle.group = group;
				}
				xPos += (rectTrans.sizeDelta.x - 10);
			}
			durationButtonTemplate.SetActive(false);
		}
    }


	public void Open(Material targetMateial, VideoClip targetClip, bool owned)
	{
		StartCoroutine(WaitOpen(targetMateial, targetClip, owned));
	}

	public IEnumerator WaitOpen(Material targetMaterial, VideoClip targetClip, bool owned)
	{
		panel.SetActive(true);

		playButton.gameObject.SetActive(owned);
		buyButton.gameObject.SetActive(!owned);

		playMusicToggle.interactable = owned;
		playGuidanceToggle.interactable = owned;
		foreach (var toggle in durationToggles)
			toggle.interactable = owned;

		if (targetClip != null)
		{
			player.clip = targetClip;
			player.targetTexture = targetMaterial.mainTexture as RenderTexture;
			player.Prepare();
		}
		ScreenFade.StartFade(1, Color.black);
		yield return new WaitForSeconds(1);
		yield return new WaitWhile(() => !player.isPrepared);
		RenderSettings.skybox = targetMaterial;
		ScreenFade.StartFade(1, Color.clear);
	}


	public void Close()
	{
		StartCoroutine(WaitClose());
	}

	IEnumerator WaitClose()
	{
		ScreenFade.StartFade(1, Color.black);
		yield return new WaitForSeconds(1);
		player.clip = defaultClip;
		player.targetTexture = defaultSkybox.mainTexture as RenderTexture;
		RenderSettings.skybox = defaultSkybox;
		ScreenFade.StartFade(1, Color.clear);
	}


	void TogglePlayMusic(bool isOn)
	{
		SessionSettings.PlayMusic = isOn;
	}

	void TogglePlayGuidance(bool isOn)
	{
		SessionSettings.PlayGuidance = isOn;
	}

	void DurationIndexChanged(int index)
	{
		SessionSettings.DurationIndex = index;
	}


	void OnDestroy()
	{
		RenderSettings.skybox = defaultSkybox;
	}
}
