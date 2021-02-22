using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ColorEvent : UnityEvent<Color> { }

public class SetUIColor : MonoBehaviour
{
	[SerializeField] Color color = Color.white;
	[SerializeField] Graphic source;
	[SerializeField] Graphic target;
	[SerializeField] ColorEvent onSetColor;

	public void SetColorFromField()
	{
		if (target != null)
			target.color = color;
		else
			onSetColor?.Invoke(color);
	}

	public void SetColorFromSource()
	{
		Color color = source.color;
		if(target != null)
			target.color = color;
		else
			onSetColor?.Invoke(color);
	}
}
