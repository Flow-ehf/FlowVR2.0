using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseBehaviour : MonoBehaviour
{
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onUnpause;

	private void Awake()
	{
		PauseController.onPaused += OnPausedChanged;
		OnPausedChanged(PauseController.IsPaused);
	}

	private void OnPausedChanged(bool paused)
	{
		if (paused)
			onPause?.Invoke();
		else
			onUnpause?.Invoke();
	}

	private void OnDestroy()
	{
		PauseController.onPaused -= OnPausedChanged;

	}
}
