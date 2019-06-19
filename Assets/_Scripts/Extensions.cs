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


	public static T RandomElement<T>(this IList<T> list)
	{
		if (list.Count == 0)
			throw new InvalidOperationException("List is empty!");
		return list[Random.Range(0, list.Count)];
	}
}
