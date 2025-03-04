#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class FixCamera : MonoBehaviour
{
    private Camera _cam;
    private static float _width = 7.2f;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        FixCam();
    }

    private void FixCam()
    {

        float currentAspect = (float)Screen.width / (float)Screen.height;
        if (currentAspect >= 0.68f) return;
        var old_size = _cam.orthographicSize;
        var h_w = old_size * currentAspect;
        _cam.orthographicSize = (_width / (h_w * 2)) * old_size;

    }
}