using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewChangeTheme : MonoBehaviour, IChangable {

    [Space(5)]
    public RectTransform rectTransform;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR3)]
    public Vector3[] listPosition;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.VECTOR2)]
    public Vector2[] listSize;

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
        rectTransform.localPosition = listPosition[1];
        rectTransform.sizeDelta = listSize[1];
    }
}
