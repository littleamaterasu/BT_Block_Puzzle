using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DOTweenExtension;
using TMPro;
using UnityEngine.SceneManagement;

public class RecordingManager : MonoBehaviour {

 //
 //    [Space(10)]
 //    public GameObject staticRegularUI;
 //    public GameObject dynamicRegularUI;
 //
 //    [Space(10)]
 //    public GameObject staticRecordingUI;
 //    public GameObject dynamicRecordingUI;
 //
 //    [Space(10)]
 //    public TextMeshProUGUI stepCountText;
 //    public TextMeshProUGUI scoreTextRecord;
 //
 //    public Button X1Rate; 
 //    public Button X2Rate; 
 //    public Button X4Rate;
 //
 //    [Header("Extending UI recording")]
 //    public GameObject endingFocus;
 //    public Text playerFinished;
 //    public static string PlayerName;
 //
 //    [Space(10)]
 //    [SerializeField]
 //    private float _durationScoring = 0.3f;
 //
 //    public float speedRate = 1.0f;
 //
 //    [Header("Linker")]
 //    public GameplayManager gameplay;
 //    public StepProgress stepProgress;
 //
 //    [Space(10)]
 //    [Header("Shake Configuration")]
 //    [Range(0.1f, 1.0f)]
 //    public float shakeDuration;
 //    public Vector3[] shakeStrength = new Vector3[3];
 //
 //    [Space(10)]
 //    [Header("Combo Linker")]
 //    public ComboManager comboManager;
 //
 //    [Space(30)]
 //    [SerializeField]
 //    public Camera _mainCamera;
 //    private bool _doneRecord;
 //
 //    private int _stepLength = 0;
 //
 //    private bool _initialized = false;
 //    private bool _isDead;
 //
 //    public static System.Action<bool> OnOutRecording;
 //
 //    private void Awake()
 //    {
 //        if (!Global.FLAG_RECORDING)
 //        {
 //            enabled = false;
 //            gameObject.SetActive(false);
 //        } else {
 //            _initialized = true;
 //        }
 //
 //        if (_initialized)
 //        {
 //            staticRegularUI.SetActive(false);
 //            dynamicRegularUI.SetActive(false);
 //
 //            staticRecordingUI.SetActive(true);         
 //            dynamicRecordingUI.SetActive(true);         
 //        }
 //        
 //    }
 //
 //    private void Start()
 //    {
 //        if (_initialized)
 //        {
 //            if (!UIGameOver.SELF_PLAY)
 //            {
 //                OnOutRecording?.Invoke(false);
 //                SceneManager.SetActiveScene(gameObject.scene);
 //            }
 //
 //            GameUnit.OnScoreChange += ActionScore;
 //            GameplayManager.OnStepChange += OnStepChange;
 //            ConfigGameplay();
 //        }
 //
 //        X1Rate.onClick.AddListener(() => {
 //            SoundManager.Instance.PlayClip(AudioType.Click);
 //
 //            X1Rate.interactable = false;
 //            X2Rate.interactable = true;
 //            X4Rate.interactable = true;
 //            ChangeSpeedRate(speedRate);
 //        });
 //
 //        X2Rate.onClick.AddListener(() => {
 //            SoundManager.Instance.PlayClip(AudioType.Click);
 //
 //            X1Rate.interactable = true;
 //            X2Rate.interactable = false;
 //            X4Rate.interactable = true;
 //            ChangeSpeedRate(speedRate / 2);
 //        });
 //
 //        X4Rate.onClick.AddListener(() => {
 //            SoundManager.Instance.PlayClip(AudioType.Click);
 //
 //            X1Rate.interactable = true;
 //            X2Rate.interactable = true;
 //            X4Rate.interactable = false;
 //            ChangeSpeedRate(speedRate / 4);
 //        });
 //        
 //        X4Rate.onClick.Invoke();
 //    }
 //
 //    private void OnDestroy()
 //    {
 //        if (_initialized)
 //        {
 //            GameUnit.OnScoreChange -= ActionScore;
 //            GameplayManager.OnStepChange -= OnStepChange;
 //        }
 //    }
 //
 //    /// <summary>
 //    /// 
 //    /// </summary>
 //    private void ConfigGameplay()
 //    {
 //        GameProgress gp;
 //        if (UIGameOver.SELF_PLAY)
 //        {
 //            gp = GameUnit.Instance.HighestProgress;
 //        }
 //        else /// play other
 //        {
 //            gp = GameUnit.Instance.OtherProgress;
 //        }
 //
 //        if(gp != null)
 //            _stepLength = gp.length;
 //
 //        stepCountText.text = "0/" + _stepLength;
 //
 //        gameplay.DisableTouchAllBoard();
 //        gameplay.SetUpGamePlay(speedRate, gp);
 //    }
 //
 //    /// <summary>
 //    /// 
 //    /// </summary>
 //    /// <param name="rate"></param>
 //    private void ChangeSpeedRate(float rate)
 //    {
 //        gameplay.ChangeSpeedRate(rate);
 //    }
 //
 //    /// <summary>
 //    /// 
 //    /// </summary>
 //    public void OutRecordingPlay()
 //    {
 //        SoundManager.Instance.PlayClip(AudioType.Click);
 //
 //        if (_isDead) return;
 //
 //        _isDead = true;
 //
 //        GameUnit.Instance.COMBO = 0;
 //        System.GC.Collect();
 //
 //        UIGameOver.OUT_FROM_GAMEPLAY = false;
 //        if (!UIGameOver.SELF_PLAY)
 //        {
 //            AdsManager.Instance.HideBanner();
 //            SceneManager.UnloadSceneAsync(Global.GAMEPLAY_SCENE);
 //            OnOutRecording?.Invoke(true);
 //        }
 //        else
 //        {
	// 		StartCoroutine(LoadHome());
 //
 //            if(_doneRecord)
 //                AdsManager.Instance.ShowBanner();
 //        }
 //    }
 //
	// private IEnumerator LoadHome()
	// {
	// 	var async = SceneHelper.Instance.LoadSceneAsync(Global.GAME_OVER_SCENE, LoadSceneMode.Single);
 //
	// 	async.allowSceneActivation = false;
 //
	// 	while (!async.isDone) {
	// 		if (async.progress >= 0.9f) {
	// 			SceneHelper.Instance.StartFadingOnly ();
	// 			async.allowSceneActivation = true;
	// 		}
 //
	// 		yield return null;
	// 	}
	// }
 //
 //
 //    /// <summary>
 //    /// 
 //    /// </summary>
 //    /// <param name="step"></param>
 //    private void OnStepChange(int step)
 //    {
 //        if(step < 0)
 //        {
 //            _doneRecord = true;
 //            StartCoroutine(WaitEndingFocus(haveStep: false));
 //            return;
 //        }
 //
 //        var showedStep = step + 1;
 //        stepCountText.text = showedStep + "/" + _stepLength;
 //
 //        if(showedStep == _stepLength)
 //        {
 //            FirebaseLogger.Log(FirebaseLogger.EVENT_FULL_PLAY_RECORD);
 //        }
 //
 //        if (stepProgress)
 //        {
 //            var percent = Mathf.Clamp01((float)showedStep / _stepLength);
 //
 //            stepProgress.Move(percent);
 //        }
 //
 //        if(showedStep >= _stepLength)
 //        {
 //            _doneRecord = true;
 //            StartCoroutine(WaitEndingFocus());
 //        }
 //    }
 //
 //    /// <summary>
 //    /// 
 //    /// </summary>
 //    /// <returns></returns>
 //    private IEnumerator WaitEndingFocus(bool haveStep = true)
 //    {
 //        AdsManager.Instance.HideBanner();
 //
 //        if(haveStep)
 //            yield return new WaitForSeconds(1.75f);
 //        else
 //            yield return new WaitForSeconds(0.75f);
 //
 //        if (!endingFocus.activeInHierarchy)
 //        {
 //            if (haveStep)
 //            {
 //                if (UIGameOver.SELF_PLAY)
 //                {
 //                    playerFinished.text = I2.Loc.ScriptLocalization.self_record_finished;
 //                }
 //                else
 //                {
 //                    string show = string.Format(I2.Loc.ScriptLocalization.player_record_finished, PlayerName);
 //                    playerFinished.text = show;
 //                }
 //            } else
 //            {
 //                if (UIGameOver.SELF_PLAY)
 //                {
 //                    playerFinished.text = I2.Loc.ScriptLocalization.self_no_progress;
 //                }
 //                else
 //                {
 //                    string show = string.Format(I2.Loc.ScriptLocalization.player_no_progress, PlayerName);
 //                    playerFinished.text = show;
 //                }
 //            }
 //
 //            endingFocus.SetActive(true);
 //        }
 //    }
 //
 //    /// <summary>
 //    /// Action for scoring
 //    /// </summary>
 //    /// <param name="score"></param>
 //    /// <param name="valueChange"></param>
 //    private void ActionScore(int score, int valueChange)
 //    {
 //        int combo = GameUnit.Instance.COMBO;
 //
 //        if (combo >= 1)
 //        {
 //            SoundManager.Instance.PlayClip(AudioType.Combo);
 //
 //            comboManager.GenFloatingText(valueChange);
 //
 //            if (combo == 1)
 //            {
 //                //
 //                Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[0]);
 //            }
 //            else
 //            {
 //                if (combo == 2)
 //                    Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[1]);
 //                else
 //                    Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[2]);
 //
 //                comboManager.GenCombo(combo - 2);
 //            }
 //        }
 //
 //        scoreTextRecord.DOTextInt(score, score + valueChange, _durationScoring);
 //    }

}
