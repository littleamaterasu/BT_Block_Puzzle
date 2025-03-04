using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReposition : MonoBehaviour
{

    public float designOrthoSize = 6.4f;
    public bool ignoreBottom = false;
    private Vector3 _originPosition;

    private void Awake()
    {
        _originPosition = transform.position;

        var originY = Mathf.Abs(_originPosition.y);

        float inverseOriginY = designOrthoSize - originY;

        var main = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        float half = main.orthographicSize;

        var newY = (half - inverseOriginY) * Mathf.Sign(_originPosition.y);

        if (!ignoreBottom)
        {
            if (Global.CAM_ASPECT >= 0.42f && Global.CAM_ASPECT <= 0.503f)
            {
                //IpX
                newY += 1.25f;
            }
            else
            {
                newY += 0.4f;
            }
        }
        else
        {
            if (Global.CAM_ASPECT >= 0.42f && Global.CAM_ASPECT <= 0.503f)
            {
                //IpX
                newY += 0.4f;
            }
        }

        transform.position = new Vector3(_originPosition.x, newY, _originPosition.z);
    }
}
