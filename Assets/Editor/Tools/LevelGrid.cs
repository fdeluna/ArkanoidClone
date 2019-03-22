using UnityEditor;
using UnityEngine;

public class LevelGrid
{
    private LevelManager _levelInfo;
    private Vector3 _offSetPosition;

    public LevelGrid(LevelManager levelManager)
    {
        _levelInfo = levelManager;
        _offSetPosition = new Vector3(_levelInfo.transform.position.x + LevelData.BrickWidth / 2, _levelInfo.transform.position.y - LevelData.BrickHeight / 2);
    }

    public void DrawGrid()
    {
        for (int x = 0; x < LevelData.LevelWidth; x++)
        {
            for (int y = 0; y < LevelData.LevelHeight; y++)
            {
                Vector3 pos = new Vector3(_offSetPosition.x + x * LevelData.BrickWidth, _offSetPosition.y - y * LevelData.BrickHeight);
                EditorToolsUtils.DrawRectangle(pos, LevelData.BrickWidth, LevelData.BrickHeight, Color.clear, Color.white);
            }
        }
    }

    public Vector3 MousePositionToWorldPosition(Vector3 mousePosition)
    {
        Camera camera = SceneView.currentDrawingSceneView.camera;
        mousePosition.y = camera.pixelHeight - mousePosition.y;
        Vector2 gridPosition = WorldPositionToGrid(camera.ScreenToWorldPoint(mousePosition));
        return GridToWorldPosition(gridPosition);
    }

    public Vector2 MousePositionToGridPosition(Vector3 mousePosition)
    {
        Camera camera = SceneView.currentDrawingSceneView.camera;
        mousePosition.y = camera.pixelHeight - mousePosition.y;
        return WorldPositionToGrid(camera.ScreenToWorldPoint(mousePosition));
    }

    public Vector2Int WorldPositionToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((worldPosition.x - _offSetPosition.x) / LevelData.BrickWidth, 0, LevelData.LevelWidth - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp((-worldPosition.y + _offSetPosition.y) / LevelData.BrickHeight, 0, LevelData.LevelHeight - 1));

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPosition)
    {
        Vector3 pos = new Vector3
        {
            x = _levelInfo.transform.position.x + (gridPosition.x * LevelData.BrickWidth + LevelData.BrickWidth / 2.0f),
            y = _levelInfo.transform.position.y - (gridPosition.y * LevelData.BrickHeight + LevelData.BrickHeight / 2.0f)
        };

        return pos;
    }
}
