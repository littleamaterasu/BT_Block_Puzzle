using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepProgress : MonoBehaviour {

    public Transform thumb;
    public Image fill;

    [Range(0f, 10f)]
    public float offsetXThumb = 4f;

    private float _progressImageLength;
    private Vector3 _thumbOriginPosition;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        var positionByTheme = thumb.GetComponent<PositionThemeItem>();

        if (positionByTheme)
        {
            _thumbOriginPosition = positionByTheme.listPositionTheme[1];
        } else
        {
            _thumbOriginPosition = thumb.localPosition;
        }

        _progressImageLength = _rect.rect.width;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="percent"></param>
    public void Move(float percent)
    {
        var thumbDestination = percent * _progressImageLength - offsetXThumb;
        thumb.localPosition = new Vector3(thumbDestination, _thumbOriginPosition.y, 0f);

        fill.fillAmount = percent;
    }
}
