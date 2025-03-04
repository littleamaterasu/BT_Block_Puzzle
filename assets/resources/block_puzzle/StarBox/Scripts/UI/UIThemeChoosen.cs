using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIThemeChoosen : MonoBehaviour
{

    public Button cuteThemeButton;
    public Button woodieThemeButton;

    [Space(10)]
    public float offsetY = 110f;
    public Transform selectedText;

    [Space(10)]
    public bool isChangeInstantly = true;
    public Button confirmButton;

    [Space(10)]
    public GameObject parent;

    private DG.Tweening.DOTweenAnimation _anim;
    private int _currentThemeId = -1;

    private Vector3 _woodieOriginPos;
    private Vector3 _cuteOriginPos;

    private void Awake()
    {
        _anim = GetComponent<DG.Tweening.DOTweenAnimation>();
        _woodieOriginPos = woodieThemeButton.transform.localPosition;
        _cuteOriginPos = cuteThemeButton.transform.localPosition;
    }

    private void OnEnable()
    {
        Setup(1);

        StartCoroutine(WaitAnim());
    }

    private IEnumerator WaitAnim()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        _anim.DOPlay();
    }

    private void Start()
    {
        cuteThemeButton.onClick.AddListener(() =>
        {
            ChangeTheme(Theme.Cute);
        });

        woodieThemeButton.onClick.AddListener(() =>
        {
            ChangeTheme(Theme.Woodie);
        });

        confirmButton.onClick.AddListener(Confirm);
    }

    /// <summary>
    /// Setup visualize
    /// </summary>
    /// <param name="themeID"></param>
    private void Setup(int themeID)
    {
        if (_currentThemeId == themeID) return;

        _currentThemeId = themeID;
        var theme = (Theme)themeID;

        switch (theme)
        {
            case Theme.Woodie:
                woodieThemeButton.interactable = false;
                cuteThemeButton.interactable = true;

                selectedText.localPosition = new Vector3(_woodieOriginPos.x, _woodieOriginPos.y - offsetY, 0f);

                break;
            case Theme.Cute:
                woodieThemeButton.interactable = true;
                cuteThemeButton.interactable = false;

                selectedText.localPosition = new Vector3(_cuteOriginPos.x, _cuteOriginPos.y - offsetY, 0f);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="themeID"></param>
    private void ChangeTheme(Theme theme)
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        Setup((int)theme);

        if (isChangeInstantly)
            ThemeManager.Instance.ChangeThemeInstantly(theme);
        else
            ThemeManager.Instance.ChangeAndReload(theme);

    }

    /// <summary>
    /// 
    /// </summary>
    private void Confirm()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);

        FirebaseLogger.Log(FirebaseLogger.EVENT_CHOOSE_CUTE);

        _anim.DOPlayBackwards();
        StartCoroutine(Auto());
    }

    /// <summary>
    /// Use for terminate popup
    /// </summary>
    public void OnTerminate()
    {
        _anim.DOPlayBackwards();
        StartCoroutine(Auto());
    }

    private IEnumerator Auto()
    {
        yield return new WaitForSeconds(_anim.duration);
        parent.SetActive(false);
    }
}
