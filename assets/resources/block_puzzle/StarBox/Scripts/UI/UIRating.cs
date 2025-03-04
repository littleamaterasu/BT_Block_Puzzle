using System.Collections;
using TMPro;
using UnityEngine;

public class UIRating : MonoBehaviour
{

    public TextMeshProUGUI feedbackRate;

    public NotificationObject notifyObject;

    public DG.Tweening.DOTweenAnimation anim;

    public GameObject parent;

    [Space(10)]
    [Header("Market URL")]
    public string googlePlayStoreURL;
    public string appstoreURL;

    void Start()
    {

        // marked showed off
        PlayerPrefs.SetInt(GameKey.READY_FOR_RATE, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RateNow()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
#if UNITY_ANDROID
            Application.OpenURL(googlePlayStoreURL);
#elif UNITY_IOS
            Application.OpenURL(appstoreURL);
#else
            Application.OpenURL(googlePlayStoreURL);
#endif
        }
        else
        {
            notifyObject.SetNotification(NotificationObject.NotifyType.Warning, I2.Loc.ScriptLocalization.error_network);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxStar"></param>
    public void OnClickBtnRate(bool maxStar)
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        feedbackRate.gameObject.SetActive(true);

        if (maxStar)
        {
            feedbackRate.text = I2.Loc.ScriptLocalization.thank_you;

#if UNITY_ANDROID
            Application.OpenURL(googlePlayStoreURL);
#elif UNITY_IOS
            Application.OpenURL(appstoreURL);
#else
            Application.OpenURL(googlePlayStoreURL);
#endif
            StartCoroutine(Auto(false));
        }
        else
        {
            feedbackRate.text = I2.Loc.ScriptLocalization.feedback_text;
            StartCoroutine(Auto(true));
        }  
    }

    /// <summary>
    /// Checking auto close
    /// </summary>
    private IEnumerator Auto(bool waitClose)
    {
        if (!waitClose)
        {
            anim.DOPlayBackwards();
            yield return new WaitForSeconds(anim.duration);
            parent.SetActive(false);
        } else
        {
            yield return new WaitForSeconds(1.0f);
            anim.DOPlayBackwards();
            yield return new WaitForSeconds(anim.duration);
            parent.SetActive(false);
        }
    }

    /// <summary>
    /// Use for termiate popup
    /// </summary>
    public void OnTerminate()
    {
        StartCoroutine(Auto(false));
    }

    public void TestShowRating()
    {
        gameObject.SetActive(true);
    }
}
