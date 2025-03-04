using System;
using System.Collections;
using System.Collections.Generic;
using Falcon.FalconMediation.Core;
using UnityEngine;
using SuperScrollView;
using UnityEngine.UI;
using I2.Loc;
using OSNet;

public class UIRanking : MonoBehaviour
{

    public static UIRanking Instance;

    [Space(10)]
    public UITabButton countryBtn;
    public UITabButton worldBtn;

    [Space(10)]
    public Button periodBtn;

    //[Space(10)]
    //public Button leaderboardBtn;

    [Space(10)]
    [Header("Side Action")]
    public Button renamebtn;
    public Button backBtn;

    [Space(5)]
    public LoopListView2 loopList;

    [Space(5)]
    [Header("UI Content")]
    public ScrollRect scrollRect;
    public GameObject content;
    public GameObject loadingObject;
    public GameObject recordLoadingObject;

    public GameObject updatePlayerObject;

    [Space(5)]
    public int numberItemCount = 20;

    [Space(10)]
    public GameObject guide;

    /// setup first time
    private bool _isSetup = false;
    private PlayerData _selfPlayer;


    public static int CurrentList = OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME;
    public static List<PlayerRank>[] AllList = new List<PlayerRank>[4];

    private RegionType _currentRegion = RegionType.COUNTRY;
    public static bool IsAllTime = true;

    private WaitForSeconds _delayShowContent = new WaitForSeconds(0.1f);

    public NotificationObject nofifyObject;

    private void Awake()
    {
        if (Instance != this)
            Instance = this;
    }

    private void OnEnable()
    {
        _selfPlayer = PlayerData.GetPlayerData();

        CreatePlayerIfNotExist();
        Reload();
    }

    // Use this for initialization
    void Start()
    {
        if (!string.IsNullOrEmpty(_selfPlayer.name))
            ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME);

        countryBtn.SetSelect(true);
        countryBtn.AddListener(() =>
        {
            if (IsAllTime)
                ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME);
            else
                ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_WEEK);

            _currentRegion = RegionType.COUNTRY;

            countryBtn.SetSelect(true);
            worldBtn.SetSelect(false);
        });

        worldBtn.SetSelect(false);
        worldBtn.AddListener(() =>
        {
            if (IsAllTime)
                ChooseRanking(OSNet.CSGetTopPlayers.TYPE_WORLD_ALLTIME);
            else
                ChooseRanking(OSNet.CSGetTopPlayers.TYPE_WORLD_WEEK);

            _currentRegion = RegionType.WORLD;

            countryBtn.SetSelect(false);
            worldBtn.SetSelect(true);
        });

        periodBtn.onClick.AddListener(OnChoosePeriodTime);

        backBtn.onClick.AddListener(OnBack);
        renamebtn.onClick.AddListener(OnRename);

        UIRegister.OnDoneRegister += OnDoneRegister;
        OSNet.SCGetTopPlayerRsp.OnGetTopPlayers += OnGetTopPlayers;
    }

    private void OnDestroy()
    {
        UIRegister.OnDoneRegister -= OnDoneRegister;
        OSNet.SCGetTopPlayerRsp.OnGetTopPlayers -= OnGetTopPlayers;

        for (int i = 0; i < AllList.Length; ++i)
            AllList[i] = null;

        CurrentList = OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME;
    }

#if UNITY_ANDROID
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!updatePlayerObject.activeInHierarchy && !guide.activeInHierarchy)
            {
                gameObject.SetActive(false);
                
                Debug.LogError("UIRanking > show banner update");

                FalconMediation.ShowBanner();
            }
        }
    }
