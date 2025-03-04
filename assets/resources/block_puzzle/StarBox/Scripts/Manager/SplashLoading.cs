using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoading : MonoBehaviour
{
    [Range(30, 60)]
    public int targetFPS = 60;

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = targetFPS;

        StartCoroutine(LoadLoadingScene());
    }

    private IEnumerator LoadLoadingScene()
    {

        AsyncOperation _async = SceneHelper.Instance.LoadSceneAsync(Global.LOADING_SCENE, LoadSceneMode.Single, needFadingImmediately: false);
        _async.allowSceneActivation = false;

        while (_async.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        SceneHelper.Instance.StartFading();

        _async.allowSceneActivation = true;

    }
}
