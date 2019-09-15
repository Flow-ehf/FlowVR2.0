using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIColor : MonoBehaviour
{
	[SerializeField] Color color = Color.white;
	[SerializeField] Graphic source;
	[SerializeField] Graphic target;

    public void SetColorFromField()
	{
		target.color = color;
	}

	public void SetColorFromSource()
	{
		target.color = source.color;
	}
}
