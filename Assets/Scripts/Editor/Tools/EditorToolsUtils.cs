using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorToolsUtils
{ 
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


    public static void DrawScrollViewWindow<T>(int windowID, ref Vector2 scrollPosition, ref int currentIndex, List<T> objects, float guiStylefixedWidth, float guiStylefixedHeigh) where T : Object
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        currentIndex = GUILayout.SelectionGrid(
            currentIndex,
            GetGUIContents(objects),
            1,
            GetGUIStyle(guiStylefixedWidth, guiStylefixedHeigh)
            );
        GUILayout.EndScrollView();
    }

    private static GUIContent[] GetGUIContents<T>(List<T> objects) where T : Object
    {
        List<GUIContent> guiContents = new List<GUIContent>();
        if (objects.Count > 0)
        {
            foreach (Object unityObject in objects)
            {
                GUIContent guiContent = new GUIContent
                {
                    text = unityObject.name,
                    image = AssetPreview.GetAssetPreview(unityObject)

                };
                guiContents.Add(guiContent);
            }
        }
        return guiContents.ToArray();
    }


    private static GUIStyle GetGUIStyle(float guiStylefixedWidth, float guiStylefixedHeigh)
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button)
        {
            imagePosition = ImagePosition.ImageAbove,
            alignment = TextAnchor.UpperCenter,
            fixedWidth = guiStylefixedWidth,
            fixedHeight = guiStylefixedHeigh
        };
        return guiStyle;
    }
}
