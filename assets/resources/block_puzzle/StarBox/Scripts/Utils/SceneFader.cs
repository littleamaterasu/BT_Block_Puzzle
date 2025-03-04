using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class SceneFader : MonoBehaviour
{
    #region FIELDS
    public Image fadeImage;

    public bool fadeOnEnable = false;

    [Range(0.1f, 3f)]
    public float fadeSpeed = 0.8f;

    public FadeDirection direction = FadeDirection.Out;

    public enum FadeDirection
    {
        In, //Alpha = 1
        Out // Alpha = 0
    }

    #endregion
    #region MONOBHEAVIOR
    void OnEnable()
    {
        if(fadeOnEnable)
            StartCoroutine(Fade(direction));
    }
    #endregion
    #region FADE
    private AsyncOperation FadeAsync(FadeDirection fadeDirection, string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Single)
    {
        StartCoroutine(Fade(fadeDirection));
        return SceneManager.LoadSceneAsync(sceneToLoad, mode);
    }

    private IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            fadeImage.enabled = false;
        }
        else
        {
            fadeImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }
    #endregion

    #region HELPERS
    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(sceneToLoad);
    }
    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
    #endregion
}
