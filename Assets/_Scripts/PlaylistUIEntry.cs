using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistUIEntry : MonoBehaviour
{
	[SerializeField] Text numberOrder;
	[SerializeField] Text levelName;
	[SerializeField] Image backgroundImage;
	[SerializeField] Text durationText;
	[SerializeField] Toggle musicState;
	[SerializeField] Toggle guidanceState;
	[SerializeField] Toggle settingsToggle;
	[SerializeField] Button removeButton;

	public PlaylistUI playlist;

	private void Start()
	{
		settingsToggle.onValueChanged.AddListener(OnToggleSettings);
		removeButton.onClick.AddListener(RemoveSelf);
	}

	private void OnToggleSettings(bool isOn)
	{
		playlist.OnToggleSettings(isOn, this);
	}

	private void RemoveSelf()
	{
		playlist.RemoveEntry(this);
	}

	public void SetToggleGroup(ToggleGroup group)
	{
		settingsToggle.group = group;
	}

	public void UpdateSession(MeditationQueue.Session session, int index)
	{
		levelName.text = session.level;
		durationText.text = session.DurationFormatted;
		musicState.isOn = session.playMusic;
		guidanceState.isOn = session.playGuidance;

		numberOrder.text = (index + 1).ToString();
	}
}
