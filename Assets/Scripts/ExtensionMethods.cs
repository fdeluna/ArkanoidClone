using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static void ClearChildrens(this Transform t)
    {
        //Array to hold all child obj
        //Find all child obj and store to that array
        var allChildren = t.Cast<Transform>().ToList();
        
        //Now destroy them
        foreach (var child in allChildren)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }


    private static readonly System.Random Rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {        
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
