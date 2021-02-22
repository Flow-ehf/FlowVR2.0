using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistUIEntry : MonoBehaviour
{
	[SerializeField] Text numberOrder;
	[SerializeField] LanguageText levelName;
	[SerializeField] Image backgroundImage;
	[SerializeField] Text durationText;
	[SerializeField] Toggle musicState;
	[SerializeField] Toggle guidanceState;
	[SerializeField] Toggle durationState;
	[SerializeField] Button removeButton;

	public PlaylistUI playlist;

	private void Start()
	{
		removeButton.onClick.AddListener(RemoveSelf);
	}

	private void OnToggleMusic(bool isOn)
	{
		playlist.SetMusic(isOn, this);
	}

	private void OnToggleGuidancec(bool isOn)
	{
		playlist.SetGuidance(isOn, this);
	}

	private void OnToggleDuration(bool isOn)
	{
		playlist.SetDuration(isOn ? 1 : 0, this);
	}


	private void RemoveSelf()
	{
		playlist.RemoveEntry(this);
	}

	public void SetColor(Color color)
	{
		backgroundImage.color = color;
	}

	public void UpdateSession(MeditationQueue.Session session, int index)
	{
		string displayName = session.level.displayName;
		levelName.UpdateText(displayName);
		if (durationText != null)
			durationText.text = session.DurationFormatted;
		musicState.SetWithoutNotify(session.playMusic, OnToggleMusic);
		guidanceState.SetWithoutNotify(session.playGuidance, OnToggleGuidancec);
		durationState.SetWithoutNotify(session.durationIndex == 0 ? false : true, OnToggleDuration);

		numberOrder.text = (index + 1).ToString();
	}
}
