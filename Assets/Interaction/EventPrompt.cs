using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class EventPrompt : InteractionPrompt
{   
    public RectTransform promptRect;
    public Image icon;
    public Text iconText;
    public Text prompt;


    public UnityEvent onActivated;

    public float deactivationTime = 0.5f;
    public UnityEvent onDeactivated;


    public UnityEvent onGainedFocus;
    public UnityEvent onLostFocus;



    protected override void OnActivate()
    {
        EventInteraction item = this.item as EventInteraction;

        icon.sprite = item.icon;
        iconText.text = string.Empty + item.iconKey;

        prompt.text = item.promptText;

        bool isText = preferKeyboard;

        if (char.IsWhiteSpace(item.iconKey) && item.icon != null)
            isText = false;

        icon.enabled = !isText;
        iconText.enabled = isText;

        onActivated.Invoke();
    }


    public override void UpdatePrompt()
    {
        promptRect.anchoredPosition = Camera.main.WorldToScreenPoint(item.transform.position);
    }


    public override void OnFocusChanged(bool focusState)
    {
        prompt.enabled = focusState;

        if (focusState)
            onGainedFocus.Invoke();
        else
            onLostFocus.Invoke();
    }


    public override float OnWillDestroy()
    {
        onDeactivated.Invoke();

        return deactivationTime;
    }
}
