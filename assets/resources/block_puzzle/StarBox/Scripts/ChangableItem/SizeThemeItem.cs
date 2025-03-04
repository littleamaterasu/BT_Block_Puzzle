using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeThemeItem : MonoBehaviour, IChangable {

    private RectTransform _rectTrans;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR2)]
    public Vector2[] listSizeTheme;


    void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start()
    {
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

        if (listSizeTheme.Length == 0)
        {
            Debug.LogWarning("Size Theme is missing or empty at " + gameObject.name);
            return;
        }

        _rectTrans.sizeDelta = listSizeTheme[1];
    }
}
