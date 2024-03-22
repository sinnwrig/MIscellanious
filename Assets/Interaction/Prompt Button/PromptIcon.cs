using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class PromptIcon : MonoBehaviour
{
    public RectTransform promptRect;
    public Image icon;
    public Text iconText;
    public Text prompt;


    public UnityEvent onActivated;

    public float deactivationTime = 0.5f;
    public UnityEvent onDeactivated;


    public UnityEvent onEnterInteract;
    public UnityEvent onLeaveInteract;


    public UnityEvent onInteractionBegin;
    public UnityEvent<float> onInteractionProgress;
    public UnityEvent onInteractionEnd;

    bool isInteracting = true;
    float interactionLength;
    float interactionProgress;



    public void ActivatePrompt(Sprite icon, char iconKey, string prompt, bool preferKeyboard)
    {
        this.icon.sprite = icon;
        iconText.text = string.Empty + iconKey;

        this.prompt.text = prompt;

        bool isText = preferKeyboard;

        if (char.IsWhiteSpace(iconKey) && icon != null)
            isText = false;

        this.icon.enabled = !isText;
        iconText.enabled = isText;
        this.prompt.enabled = false;

        onActivated.Invoke();
    }


    public void UpdateIcon(Camera cam, Vector3 wpos)
    {
        promptRect.anchoredPosition = cam.WorldToScreenPoint(wpos);

        interactionProgress = Mathf.MoveTowards(interactionProgress, isInteracting ? interactionLength : 0, Time.deltaTime * (isInteracting ? 1 : 15));

        onInteractionProgress.Invoke(Mathf.InverseLerp(0, interactionLength, interactionProgress));
    }


    public void SetInteractionState(bool canInteract)
    {
        prompt.enabled = canInteract;

        if (canInteract)
            onEnterInteract.Invoke();
        else
            onLeaveInteract.Invoke();
    }


    public void OnBeganInteract(float length)
    {
        interactionLength = length;
        onInteractionBegin.Invoke();
        isInteracting = true;
    }


    public void OnEndInteract()
    {
        onInteractionEnd.Invoke();
        isInteracting = false;
    }


    public void DestroyPrompt()
    {
        onDeactivated.Invoke();
        Destroy(gameObject, deactivationTime);
    }
}
