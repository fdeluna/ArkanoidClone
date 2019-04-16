using UnityEngine;

public abstract class ArkanoidObject : MonoBehaviour
{
    private void Awake()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    protected abstract void OnGameStateChanged(GameManager.GameState state);
}
