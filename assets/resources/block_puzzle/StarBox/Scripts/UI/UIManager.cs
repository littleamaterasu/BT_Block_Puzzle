
using UnityEngine;
using UnityEngine.UI;
using DOTweenExtension;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Text")]
    public TextMeshProUGUI highScoreText;
    [Space(5)]
    public TextMeshProUGUI scoreText;

    [Space(15)]
    public Button bgmButton;
    public Button soundButton;

    [SerializeField]
    private float _durationScoring = 0.3f;

    [Space(10)]
    [Header("Shake Configuration")]
    [Range(0.1f, 1.0f)]
    public float shakeDuration;
    public Vector3[] shakeStrength = new Vector3[3];

    [Space(10)]
    [Header("Combo Linker")]
    public ComboManager comboManager;

    [Space(30)]
    [SerializeField]
    public Camera _mainCamera;  // cache this, so we dont need to call Camera.main

    private int _tempHighScore = 0;

    private void Start()
    {

        bgmButton.onClick.AddListener(OnMusic);
        soundButton.onClick.AddListener(OnSoundSfx);

        if (SoundManager.AudioSettings == 0)
            bgmButton.ChangeState(ButtonStateVisualizer.State.OnDisable);

        if (SoundManager.SoundSettings == 0)
            soundButton.ChangeState(ButtonStateVisualizer.State.OnDisable);

        scoreText.text = GameUnit.Instance.LAST_SCORE.ToString();
        highScoreText.text = GameUnit.Instance.HIGH_SCORE.ToString();

        if(!Global.FLAG_RECORDING)
            GameUnit.OnScoreChange += ActionScore;
    }

    private void OnDestroy()
    {
        if (!Global.FLAG_RECORDING)
            GameUnit.OnScoreChange -= ActionScore;
    }
  


    /// <summary>
    /// Action for scoring
    /// </summary>
    /// <param name="combo"></param>
    /// <param name="score"></param>
    /// <param name="valueChange"></param>
    private void ActionScore(int score, int valueChange)
    {
        if (score == 0 && valueChange == 0)
        {
            if (_tempHighScore > 0 && GameUnit.Instance.SCORE > 0)
            {
                scoreText.DOTextInt(GameUnit.Instance.SCORE, 0, 0.5f);
            }

            if (_tempHighScore > GameUnit.Instance.HIGH_SCORE)
            {
                highScoreText.DOTextInt(_tempHighScore, GameUnit.Instance.HIGH_SCORE, 0.5f);
            }

            _tempHighScore = GameUnit.Instance.HIGH_SCORE;
            return;
        }

        int combo = GameUnit.Instance.COMBO;

        if (combo >= 1)
        {
            comboManager.GenFloatingText(valueChange);

            SoundManager.Instance.PlayClip(AudioType.Combo);
            if (combo == 1)
            {
                //
                Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[0]);
            }
            else
            {
                if (combo == 2)
                    Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[1]);
                else
                    Helper.ShakeCamera(_mainCamera, shakeDuration, shakeStrength[2]);

                comboManager.GenCombo(combo - 2);
            }
        }

        if((score + valueChange) != GameUnit.Instance.LAST_SCORE)
            scoreText.DOTextInt(score, (score + valueChange), _durationScoring);

        if(_tempHighScore == 0)
        {
            _tempHighScore = GameUnit.Instance.HIGH_SCORE;
        }


        if((score + valueChange) > _tempHighScore)
        {
            var delta = (score + valueChange) - _tempHighScore;
            highScoreText.DOTextInt(_tempHighScore, (_tempHighScore + delta), _durationScoring);
            _tempHighScore += delta;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMusic()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        if (SoundManager.AudioSettings == 1)
        {
            SoundManager.Instance.StopBGM();
            bgmButton.ChangeState(ButtonStateVisualizer.State.OnDisable);
        } else
        {
            SoundManager.Instance.ForcePlayBGM();
            bgmButton.ChangeState(ButtonStateVisualizer.State.OnEnable);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnSoundSfx()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        if(SoundManager.Instance.OnOffSoundFx())
            soundButton.ChangeState(ButtonStateVisualizer.State.OnEnable);
        else
            soundButton.ChangeState(ButtonStateVisualizer.State.OnDisable);
    }

    public void ChangeThemeTest()
    {
        
    }
}
