using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextConfigTheme : MonoBehaviour, IChangable {

    public Text thisText;
    public Color[] listColors;

    // Use this for initialization
    void Start () {
		Change();
        ThemeManager.Instance.AddToManager(this);
	}
    
    /// <summary>
    /// 
    /// </summary>
    public void Change()
    {
        thisText.color = listColors[1];
    }

    private void OnDestroy()
    {
        if (!Global.FLAG_RUNNING) return;

        ThemeManager.Instance.RemoveFromManager(this);
    }
}
