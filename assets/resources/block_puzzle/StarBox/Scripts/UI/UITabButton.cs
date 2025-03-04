
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UITabButton : MonoBehaviour
{
    public GameObject goSelected;
    
    private Button _self;

    private void Awake()
    {
        _self = GetComponent<Button>();
    }

    public void SetSelect(bool value)
    {
        goSelected.SetActive(value);
    }

    public void AddListener(UnityAction action)
    {
        if (action == null) return;
        _self.onClick.AddListener(action);
    }
}
