using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
	public static ScreenFade instance;

	static Color currentColor = new Color(0, 0, 0, 1);
	static Color targetColor = new Color(0, 0, 0, 1);
	static Color deltaColor = new Color(0, 0, 0, 0);   // the delta-color is basically the "speed / second" at which the current color should change

	static Material fadeMaterial = null;


	void OnEnable()
	{
		instance = this;

		if (fadeMaterial == null)
		{
			fadeMaterial = new Material(Shader.Find("Custom/Fade"));
		}
	}

	public static void StartFade(float duration, Color newColor)
	{
		if (duration > 0.0f)
		{
			targetColor = newColor;
			deltaColor = (targetColor - currentColor) / duration;
		}
		else
		{
			currentColor = newColor;
		}
	}

	void OnPostRender()
	{
		if (currentColor != targetColor)
		{
			// if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
			if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.unscaledDeltaTime)
			{
				currentColor = targetColor;
				deltaColor = new Color(0, 0, 0, 0);
			}
			else
			{
				currentColor += deltaColor * Time.unscaledDeltaTime;
			}
		}

		if (currentColor.a > 0 && fadeMaterial)
		{
#if UNITY_EDITOR || !UNITY_ANDROID
			GL.PushMatrix();
			GL.LoadOrtho();
			fadeMaterial.SetPass(0);
			GL.Begin(GL.QUADS);
			GL.Color(currentColor);
			GL.Vertex3(0, 0, 0);
			GL.Vertex3(1, 0, 0);
			GL.Vertex3(1, 1, 0);
			GL.Vertex3(0, 1, 0);
			GL.End();
			GL.PopMatrix();
#endif
		}
	}
}