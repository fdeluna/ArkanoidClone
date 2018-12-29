using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public  static class EditorToolsUtils
{

    public static List<GameObject> GetPrefabsAtPath(string prefab)
    {
        string[] prefabsGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { prefab });
        List<GameObject> prefabs = new List<GameObject>();

        foreach (string prefabGUID in prefabsGUIDs)
        {
            prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGUID), typeof(GameObject)));
        }

        return prefabs;
    }


    public static void DrawRectangle(Vector3 position, float width, float height, Color backgroundColor, Color outLineColor)
    {
        Vector3 p0 = new Vector3(position.x - width / 2, position.y + height / 2, 0);
        Vector3 p1 = new Vector3(position.x + width / 2, position.y - height / 2, 0);
        Vector3[] v = new Vector3[4];

        v[0] = new Vector3(p0.x, p0.y, 0);
        v[1] = new Vector3(p1.x, p0.y, 0);
        v[2] = new Vector3(p1.x, p1.y, 0);
        v[3] = new Vector3(p0.x, p1.y, 0);

        Handles.DrawSolidRectangleWithOutline(v, backgroundColor, outLineColor);
    }
}
