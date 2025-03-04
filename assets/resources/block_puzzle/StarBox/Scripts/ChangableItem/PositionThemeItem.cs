using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionThemeItem : MonoBehaviour, IChangable {

    private RectTransform _rectTrans;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR2)]
    public Vector2[] listPositionTheme;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR4)]
    public Vector4[] listAnchors;

    void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
        Change();
        ThemeManager.Instance.AddToManager(this);
    }

    private void OnDestroy()
    {
        if (!Global.FLAG_RUNNING) return;

        ThemeManager.Instance.RemoveFromManager(this);
    }


    public void Change()
    {
        if (listPositionTheme.Length == 0 )
        {
            Debug.LogWarning("Position Theme is missing or empty at " + gameObject.name);
            return;
        }
        var themeId = 1;
        if( listAnchors.Length != 0)
        {
            _rectTrans.anchorMin = new Vector2(listAnchors[themeId].x, listAnchors[themeId].y);
            _rectTrans.anchorMax = new Vector2(listAnchors[themeId].z, listAnchors[themeId].w);
        }
        _rectTrans.anchoredPosition = listPositionTheme[themeId];
    }
}
