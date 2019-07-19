using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionSettingsUI : MonoBehaviour
{
	[SerializeField] Toggle playMusicToggle;
	[SerializeField] Toggle playGuidanceToggle;
	[SerializeField] GameObject durationButtonTemplate;
	[SerializeField] RectTransform durationContainer;

	UIPanel panel;

	Material startSkybox;

	void Awake()
	{
		panel = GetComponent<UIPanel>();
		startSkybox = RenderSettings.skybox;
	}


	// Start is called before the first frame update
	void Start()
    {
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

			float yPos = 0;
			for (int i = 0; i < SessionSettings.AvailableDurations.Length; i++)
			{
				int min = SessionSettings.AvailableDurations[i] / 60;
				RectTransform rectTrans = Instantiate(durationButtonTemplate).GetComponent<RectTransform>();
				rectTrans.SetParent(durationContainer, false);
				rectTrans.anchoredPosition = new Vector2(0, -yPos);
				Text text = rectTrans.GetComponentInChildren<Text>();
				if (text != null)
					text.text = min + " \nmin";
				Toggle toggle = rectTrans.GetComponentInChildren<Toggle>();
				if(toggle != null)
				{
					int index = i;
					toggle.isOn = SessionSettings.DurationIndex == index;
					toggle.onValueChanged.AddListener((isOn) =>
					{
						if(isOn)
							DurationIndexChanged(index);
					});
					toggle.group = group;
				}
				yPos += rectTrans.sizeDelta.y;
			}
			durationButtonTemplate.SetActive(false);
		}
    }


	public void Open(Material targetSkybox)
	{
		StartCoroutine(WaitOpen(targetSkybox));
	}

	public IEnumerator WaitOpen(Material targetSkybox)
	{
		panel.SetActive(true);

		ScreenFade.instance.StartFade(1, Color.black);
		yield return new WaitForSeconds(1);
		RenderSettings.skybox = targetSkybox;
		ScreenFade.instance.StartFade(1, Color.clear);
	}


	public void Close()
	{
		StartCoroutine(WaitClose());
	}

	IEnumerator WaitClose()
	{
		ScreenFade.instance.StartFade(1, Color.black);
		yield return new WaitForSeconds(1);
		RenderSettings.skybox = startSkybox;
		ScreenFade.instance.StartFade(1, Color.clear);
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
		RenderSettings.skybox = startSkybox;
	}
}
