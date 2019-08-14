using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DLCButton : MonoBehaviour
{
	public LanguageText nameText;
	public LanguageText priceText;
	public LanguageText descriptionText;
	public Button buyButton;

	[HideInInspector] public string sku;
}
