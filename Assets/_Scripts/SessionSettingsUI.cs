using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionSettingsUI : MonoBehaviour
{
	[SerializeField] Toggle playMusicToggle;
	[SerializeField] Toggle playGuidanceToggle;
	[SerializeField] Dropdown durationDropdown;

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
		if(durationDropdown != null)
		{
			durationDropdown.ClearOptions();
			foreach (var duration in SessionSettings.AvailableDurations)
			{
				int min = duration / 60;
				durationDropdown.options.Add(new Dropdown.OptionData(min + " min"));
			}
			durationDropdown.value = SessionSettings.DurationIndex;
			durationDropdown.RefreshShownValue();
			durationDropdown.onValueChanged.AddListener(DurationDropdownChanged);
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

	void DurationDropdownChanged(int index)
	{
		SessionSettings.DurationIndex = index;
	}
}
