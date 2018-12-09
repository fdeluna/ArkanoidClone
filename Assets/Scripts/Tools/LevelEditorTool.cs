using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelEditorTool
{
    protected float _prefabPreviewWidth = 80;
    protected float _prefabPreviewHeight = 90;
    protected LevelManager _levelManager;


    protected virtual void Init(LevelManager levelmanager)
    {
        _levelManager = levelmanager;
    }
    protected abstract void DrawTool();
    protected abstract void UpdateTool();

    protected abstract void OnMouseMove(Vector3 mousePosition);
    protected abstract void OnMouseDown(Vector3 mousePosition);
    protected abstract void OnMouseUp();
}
