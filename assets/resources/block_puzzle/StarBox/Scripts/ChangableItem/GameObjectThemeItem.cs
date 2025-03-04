using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectThemeItem : MonoBehaviour, IChangable
{
    [Space(5)]
    [ListElementTitle(new string[] { "Woodie", "Cute" }, ListElementTitleAttribute.TypeSupport.OBJECT)]
    public GameObject[] listGameObjectTheme;

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

    /// <summary>
    /// 
    /// </summary>
    public void Change()
    {
        if(listGameObjectTheme.Length == 0)
        {
            Debug.LogWarning("GameObject referent is missing or empty at " + gameObject.name);
            return;
        }

        for(int i = 0; i < listGameObjectTheme.Length; i++)
        {
            if(listGameObjectTheme[i])
                listGameObjectTheme[i].SetActive(i == 1);
        }
    }
}
