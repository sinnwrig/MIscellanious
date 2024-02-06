using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionController : MonoBehaviour
{
    private static readonly HashSet<Interactable> interactables = new HashSet<Interactable>();

    public static void Register(Interactable item) => interactables.Add(item);
    public static void Deregister(Interactable item) => interactables.Remove(item);


    public Camera controlCamera;
    public float minPromptAngle = 20.0f;
    public Transform promptParent;
    public bool preferKeyboard = true;


    [HideInInspector]
    public Interactable closest;

    [HideInInspector]
    public Interactable focus;


    private InteractionPrompt activePrompt;



    private IEnumerator Start() 
    {
        SetFocus(null);

        yield return UpdateInteractables();
    }


    private IEnumerator UpdateInteractables()
    {
        var wait = new WaitForSeconds(0.1f);

        while (true)
        {
            var lastClosest = closest;
            Interactable searchFocus = SearchFocusInteractable(out closest);

            if (closest != lastClosest)
                ClosestChanged();

            if (focus != searchFocus)
                SetFocus(searchFocus);

            yield return wait;            
        }
    }


    private Interactable SearchFocusInteractable(out Interactable closest)
    {
        Vector3 cameraPos = controlCamera.transform.position;
        Vector3 lookDir = controlCamera.transform.forward;

        float closestAngle = 90.0f;
        
        Interactable focus = null;
        closest = null;

        foreach (Interactable item in interactables)
        {
            item.DistanceAndAngle(cameraPos, lookDir, out float distToItem, out float angleToItem);

            bool withinDistance = distToItem < item.appearanceRange && angleToItem < 90.0f;
            bool canInteract = distToItem < item.interactionRange;

            bool closestFocus = angleToItem < closestAngle;
            
            if (!withinDistance)
                continue;

            if (closest == null)
                closest = item;
            else if (closestFocus)
                closest = item;

            if (!closestFocus)
                continue;

            closestAngle = angleToItem;

            if (!canInteract || angleToItem > minPromptAngle)
                continue;

            focus = item;
        }

        return focus;
    }


    private void SetFocus(Interactable focus)
    {
        if (this.focus != null)
            this.focus.OnLostFocus();

        this.focus = focus;

        if (this.focus != null)
            this.focus.OnGainedFocus();

        FocusChanged();
    }


    private void Update()
    {
        if (activePrompt == null)
            return;

        activePrompt.UpdatePrompt();
    }


    private void ClosestChanged()
    {        
        ChangeActivePrompt();
        FocusChanged();
    }


    private void FocusChanged()
    {
        if (activePrompt == null)
            return;
        
        activePrompt.OnFocusChanged(closest == focus);
    }


    private void ChangeActivePrompt()
    {
        StartCoroutine(DestroyTemp(activePrompt));

        // No item- don't do anything
        if (closest == null)
        {
            activePrompt = null;
            return;
        }

        InteractionPrompt sourcePrompt = closest.GetPrompt();

        if (sourcePrompt == null)
            return;

        activePrompt = Instantiate(sourcePrompt, promptParent);

        activePrompt.Activate(closest, preferKeyboard);
        activePrompt.OnFocusChanged(closest == focus);
    }


    private IEnumerator DestroyTemp(InteractionPrompt toDestroy)
    {
        if (toDestroy == null)
            yield break;

        float destroyTime = toDestroy.OnWillDestroy();

        while (destroyTime > 0)
        {
            destroyTime -= Time.deltaTime;
            toDestroy.UpdatePrompt();
            yield return null;
        }

        Destroy(toDestroy.gameObject);
    }
}
