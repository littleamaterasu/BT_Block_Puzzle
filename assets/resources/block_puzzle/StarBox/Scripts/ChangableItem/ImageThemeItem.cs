using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageThemeItem : MonoBehaviour, IChangable
{

    private RectTransform _rectTrans;
    private UnityEngine.UI.Image _self;

    [Header("Prevent from Stretch")]
    public bool preventStretchX = false;
    public bool preventStretchY = false;

    private Vector2 _currentSize;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.OBJECT)]
    public Sprite[] listSpriteTheme;

    private void Awake()
    {
        // _rectTrans = GetComponent<RectTransform>();
        // _self = GetComponent<UnityEngine.UI.Image>();
        // _currentSize = _self.sprite.rect.size;
    }

    public void Start()
    {
        // Change();
        // ThemeManager.Instance.AddToManager(this);
    }

    private void OnDestroy()
    {
        // if (!Global.FLAG_RUNNING) return;
        //
        // ThemeManager.Instance.RemoveFromManager(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Change()
    {
        if(listSpriteTheme.Length == 0)
        {
            Debug.LogWarning("Image Theme is missing or empty at " + gameObject.name);
            return;
        }

        var spriteTheme = listSpriteTheme[1];

        if(_self.sprite != spriteTheme)
            _self.sprite = spriteTheme;

        if (_currentSize != spriteTheme.rect.size) {

            if (preventStretchX && preventStretchY)
                return;


            if (preventStretchX)
                _currentSize = new Vector2(0, spriteTheme.rect.size.y);

            if(preventStretchY)
                _currentSize = new Vector2(spriteTheme.rect.size.x, 0);

            if(!preventStretchX && !preventStretchY)
                _currentSize = spriteTheme.rect.size;

            _rectTrans.sizeDelta = _currentSize;
        }
    }
}
