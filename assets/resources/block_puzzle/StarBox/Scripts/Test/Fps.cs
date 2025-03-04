using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fps : MonoBehaviour
{

    float deltaTime = 0.0f;
    int w, h;
    Rect rect;
    GUIStyle style = new GUIStyle();

    void Start()
    {
        w = Screen.width;
        h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 75;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
