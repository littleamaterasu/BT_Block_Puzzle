using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationThemeItem : MonoBehaviour, IChangable {

    private RectTransform _rectTrans;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR3)]
    public Vector3[] listRotationTheme;

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

        if (listRotationTheme.Length == 0)
        {
            Debug.LogWarning("Rotation Theme is missing or empty at " + gameObject.name);
            return;
        }

        _rectTrans.eulerAngles = listRotationTheme[1];
    }
}
