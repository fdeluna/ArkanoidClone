using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

    public static class Utils
    {
        public const string BricksPath = "Assets/Prefabs/Resources/Bricks";
        public const string PowerUpsPath = "Assets/Prefabs/Resources/PowerUp";
        public const string BackgroundMaterialsPath = "Assets/Material/Background Materials";

        public static List<T> GetPrefabsAtPath<T>(string prefab) where T : Object
        {
            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefab });

            return prefabsGuids.Select(prefabGuid => (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGuid), typeof(T))).ToList();
        }

        public static List<Sprite> GetBackGroundMaterialsAtPath(string prefab)
        {
            var prefabsGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { prefab });

            return prefabsGuids.Select(prefabGuid => (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGuid), typeof(Sprite))).ToList();
        }

        public static Vector2 GetRandomPointOutisdeCamera(Camera camera, Vector2 viewPortX, Vector2 viewPortY)
        {
            Vector2 viewportPosition = new Vector2(Random.Range(viewPortX.x, viewPortX.y), Random.Range(viewPortY.x, viewPortY.y));
            Vector2 spawnPosition = camera.ViewportToWorldPoint(viewportPosition);

            return spawnPosition;
        }
    }
