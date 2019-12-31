using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Utils
{    
    public const string BricksPath = "Assets/Prefabs/Resources/Bricks";
    public const string PowerUpsPath = "Assets/Prefabs/Resources/PowerUp";
    public const string BackgroundMaterialsPath = "Assets/Material/Background Materials";

    public static List<GameObject> GetPrefabsAtPath(string prefab)
    {
        var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefab });

        return prefabsGuids.Select(prefabGuid => (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGuid), typeof(GameObject))).ToList();
    }

    public static List<Sprite> GetBackGroundMaterialsAtPath(string prefab)
    {
        var prefabsGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { prefab });

        return prefabsGuids.Select(prefabGuid => (Sprite) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGuid), typeof(Sprite))).ToList();
    }
}
