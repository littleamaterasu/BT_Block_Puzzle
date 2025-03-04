using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WarningReplay : MonoBehaviour {

    public GameplayManager gameplay;

    [Header("UI")]
    public Button goAheadBtn;
    public Button neverMindBtn;

    public DG.Tweening.DOTweenAnimation anim;

    private WaitForSeconds _waitClose = new WaitForSeconds(1f);

    private void Start()
    {
        goAheadBtn.onClick.AddListener(ResetGameplay);
        neverMindBtn.onClick.AddListener(NeverMind);
    }

    private void ResetGameplay()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);
        anim.DOPlayBackwards();
        TakeAction().WrapErrors();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NeverMind()
    {
        SoundManager.Instance.PlayClip(AudioType.Click);
        OnTerminate();
    }

    private async Task TakeAction()
    {
        await _waitClose;
        gameObject.SetActive(false);
        gameplay.ResetGameplay();
    }

    /// <summary>
    /// Use for terminate popup
    /// </summary>
    public void OnTerminate()
    {
        anim.DOPlayBackwards();
        StartCoroutine(Auto());
    }

    private IEnumerator Auto()
    {
        yield return new WaitForSeconds(anim.duration);
        gameplay.DontPreventTouch();
        gameObject.SetActive(false);
    }
}
