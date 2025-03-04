using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour {


    public GameObject iconHand;
    public GameObject highlightText;


    private Button _recordButton;

    private void Awake()
    {
        var enable = !GameUnit.Instance.isHighlightShown;

        if (enable)
        {
            _recordButton = GetComponent<Button>();
            
            iconHand.SetActive(true);
            highlightText.SetActive(true);

        } else
        {
            enabled = false;
        }
    }

    private void Start()
    {
        if (_recordButton)
        {
            _recordButton.onClick.AddListener(() =>
            {
                if (iconHand.activeInHierarchy)
                {
                    PlayerPrefs.SetInt(GameKey.FIRST_OVER_HIGH_SCORE, 1);
                    GameUnit.Instance.isHighlightShown = true;

                    iconHand.SetActive(false);
                    DG.Tweening.DOTween.Kill(iconHand);

                    highlightText.SetActive(false);
                    DG.Tweening.DOTween.Kill(highlightText);
                }
            });
        }
    }
}
