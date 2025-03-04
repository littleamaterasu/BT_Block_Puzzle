using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendPositionThemeItem : MonoBehaviour, IChangable {

    private RectTransform _rectTrans;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR2)]
    public Vector2[] listPositionTheme;

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
            Debug.LogWarning("Extend Position Theme is missing or empty at " + gameObject.name);
            return;
        }


        var newPos = listPositionTheme[1];
        if (Global.CAM_ASPECT >= 0.42f && Global.CAM_ASPECT <= 0.503f)
        {
            //IpX
            newPos.y += 90f;
        }

        _rectTrans.anchoredPosition = newPos;
    }
}
