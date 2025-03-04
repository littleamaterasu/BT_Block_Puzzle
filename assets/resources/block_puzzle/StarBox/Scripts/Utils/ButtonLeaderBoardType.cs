using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonLeaderBoardType : MonoBehaviour
{

    Button _selfButton;
    Image _selfImage;

    [Space(5)]
    public Sprite[] cuteWeeklySprite;
    public Sprite[] cuteAllTimeSprite;
    
    int langId;

    bool firstTimeSet = false;

    bool isAllTime = true;
    private void Awake()
    {
        _selfButton = GetComponent<Button>();
        _selfImage = GetComponent<Image>();
    }
    // Use this for initialization
    void OnEnable()
    {
        langId = (int)Global.LANG;

        _selfButton.onClick.AddListener(ChangeType);
        
        _selfImage.sprite = UIRanking.IsAllTime ? cuteAllTimeSprite[langId] : cuteWeeklySprite[langId];
        isAllTime = UIRanking.IsAllTime;
    }

    private void OnDisable()
    {
        _selfButton.onClick.RemoveListener(ChangeType);
    }

    void ChangeType()
    {
        isAllTime = !isAllTime;
        _selfImage.sprite = isAllTime ? cuteAllTimeSprite[langId] : cuteWeeklySprite[langId];
    }
}