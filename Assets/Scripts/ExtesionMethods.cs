using System.Collections.Generic;
using UnityEngine;

public static class ExtesionMethods
{
    public static void ClearChildrens(this Transform t)
    {
        //Array to hold all child obj
        List<Transform> allChildren = new List<Transform>();

        //Find all child obj and store to that array
        foreach (Transform child in t)
        {
            allChildren.Add(child);
        }

        //Now destroy them
        foreach (Transform child in allChildren)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {        
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
