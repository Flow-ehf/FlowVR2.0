using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIPanel : MonoBehaviour, IUIConditional
{
	enum TransitionType
	{
		None,
		Animation,
		Fade,
	}

	[SerializeField] TransitionType transition = TransitionType.Fade;
	//When acivating panel, panels of higher or same layer are disabled with
	[SerializeField] int layer = 0;
	[SerializeField] int group = 0;
	[SerializeField] bool isOpen = false;
	[Space]
	[SerializeField] Animator anim;
	[SerializeField] CanvasGroup fadeGroup;
	[SerializeField] float fadeTime = 1;
	[SerializeField] UnityEvent Opened;
	[SerializeField] UnityEvent Closed;

	static List<UIPanel> allPanels = new List<UIPanel>();

	float fadeinDelayTimer;


	void OnValidate()
	{
		if(anim == null)
			anim = GetComponent<Animator>();
		if(fadeGroup == null)
			fadeGroup = GetComponent<CanvasGroup>();
	}


	public bool Enabled => isOpen;


	void OnEnable()
	{
		allPanels.Add(this);
	}


	void Start()
	{
		OnActivatedImmediately(isOpen);
	}


	void OnDisable()
	{
		allPanels.Remove(this);
	}


	public void SetActiveImmediately(bool open)
	{
		if (!open)
		{
			Closed?.Invoke();
			OnActivatedImmediately(false);
		}
		else
		{
			Opened?.Invoke();
			for (int i = 0; i < allPanels.Count; i++)
			{
				//Activate this panel
				if (allPanels[i] == this)
				{
					allPanels[i].OnActivatedImmediately(true);
				}
				else
				{
					if (allPanels[i].layer >= layer && allPanels[i].group == group)
						allPanels[i].OnActivatedImmediately(false);
				}
			}
		}
	}


	public void SetActive(bool open)
	{
		if (!open)
		{
			Closed?.Invoke();
			OnActivated(false);
		}
		else
		{
			Opened?.Invoke();
			for (int i = 0; i < allPanels.Count; i++)
			{
				if (allPanels[i] == this)
				{
					allPanels[i].OnActivated(open);
				}
				else
				{
					if (allPanels[i].layer >= layer && allPanels[i].group == group)
						allPanels[i].OnActivated(false);
				}
			}
		}
	}


	void OnActivated(bool open)
	{
		isOpen = open;

		switch (transition)
		{
			case TransitionType.None:
				SetActive(open);
				break;
			case TransitionType.Animation:
				if (anim != null)
					anim.SetBool("Open", open);
				else
					SetActive(open);
				break;
			case TransitionType.Fade:
				if (fadeGroup != null)
				{
					fadeGroup.blocksRaycasts = open;
					fadeinDelayTimer = 0;
				}
				else
					SetActive(open);	
				break;
		}
	}


	void OnActivatedImmediately(bool open)
	{
		isOpen = open;

		switch (transition)
		{
			case TransitionType.None:
				SetActive(open);
				break;
			case TransitionType.Animation:
				if (anim != null)
					anim.SetBool("Open", open);
				else
					SetActive(open);
				break;
			case TransitionType.Fade:
				if (fadeGroup != null)
				{
					fadeGroup.blocksRaycasts = open;
					fadeGroup.alpha = open ? 1 : 0;
				}
				else
					SetActive(open);
				break;
		}
	}


	void Update()
	{
		if(transition == TransitionType.Fade && fadeGroup != null)
		{
			float alpha = fadeGroup.alpha;
			if(isOpen)
			{
				if (fadeinDelayTimer < fadeTime)
					fadeinDelayTimer += Time.deltaTime;
				else if (alpha < 1)
				{
					alpha += Time.deltaTime / fadeTime;
				}
			}
			else
			{
				if(alpha > 0)
				{
					alpha -= Time.deltaTime / fadeTime;

				}
			}
			alpha = Mathf.Clamp01(alpha);

			if(fadeGroup.alpha != alpha)
			{
				fadeGroup.alpha = alpha;
			}
		}
	}

	[ContextMenu("SetActive")]
	void EditorSetActive()
	{
		SetActive(true);
	}
}
