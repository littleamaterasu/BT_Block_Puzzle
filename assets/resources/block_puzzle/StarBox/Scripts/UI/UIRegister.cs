using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class UIRegister : MonoBehaviour 
{
    public Button exitBtn;
    public Button confirmBtn;
    public InputField input;

    public NotificationObject notifyObject;

    public static System.Action<string> OnDoneRegister;

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(DataCache.Name))
        {
            input.text = DataCache.Name;
        }
    }

    private void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirm);
        exitBtn.onClick.AddListener(OnExit);
    }

    private void OnExit()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);
        gameObject.SetActive(false);

        if(string.IsNullOrEmpty(DataCache.Name))
            notifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.information_not_created);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnConfirm()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        var trim = input.text.Trim();

        if (!string.IsNullOrEmpty(trim))
        {
            if (trim.Length >= 3 && trim.Length <= 12)
            {
                gameObject.SetActive(false);
                OnDoneRegister?.Invoke(trim);
            }
        } else
        {
            notifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.empty_given_name);
        }

    }
}
