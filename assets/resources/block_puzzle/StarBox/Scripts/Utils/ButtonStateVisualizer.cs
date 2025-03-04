using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStateVisualizer : MonoBehaviour {
    
    private Image _self;

    public bool isTwoState = true;
    public Image imgEnable;
    public Image imgDisable;

    public Sprite onExtendStateVisual;

    private State _currentState = State.OnEnable;

    public enum State
    {
        OnEnable,
        OnDisable,
        OnExtend
    }

    /// <summary>
    /// Configuration of this button
    /// </summary>
    /// <param name="whichState"></param>
    public void ChangeState(State state)
    {
        if (_currentState == state) return;

        if(_self == null)
            _self = GetComponent<Image>();

        switch (state)
        {
            case State.OnEnable:
                imgEnable.enabled = true;
                imgDisable.enabled = false;
                break;

            case State.OnDisable:
                imgEnable.enabled = false;
                imgDisable.enabled = true;
                break;

            case State.OnExtend:
                if(!isTwoState)
                    _self.sprite = onExtendStateVisual;
                break;

            default:
                break;
        }

        _currentState = state;
    }
}

public static class ButtonState
{

    public static void ChangeState(this Button button, ButtonStateVisualizer.State state)
    {
        var visualizer = button.GetComponent<ButtonStateVisualizer>();

        if (visualizer)
            visualizer.ChangeState(state);
    }
}
