
using System.Collections;
using UnityEngine;

public class AutoDespawn : MonoBehaviour
{

    [Range(0.1f, 3f)]
    public float existTime = 1.0f;
    private WaitForSeconds _waitTime;

    private void OnEnable()
    {
        if (_waitTime == null)
            _waitTime = new WaitForSeconds(existTime);

        StartCoroutine(GoPool());
    }

    private IEnumerator GoPool()
    {
        yield return _waitTime;
        gameObject.SetActive(false);
    }

}
