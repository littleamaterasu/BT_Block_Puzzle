using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionThemeItem : MonoBehaviour, IChangable {

    private Transform _trans;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR3)]
    public Vector3[] listPositionTheme;

    private void Awake()
    {
        _trans = transform;
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

        var pos = listPositionTheme[1];

        if((_trans.localPosition != pos))
        {
            _trans.localPosition = pos;
        }
    }
}
