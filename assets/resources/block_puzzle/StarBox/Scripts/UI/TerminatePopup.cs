using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TerminatePopup : MonoBehaviour, IPointerClickHandler
{
    public GameObject popupOwner;

    [Space(10)]
    public bool extendEventOnly = false;

    [Space(10)]
    public UnityEvent extendEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (popupOwner && popupOwner.activeInHierarchy && !extendEventOnly)
        {
            popupOwner.SetActive(false);
        }

        extendEvent.Invoke();
    }

#if UNITY_ANDROID
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupOwner && popupOwner.activeInHierarchy && !extendEventOnly)
            {
                popupOwner.SetActive(false);
            }

            extendEvent.Invoke();
        }
    }

#endif
}
