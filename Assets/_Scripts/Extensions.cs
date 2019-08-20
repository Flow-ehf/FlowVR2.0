using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions 
{
	public static T Find<T>(this T[] array, System.Predicate<T> predicate)
	{
		return Array.Find(array, predicate);
	}


	public static bool Exists<T>(this T[] array, System.Predicate<T> predicate)
	{
		return Array.Exists(array, predicate);
	}


	public static int IndexOf<T>(this T[] array, T element)
	{
		return Array.IndexOf(array, element);
	}


	public static T RandomElement<T>(this IList<T> list)
	{
		if (list.Count == 0)
			throw new InvalidOperationException("List is empty!");
		return list[Random.Range(0, list.Count)];
	}


	public static Color WithAlpha(this Color c, float alpha)
	{
		return new Color(c.r, c.g, c.b, alpha);
	}
}
