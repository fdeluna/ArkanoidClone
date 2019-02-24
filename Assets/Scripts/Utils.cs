﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Utils
{    
    public static string BRICKS_PATH = "Assets/Prefabs/Resources/Bricks";
    public static string POWERUPS_PATH = "Assets/Prefabs/Resources/PowerUp";
    public static string BACKGROUND_MATERIALS_PATH = "Assets/Material/Background Materials";

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

    public static List<Material> GetBackGroundMaterialsAtPath(string prefab)
    {
        string[] prefabsGUIDs = AssetDatabase.FindAssets("t:Material", new string[] { prefab });
        List<Material> materials = new List<Material>();

        foreach (string prefabGUID in prefabsGUIDs)
        {
            materials.Add((Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGUID), typeof(Material)));
        }

        return materials;
    }
}