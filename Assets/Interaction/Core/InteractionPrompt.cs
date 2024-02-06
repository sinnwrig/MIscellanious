using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public abstract class InteractionPrompt : MonoBehaviour
{
    public Interactable item { get; private set; }
    public bool preferKeyboard { get; private set; }


    public void Activate(Interactable item, bool preferKeyboard) 
    {
        this.item = item;
        this.preferKeyboard = preferKeyboard;

        OnActivate();
    }


    protected virtual void OnActivate() { }

    public virtual float OnWillDestroy() => 0.0f;


    public virtual void UpdatePrompt() { } 

    public virtual void OnFocusChanged(bool focusState) { }
}
