using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationObject : MonoBehaviour {

    public enum NotifyType
    {
        Information,
        Warning
    }

    public Sprite inforSprite;
    public Sprite warningSprite;

    [SerializeField]
    private UnityEngine.UI.Image _visual;

    [SerializeField]
    private TextMeshProUGUI _contentText;

    private NotifyType _type = NotifyType.Information;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public void SetNotification(NotifyType type, string content)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        else
            return;

        if(_type != type)
        {
            _type = type;

            switch (type)
            {
                case NotifyType.Information:
                    _visual.sprite = inforSprite;
                    break;
                case NotifyType.Warning:
                    _visual.sprite = warningSprite;
                    break;
                default:
                    _visual.sprite = inforSprite;
                    break;
            }      
        }

        _contentText.text = content;
    }
}
