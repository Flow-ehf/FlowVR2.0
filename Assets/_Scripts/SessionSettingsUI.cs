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
					text.text = min + " min";
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
}
