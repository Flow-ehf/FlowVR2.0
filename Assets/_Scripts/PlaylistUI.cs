using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistUI : MonoBehaviour
{
	[SerializeField] GameObject playlistEntryPrefab;
	[SerializeField] Transform playlistContainer;
	[SerializeField] Toggle musicToggle;
	[SerializeField] Toggle guidanceToggle;
	[SerializeField] Dropdown durationDropdown;
	[SerializeField] UIPanel settingsPanel;
	[SerializeField] Button playButton;

	private List<MeditationQueue.Session> playlist = new List<MeditationQueue.Session>();
	private List<PlaylistUIEntry> uiList = new List<PlaylistUIEntry>();

	private ToggleGroup group;

	private int settingsEditIndex = -1;

	private void Start()
	{
		group = playlistContainer.GetComponent<ToggleGroup>();
	
		durationDropdown.ClearOptions();
		List<string> durationOptions = new List<string>();
		for (int i = 0; i < MeditationQueue.availableSessionDurations.Length; i++)
		{
			durationOptions.Add(MeditationQueue.DurationFormatted(i));
		}
		durationDropdown.AddOptions(durationOptions);

		musicToggle.onValueChanged.AddListener(SetMusic);
		guidanceToggle.onValueChanged.AddListener(SetGuidance);
		durationDropdown.onValueChanged.AddListener(SetDuration);

		playButton.interactable = false;
		playButton.onClick.AddListener(Play);
	}

	private void SetMusic(bool isOn)
	{
		if(settingsEditIndex > -1)
		{
			var session = playlist[settingsEditIndex];
			session.playMusic = isOn;
			playlist[settingsEditIndex] = session;

			uiList[settingsEditIndex].UpdateSession(session, settingsEditIndex);
		}
	}

	private void SetGuidance(bool isOn)
	{
		if (settingsEditIndex > -1)
		{
			var session = playlist[settingsEditIndex];
			session.playGuidance = isOn;
			playlist[settingsEditIndex] = session;

			uiList[settingsEditIndex].UpdateSession(session, settingsEditIndex);
		}
	}

	private void SetDuration(int index)
	{
		if (settingsEditIndex > -1)
		{
			var session = playlist[settingsEditIndex];
			session.durationIndex = index;
			playlist[settingsEditIndex] = session;

			uiList[settingsEditIndex].UpdateSession(session, settingsEditIndex);
		}
	}

	private void Play()
	{
		if(playlist.Count > 0)
		{
			MeditationQueue.SetQueue(playlist);
			LevelLoader.LoadLevel(playlist[0].level);
		}
	}

	public void AddEntry(string level)
	{
		var session = new MeditationQueue.Session(level);
		playlist.Add(session);

		GameObject go = Instantiate(playlistEntryPrefab);
		go.transform.SetParent(playlistContainer);
		go.transform.localPosition = Vector3.zero;
		PlaylistUIEntry entry = go.GetComponent<PlaylistUIEntry>();
		entry.playlist = this;
		entry.SetToggleGroup(group);
		entry.UpdateSession(session, playlist.Count - 1);
		uiList.Add(entry);

		playButton.interactable = true;
	}

	public void OnToggleSettings(bool isOn, PlaylistUIEntry entry)
	{
		if(!isOn)
		{
			settingsEditIndex = -1;
			settingsPanel.SetActive(false);
		}
		else
		{
			int idx = uiList.IndexOf(entry);
			if(idx > -1)
			{
				settingsEditIndex = idx;
				musicToggle.isOn = playlist[idx].playMusic;
				guidanceToggle.isOn = playlist[idx].playGuidance;
				durationDropdown.value = playlist[idx].durationIndex;
				settingsPanel.SetActive(true);
			}
		}

	}

	public void RemoveEntry(PlaylistUIEntry entry)
	{
		int index = uiList.IndexOf(entry);
		if (index > -1)
			RemoveEntry(index);
	}

	public void RemoveEntry(int index)
	{
		playlist.RemoveAt(index);

		Destroy(uiList[index].gameObject);
		uiList.RemoveAt(index);

		if (index == settingsEditIndex)
			OnToggleSettings(false, null);

		if (playlist.Count == 0)
			playButton.interactable = false;
	}
}
