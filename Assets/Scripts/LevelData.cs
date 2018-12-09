using UnityEngine;

[CreateAssetMenu(menuName = "Arkanoid/ New Level")]
public class LevelData : ScriptableObject
{
    public int LevelHeight = 15;
    public int LevelWidth = 10;
    public float BrickHeight = 1;
    public float BrickWidth = 0.5f;

    public Sprite background;
    public AudioClip backgroundMusic;


    public LevelData nextLevel;

    // TODO BRICKS LIST TO PAINT
    // public void save()
    // public void load(LevelManager level)
}