#endif


    /// <summary>
    /// Setup scroll, once execute when networking is calling. Callback goes here
    /// </summary>
    private void SetupScrollView()
    {
        loadingObject.SetActive(false);

        if (_isSetup) return;

        loopList.InitListView(numberItemCount, OnGetItemByIndex);

        _isSetup = true;
    }

    /// <summary>
    /// After setup scroll, we need to call this to refresh data of scroll view.
    /// Delay show after 0.1 seconds
    /// </summary>
    /// <param name="count"></param>
    private void ShowContent(int count)
    {
        StartCoroutine(DelayShowContentLeaderboard(count));
    }

    /// <summary>
    /// A callback when initialize a item row, useful for fill data
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0)
        {
            return null;
        }

        //create one row
        LoopListViewItem2 item = listView.NewListViewItem("ItemRanking");
        var itemScript = item.GetComponent<ItemRanking>();

        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
        }

        if (index >= numberItemCount || index >= AllList[CurrentList].Count)
        {
            itemScript.gameObject.SetActive(false);
        }
        else
        {
            itemScript.SetData(index);
        }

        return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator DelayShowContentLeaderboard(int count)
    {
        if (scrollRect.gameObject.activeSelf)
        {
            loopList.RefreshAllShownItemWithFirstIndex(0);  // refresh all shown item
            loopList.SetListItemCount(count);               // set new list count
        }

        yield return _delayShowContent;

        if (scrollRect.gameObject.activeSelf)
        {
            scrollRect.verticalNormalizedPosition = 1;      // scroll update to top view
            content.SetActive(true);                        // content is now available

            if (!PlayerPrefs.HasKey(GameKey.SEEN_GUIDE))
            {
                guide.SetActive(true);
                AntiCheat.SetInt(GameKey.SEEN_GUIDE, 1);
            }
        }
    }

    /// <summary>
    /// When choose ranking type, net-working takes action.
    /// When callback is comes show content (We need a callback) or never
    /// </summary>
    /// <param name="type"></param>
    private void ChooseRanking(int type)
    {
        loadingObject.SetActive(true);
        content.SetActive(false);

        CurrentList = type;

        if (!string.IsNullOrEmpty(_selfPlayer.name))
        {
            if (AllList[type] == null)
            {

                if (Application.internetReachability != NetworkReachability.NotReachable)
                    new OSNet.CSGetTopPlayers(type).Send();
                else
                    OnNetworkUnavailable();

            }
            else
            {
                SetupScrollView();
                ShowContent(AllList[type].Count);
            }
        }
        else
        {
            //nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.error);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void OnGetTopPlayers(int type, List<PlayerRank> list)
    {

        if (AllList[type] == null)
        {
            if (list.Count > 0)
            {
                AllList[type] = list;
                SetupScrollView();
                ShowContent(AllList[type].Count);
            }
            else
            {
                nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.empty_data);
            }
        }
    }

    private void OnNetworkUnavailable()
    {
        nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.error_network);
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreatePlayerIfNotExist()
    {

        if (string.IsNullOrEmpty(_selfPlayer.name))
        {
            // open dialog
            content.SetActive(false);
            updatePlayerObject.SetActive(true);
        }
    }

    /// <summary>
    /// When hit the name
    /// </summary>
    /// <param name="username"></param>
    public void OnDoneRegister(string username)
    {

        if (string.IsNullOrEmpty(username))
        {
            if (string.IsNullOrEmpty(_selfPlayer.name))
                nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.information_not_created);
            return;
        }

        // create
        if (string.IsNullOrEmpty(_selfPlayer.name))
        {
            _selfPlayer.name = username;
            DataCache.Name = username;

            AccountManager.Instance.UpdateToServer("register_name");
            ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME);
        }
        else   /// update name
        {
            if (username.Equals(_selfPlayer.name))
            {
                // update name is still the same
                nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.error_change);

            }
            else
            {

                _selfPlayer.name = username;
                DataCache.Name = username;

                loadingObject.SetActive(true);
                // change name
                AccountManager.Instance.UpdateToServer("change_name");

                StartCoroutine(DelayReloadLeaderboard());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayReloadLeaderboard()
    {
        yield return new WaitForSeconds(0.3f);
        ForceReload();
    }

    /// <summary>
    /// Reload current
    /// </summary>
    private void Reload()
    {
        switch (_currentRegion)
        {
            case RegionType.COUNTRY:

                if (IsAllTime)
                    ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_ALLTIME);
                else
                    ChooseRanking(OSNet.CSGetTopPlayers.TYPE_LOCAL_WEEK);

                break;
            case RegionType.WORLD:

                if (IsAllTime)
                    ChooseRanking(OSNet.CSGetTopPlayers.TYPE_WORLD_ALLTIME);
                else
                    ChooseRanking(OSNet.CSGetTopPlayers.TYPE_WORLD_WEEK);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Force Reload and assign
    /// </summary>
    private void ForceReload()
    {
        content.SetActive(false);

        for(int i = 0; i < 4; ++i)
            AllList[i] = null;

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            new OSNet.CSGetTopPlayers(CurrentList).Send();

            nofifyObject.SetNotification(NotificationObject.NotifyType.Information, ScriptLocalization.change_name_successfully);
        }
        else
            OnNetworkUnavailable();

    }

    /// <summary>
    /// Choose priod time: Weekly or All Time. Refresh data.
    /// </summary>
    private void OnChoosePeriodTime()
    {
        IsAllTime = !IsAllTime;
        Reload();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBack()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);
        gameObject.SetActive(false);
        Debug.LogError("UIRanking > show banner onback");
        FalconMediation.ShowBanner();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnRename()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        updatePlayerObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    public void StartGetPlayerData(int code)
    {

        new CSGetGameStats(IsAllTime ? CSGetTopPlayers.TYPE_WORLD_ALLTIME : CSGetTopPlayers.TYPE_WORLD_WEEK, code).Send();
        StartCoroutine(TimedOut());
    }

    private IEnumerator TimedOut()
    {
        yield return new WaitForSeconds(3.0f);

        if (recordLoadingObject.activeInHierarchy)
        {
            recordLoadingObject.SetActive(false);
            nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.server_error);
        }
    }
}
