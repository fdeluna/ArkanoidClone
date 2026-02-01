using System;
using UnityEditor;
using UnityEngine;

public enum FeelEnvironments
{
    NO_FEEL,
    FEEL_,
    PLAYING,
    GAMEOVER
}

[CreateAssetMenu(menuName = "Arkanoid/Feel Configuration")]
public class FeelConfiguration : ScriptableObject
{
    
    private static FeelConfiguration _instance;
    public static FeelConfiguration Instance {
        get {
            if (_instance == null)
            {
                _instance = Resources.Load<FeelConfiguration>(typeof(FeelConfiguration).ToString());
            }
            return _instance;
        }
    }
    
    public FeelSettings settings;
}

[Serializable]
public class FeelSettings
{
    public bool startUp;
}