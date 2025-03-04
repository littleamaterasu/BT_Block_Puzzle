
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using TMPro;

public class ItemRanking : MonoBehaviour
{
    public Text usernameLb;
    public TextMeshProUGUI rankLb;

    public RectTransform rectTranPartChange;
    public RectTransform rectTransformRankLabel;

    public Vector3[] positionPartChanged;

    public Vector4[] posAndSizeRankLabel;

    public GameObject[] rankImage;

    public Image itemBackgroundCuteTheme;

    public Sprite[] backgroundCuteThemes;

    public TextMeshProUGUI scoreLb;

    public Image countryImage;

    private PlayerRank _selfPlayer;
    private int _rank = -1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void SetData(int index)
    {
        // set player here

        if (UIRanking.AllList[UIRanking.CurrentList].Count > 0)
        {
            _selfPlayer = UIRanking.AllList[UIRanking.CurrentList][index];

            usernameLb.text = _selfPlayer.name;

            if (AccountManager.Instance.Code == _selfPlayer.code)
            {
                usernameLb.color = Color.green;
            }
            else
            {
                usernameLb.color = Color.white;
            }

            _rank = _selfPlayer.rank;
            rankLb.text = _rank.ToString();

            scoreLb.text = _selfPlayer.score.ToString();

            if (!string.IsNullOrEmpty(_selfPlayer.country))
            {
                countryImage.sprite = DataHandler.Instance.GetCountryImage(_selfPlayer.country);
            }
            

            var top = (_rank >= 1 && _rank <= 3);

            for (int i = 0; i < 3; ++i)
            {
                rankImage[i].SetActive(false);
            }

            //Reset Ui element on item <Dat>
            Vector4 vec4 = posAndSizeRankLabel[0];

            itemBackgroundCuteTheme.sprite = backgroundCuteThemes[3];
            rectTranPartChange.localPosition = positionPartChanged[0];

            itemBackgroundCuteTheme.gameObject.SetActive(false);

            rectTransformRankLabel.localPosition = new Vector2(vec4.x, vec4.y);
            rectTransformRankLabel.sizeDelta = new Vector2(vec4.z, vec4.w);


            if (top)
            {
                rankImage[_rank - 1].SetActive(true);
                itemBackgroundCuteTheme.sprite = backgroundCuteThemes[_rank - 1];
                rectTranPartChange.localPosition = positionPartChanged[1];
            }
            else
            {
                vec4 = posAndSizeRankLabel[1];

                rectTransformRankLabel.localPosition = new Vector2(vec4.x, vec4.y);
                rectTransformRankLabel.sizeDelta = new Vector2(vec4.z, vec4.w);
            }

            itemBackgroundCuteTheme.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Play record with data
    /// </summary>
    public void PlayRecord()
    {
        // SoundManager.Instance.PlayClip(AudioType.Click);
        //
        // // call loading here
        // if (Application.internetReachability != NetworkReachability.NotReachable)
        // {
        //     // get data via id
        //     if (_selfPlayer != null)
        //     {
        //         UIRanking.Instance.recordLoadingObject.SetActive(true);
        //
        //         RecordingManager.PlayerName = _selfPlayer.name;
        //
        //         FirebaseLogger.Log(FirebaseLogger.EVENT_PLAY_LEADER_BOARD_RECORD);
        //         UIRanking.Instance.StartGetPlayerData(_selfPlayer.code);
        //         
        //     }
        // } else
        // {
        //     UIRanking.Instance.nofifyObject.SetNotification(NotificationObject.NotifyType.Warning, ScriptLocalization.error_network);
        // }
    }

}
