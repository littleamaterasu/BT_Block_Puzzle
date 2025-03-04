using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteThemeItem : MonoBehaviour, IChangable
{

    private SpriteRenderer _self;

    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.OBJECT)]
    public Sprite[] listSpriteTheme;

    private void Awake()
    {
        _self = GetComponent<SpriteRenderer>();
    }

    public void Start()
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
        if (_self)
        {
            if (listSpriteTheme.Length == 0)
            {
                Debug.LogWarning("Image Theme is missing or empty at " + gameObject.name);
                return;
            }

            var spriteTheme = listSpriteTheme[1];

            if (_self.sprite != spriteTheme)
                _self.sprite = spriteTheme;
        }
    }
}
