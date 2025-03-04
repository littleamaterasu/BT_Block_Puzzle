using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DOTweenExtension;
using System;
using Falcon.FalconMediation.Core;
using TMPro;

public class UIGameOver : MonoBehaviour
{
    public static bool SELF_PLAY = false;
    public static bool OUT_FROM_GAMEPLAY = true;

    [Header("UI")] public TextMeshProUGUI scoreText, scoreTextBS;
    public TextMeshProUGUI recordText;

    [Space(5)] public GameObject rootObject;
    public GameObject camMain;

    public GameObject gameoverUI, bestScoreUI;

    [Space(5)] public Button playButtonGO, playButtonBS;
    public Button playRecordButton;

    [Space(5)] public Button rankingButton;
    public Button shareButton;

    [Space(5)] public GameObject uiRanking;

    [Header("Configuration")] public float durationText = 2f;

    [Header("Rating Configuration")] public int playedTimeToTrigger = 3;
    public GameObject ratePanel;

    [Space(10)] public NotificationObject notify;

    private void Start()
    {
        Debug.LogError(GameUnit.Instance.SCORE + " * " + GameUnit.Instance.LAST_SCORE + " * " +
                       GameUnit.Instance.HIGH_SCORE);
        gameoverUI.SetActive(GameUnit.Instance.SCORE < GameUnit.Instance.HIGH_SCORE);
        bestScoreUI.SetActive(GameUnit.Instance.SCORE >= GameUnit.Instance.HIGH_SCORE);

        SELF_PLAY = false;

        ///play
        playButtonGO.onClick.AddListener(PlayNew);
        playButtonBS.onClick.AddListener(PlayNew);

        if (GameUnit.Instance.HighestProgress == null)
            playRecordButton.interactable = false;

        playRecordButton.onClick.AddListener(OpenRecording);


        ///3rd
        rankingButton.onClick.AddListener(ShowRankUI);
        shareButton.onClick.AddListener(OpenSharing);

        if (OUT_FROM_GAMEPLAY)
        {
            // special for cute theme
            SoundManager.Instance.PlayExtensionClip(2);
            FalconMediation.ShowInterstitial("game_over");

            FillDataUI();
            // CheckAutoRating();
        }
        else
        {
            var rand = Helper.RandomIntegerInRange(0, 100);

            if (rand < 50)
            {
                FalconMediation.ShowInterstitial("game_over");
            }

            scoreText.text = GameUnit.Instance.LAST_SCORE.ToString();
            scoreTextBS.text = GameUnit.Instance.LAST_SCORE.ToString();

            recordText.text = GameUnit.Instance.HIGH_SCORE.ToString();
        }

        FalconMediation.HideBanner();
        CollapsibleBanner.Instance.Show();
        // RecordingManager.OnOutRecording += Visible;
    }

    private void OnDestroy()
    {
        // RecordingManager.OnOutRecording -= Visible;
        
        CollapsibleBanner.Instance.Hide();
    }

