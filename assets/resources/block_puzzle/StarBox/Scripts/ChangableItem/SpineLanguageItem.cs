using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineLanguageItem : MonoBehaviour {

    public string[] animLangName;

    [SerializeField]
    private SkeletonGraphic _graphic;

    private void Start()
    {
        if (_graphic)
            _graphic.AnimationState.SetAnimation(0, animLangName[(int)Global.LANG], false);
    }
}
