using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using StarBox.Scripts;

public class Loading : MonoBehaviour
{

    public float loadingDuration = 2f;
    public Ease easeLoadType = Ease.OutCirc;
    public Image imgLoadingProgress;
    public Camera mainCam;


    private AsyncOperation _async;

    private void Awake()
    {
        TileSpineAnimation.NeedSpineAnimation = true;
    }


    void Start()
    {
        Input.multiTouchEnabled = false;
        Global.CAM_ASPECT = mainCam.aspect;

        Application.targetFrameRate = 60;
#if ENABLE_LOGGER || UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        // imgLoadingProgress.DOFillAmount(1f, loadingDuration)
        //     .SetEase(easeLoadType)
        //     .OnComplete(() =>
        //     {
        //         SceneHelper.Instance.StartFading();
        //         LogActive(7);
        //         LogLogin();
        //         SceneManager.LoadScene(Global.GAMEPLAY_SCENE);
        //         // _async.allowSceneActivation = true;
        //     })
        //     .SetAutoKill(true)
        //     .Play();

        // StartCoroutine(LoadGameScene());

        imgLoadingProgress.DOFillAmount(1f, loadingDuration)
            .SetEase(easeLoadType)
            .OnComplete(()=> SceneManager.LoadScene(Global.GAMEPLAY_SCENE));
            
        // StartCoroutine(CoLoadScene());
    }

    IEnumerator CoLoadScene()
    {
        var time = 1f;
        
        Debug.LogError("Loading > time: " + time);
        while (time > 0)
        {
            imgLoadingProgress.fillAmount = 1 - time;
            time -= Time.deltaTime;
            yield return null;
        }

        imgLoadingProgress.fillAmount = 1;
        Debug.LogError("Loading > LoadScene");
        SceneManager.LoadScene(Global.GAMEPLAY_SCENE);
    }

    private void LogActive(int dayApart)
    {
        var now = System.DateTime.Now;
        
        if (IsExistActiveKey())
        {
            var lastActive_str = PlayerPrefs.GetString(GameKey.FIRST_DAY_OPEN);
            var lastActive_date = System.DateTime.ParseExact(lastActive_str, Global.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            
            if (now > lastActive_date)
            {
                var diff = now - lastActive_date;
                var days = (int) (System.Math.Abs(diff.TotalHours) / 24);
                if (days > dayApart)
                {
                    Debug.Log("Log Last Active");
                    XLogger.LogLastActive(dayApart);
                }
            }
        }
            
        var now_str = now.ToString(Global.DATE_FORMAT);
        PlayerPrefs.SetString(GameKey.LAST_ACTIVE, now_str);
    }

    private bool IsExistActiveKey()
    {
        return PlayerPrefs.HasKey(GameKey.LAST_ACTIVE);
    }

    private void LogLogin()
    {
        if (IsExistFirstDayOpen())
        {
            var first_str = PlayerPrefs.GetString(GameKey.FIRST_DAY_OPEN);
            var first_date = System.DateTime.ParseExact(first_str, Global.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            
            var now = System.DateTime.Now;
            if (now > first_date)
            {
                var diff = now - first_date;
                var dayApart = (int) (System.Math.Abs(diff.TotalHours) / 24);

                Debug.Log("Log Login");
                
                if (dayApart == 3)
                {
                    XLogger.LogOpenAt3Day();
                }
                else if (dayApart == 7)
                {
                    XLogger.LogOpenAt7Day();
                }
                else if (dayApart == 14)
                {
                    XLogger.LogOpenAt14Day();
                }
            }
        }
    }

    private bool IsExistFirstDayOpen()
    {
        return PlayerPrefs.HasKey(GameKey.FIRST_DAY_OPEN);
    }

    private IEnumerator LoadGameScene()
    {
        _async = SceneHelper.Instance.LoadSceneAsync(Global.GAMEPLAY_SCENE, LoadSceneMode.Single, needFadingImmediately: false);
        _async.allowSceneActivation = false;

        while (!_async.isDone)
        {
            yield return null;
        }
        _async.allowSceneActivation = true;
    }
    
}
