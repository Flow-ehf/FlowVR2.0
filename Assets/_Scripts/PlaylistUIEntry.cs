using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaylistUIEntry : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField] Text numberOrder;
	[SerializeField] LanguageText levelName;
	[SerializeField] Image backgroundImage;
	[SerializeField] Text durationText;
	[SerializeField] Toggle musicState;
	[SerializeField] Toggle guidanceState;
	[SerializeField] Toggle durationState;
	[SerializeField] Button removeButton;

	private Vector3 initialMousePosition;
	private Vector2 initialRectPosition;
	private int initialSiblingIndex;
	private RectTransform rectTrans;

	public PlaylistUI playlist;

	private void Start()
	{
		rectTrans = (RectTransform)transform;
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

	public void OnBeginDrag(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);
		initialMousePosition = mousePos;
		initialRectPosition = rectTrans.anchoredPosition;
		initialSiblingIndex = rectTrans.GetSiblingIndex(); 
		rectTrans.SetAsLastSibling(); // Always go on top while dragging
		playlist.BeginDrag();
	}

	public void OnDrag(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);
		Vector2 delta = mousePos - initialMousePosition;
		if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
		{
			rectTrans.anchoredPosition = new Vector2(initialRectPosition.x + delta.x, initialRectPosition.y);
		}
		else
		{
			rectTrans.anchoredPosition = new Vector2(initialRectPosition.x, initialRectPosition.y + delta.y);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		rectTrans.SetSiblingIndex(initialSiblingIndex);
		if (Mathf.Abs(rectTrans.anchoredPosition.x - initialRectPosition.x) > rectTrans.sizeDelta.x * 0.25f)
			RemoveSelf();
		else if(Mathf.Abs(rectTrans.anchoredPosition.y - initialRectPosition.y) > rectTrans.sizeDelta.y * 0.5f)
		{
			int index = playlist.GetEntryDesiredIndex(this);
			playlist.SetOrder(index, this);
		}
		// will be updated by layout
		rectTrans.anchoredPosition = initialRectPosition;
		playlist.EndDrag();
	}
}
