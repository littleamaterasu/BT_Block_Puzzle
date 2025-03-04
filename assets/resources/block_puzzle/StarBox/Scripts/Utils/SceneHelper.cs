
using UnityEngine;
using System.Collections;
using UnityEngine.Internal;

using UnityEngine.SceneManagement;

public class SceneHelper : OneSoftGame.Tools.PersistentSingleton<SceneHelper> {

    [SerializeField]
    private GameObject _fader;

	[SerializeField]
	private float _delaySample = 0.15f;

	[SerializeField]
	private DG.Tweening.DOTweenAnimation _anim;

    public AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode, bool needFadingImmediately = true)
    {
		if (needFadingImmediately) {
			_fader.SetActive (true);
		}

        return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
    }


    public AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode, bool needFadingImmediately = true)
    {
		if (needFadingImmediately) {
			_fader.SetActive (true);
		}

        return SceneManager.LoadSceneAsync(sceneName, mode);     
    }

	public void StartCoroutinePlayAdditive()
	{
		StartCoroutine (LoadPlayAdditive ());
	}

	private IEnumerator LoadPlayAdditive()
	{
		var async = LoadSceneAsync(Global.GAMEPLAY_SCENE, LoadSceneMode.Additive);

		async.allowSceneActivation = false;

		while (!async.isDone) {
			if (async.progress >= 0.9f) {
				StartFadingOnly ();
				async.allowSceneActivation = true;
			}

			yield return null;
		}
	}

    public void StartFading()
    {
        _fader.SetActive(true);
		_anim.delay = _delaySample;
		_anim.DOPlay ();
    }

	public void StartFadingOnly()
	{
		_anim.delay = _delaySample;
		_anim.DOPlay ();
	}
}
