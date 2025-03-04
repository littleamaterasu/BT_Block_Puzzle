using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Only work with Canvas
/// </summary>
public class ZOrderThemeItem : MonoBehaviour, IChangable {

    private Canvas _canvas;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.INTEGER)]
    public int[] listZOrderTheme;


    void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    // Use this for initialization
    void Start()
    {
        if (_canvas)
        {
            Change();
            ThemeManager.Instance.AddToManager(this);
        }
    }

    private void OnDestroy()
    {
        if (!Global.FLAG_RUNNING) return;

        if(_canvas)
            ThemeManager.Instance.RemoveFromManager(this);
    }


    public void Change()
    {

        if (listZOrderTheme.Length == 0)
        {
            Debug.LogWarning("ZOrder Theme is missing or empty at " + gameObject.name);
            return;
        }

        _canvas.sortingOrder = listZOrderTheme[1];
    }
}
