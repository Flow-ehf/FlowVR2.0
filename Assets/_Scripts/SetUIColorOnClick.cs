using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphics), typeof(Button))]
public class SetUIColorOnClick : MonoBehaviour
{
	[SerializeField] Graphic targetGraphics;

	Graphic graphics;
	Button button;

	private void Awake()
	{
		graphics = GetComponent<Graphic>();
		button = GetComponent<Button>();

		button.onClick.AddListener(OnClick);
	}
	
	void OnClick()
	{
		targetGraphics.color = graphics.color;
	}
}
