using I2.Loc;
using UnityEngine;

public class ChangeLanguage : MonoBehaviour {

#if UNITY_EDITOR
    public Language lang = Language.English;
#endif

    private void Awake()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Japanese:
                SetLanguage(SystemLanguage.Japanese.ToString());
                Global.LANG = Language.Japanese;
                break;
            case SystemLanguage.Korean:
                SetLanguage(SystemLanguage.Korean.ToString());
                Global.LANG = Language.Korean;
                break;
            default:
                SetLanguage(SystemLanguage.English.ToString());
                Global.LANG = Language.English;
                break;
        }

#if UNITY_EDITOR
        if(lang == Language.English) return;

        if (lang == Language.Japanese)
        {
            SetLanguage(SystemLanguage.Japanese.ToString());
            Global.LANG = Language.Japanese;
        }
        else if (lang == Language.Korean)
        {
            SetLanguage(SystemLanguage.Korean.ToString());
            Global.LANG = Language.Korean;
        }
#endif
    }

    /// <summary>
    /// Set current language in I2L source file
    /// </summary>
    /// <param name="langName"> </param>
    public void SetLanguage(string langName)
    {
        if (LocalizationManager.HasLanguage(langName))
        {
            LocalizationManager.CurrentLanguage = langName;
        }
    }
}