    /// <summary>
    /// Visible or not
    /// </summary>
    /// <param name="needVisible"></param>
    private void Visible(bool needVisible)
    {
        if (needVisible)
        {
            if (!rootObject.activeInHierarchy)
            {
                camMain.SetActive(true);
                rootObject.SetActive(true);
            }
        }
        else
        {
            if (rootObject.activeInHierarchy)
            {
                camMain.SetActive(false);
                rootObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckAutoRating()
    {
        if (!PlayerPrefs.HasKey(GameKey.READY_FOR_RATE))
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                var playedTimes = PlayerPrefs.GetInt(GameKey.PLAYED_TIMES, 1);

                if (playedTimes >= playedTimeToTrigger)
                {
                    ratePanel.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FillDataUI()
    {
        int score = GameUnit.Instance.SCORE;

        if (score >= 2000) durationText *= 2;

        scoreText.DOTextInt(0, score, durationText);
        scoreTextBS.DOTextInt(0, score, durationText);

        if (GameUnit.FLAG_OVER_HIGHEST)
        {
            recordText.DOTextInt(0, score, durationText);
        }
        else
        {
            recordText.text = GameUnit.Instance.HIGH_SCORE.ToString();
        }

        GameUnit.FLAG_OVER_HIGHEST = false;
    }

    /// <summary>
    /// New gameplay
    /// </summary>
    private void PlayNew()
    {
        UIRanking.IsAllTime = true;
        Global.FLAG_RECORDING = false;
        SoundManager.Instance.PlayClip(AudioType.Click);
        DataHandler.MarkAsShuffled(false);
        DataHandler.MarkAsRotatedShapes(false);
        DataHandler.MarkAsActivatedBoom(false);
        DataHandler.MarkAsRevivaled(false);

        StartCoroutine(LoadPlay());
    }

    private IEnumerator LoadPlay()
    {
        var async = SceneHelper.Instance.LoadSceneAsync(Global.GAMEPLAY_SCENE, LoadSceneMode.Single);

        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                SceneHelper.Instance.StartFadingOnly();
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void ShowRankUI()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        FalconMediation.HideBanner();

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            uiRanking.SetActive(true);
        }
        else
        {
            notify.SetNotification(NotificationObject.NotifyType.Warning, I2.Loc.ScriptLocalization.error_network);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OpenSharing()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        // if (Application.internetReachability != NetworkReachability.NotReachable)
        // {
        //
        //     if (FB.IsInitialized)
        //     {
        //         FB.ActivateApp();
        //         FB.Mobile.ShareDialogMode = ShareDialogMode.AUTOMATIC;
        //
        //         if (FB.IsLoggedIn)
        //         {
        //             ShareFB();
        //         }
        //         else
        //         {
        //             FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, HandleFBLoginResult);
        //         }
        //     }
        //
        // }
        // else
        // {
        //     notify.SetNotification(NotificationObject.NotifyType.Warning, I2.Loc.ScriptLocalization.error_network);
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    // private void HandleFBLoginResult(IResult result)
    // {
    //     if (result == null)
    //     {
    //         Debug.Log("Null Response");
    //         return;
    //     }
    //
    //     // Some platforms return the empty string instead of null.
    //     if (!string.IsNullOrEmpty(result.Error))
    //     {
    //         Debug.Log("Error Response: " + result.Error);
    //     }
    //     else if (result.Cancelled)
    //     {
    //         Debug.Log("Cancelled Response: " + result.RawResult);
    //     }
    //     else if (!string.IsNullOrEmpty(result.RawResult))
    //     {
    //         Debug.Log("Success Response: " + result.RawResult);
    //
    //         ShareFB();
    //     }
    //     else
    //     {
    //         Debug.Log("Empty Response");
    //     }
    // }

    /// <summary>
    /// 
    /// </summary>
    private void ShareFB()
    {
        // if (FB.IsLoggedIn)
        // {
        //     if (string.IsNullOrEmpty(DataCache.FBID))
        //     {
        //         FB.API(Global.QUERY_FB_INFOR, HttpMethod.GET, result =>
        //         {
        //             FBUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<FBUser>(result.RawResult);
        //             var aToken = AccessToken.CurrentAccessToken;
        //
        //             //DataCache.Name = user.name;
        //             DataCache.FBID = user.id;
        //             DataCache.FbImageUrl = "https://graph.facebook.com/" + user.id +
        //                                    "/picture?type=small&width=100&height=100";
        //         });
        //     }
        //
        //     var url = "";
        //     if (Application.platform == RuntimePlatform.IPhonePlayer)
        //     {
        //         url = "https://itunes.apple.com/app/id1457522749";
        //     }
        //     else
        //     {
        //         url = "https://play.google.com/store/apps/details?id=com.os.block.puzzle.pirate.odyssey";
        //     }
        //
        //     FB.ShareLink(
        //         new Uri(url),
        //         callback: HandleShareResult);
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    // private void HandleShareResult(IResult result)
    // {
        // if (result == null)
        // {
        //     Debug.Log("Null Response");
        //     return;
        // }
        //
        // // Some platforms return the empty string instead of null.
        // if (!string.IsNullOrEmpty(result.Error))
        // {
        //     Debug.Log("Error Response: " + result.Error);
        // }
        // else if (result.Cancelled)
        // {
        //     Debug.Log("Cancelled Response: " + result.RawResult);
        // }
        // else if (!string.IsNullOrEmpty(result.RawResult))
        // {
        //     Debug.Log("Success Response: " + result.RawResult);
        // }
        // else
        // {
        //     Debug.Log("Empty Response");
        // }
        //
        // Debug.Log(result.ToString());
    // }

    /// <summary>
    /// Scene,
    /// </summary>
    private void OpenRecording()
    {
        UIRanking.IsAllTime = true;
        Global.FLAG_RECORDING = true;
        SELF_PLAY = true;

        SoundManager.Instance.PlayClip(AudioType.Click);

        Debug.Log("Play record");
        FirebaseLogger.Log(FirebaseLogger.EVENT_PLAY_RECORD);

        StartCoroutine(PlayRecord());
    }

    private IEnumerator PlayRecord()
    {
        var async = SceneHelper.Instance.LoadSceneAsync(Global.GAMEPLAY_SCENE, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                SceneHelper.Instance.StartFadingOnly();
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenChangeTheme()
    {
    }
}