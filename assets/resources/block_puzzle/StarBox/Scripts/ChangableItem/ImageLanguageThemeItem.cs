
using UnityEngine;

public class ImageLanguageThemeItem : MonoBehaviour, IChangable
{
    private UnityEngine.UI.Image _self;

    [Space(10)]
    [ListElementTitle(new string[] { "English", "Japanese", "Korean" }, ListElementTitleAttribute.TypeSupport.OBJECT)]
    public Sprite[] cuteLangSprite;

    private void Awake()
    {
        _self = GetComponent<UnityEngine.UI.Image>();
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
        int langIndex = (int)Global.LANG;

        Sprite spriteTheme = null;
        var lenCute = cuteLangSprite.Length;

        if (langIndex < lenCute)
            spriteTheme = cuteLangSprite[(int)Global.LANG];
        

        if (spriteTheme != null)
        {
            if(_self.sprite != spriteTheme)
                _self.sprite = spriteTheme;
        }
        
        _self.SetNativeSize();
    }
}

