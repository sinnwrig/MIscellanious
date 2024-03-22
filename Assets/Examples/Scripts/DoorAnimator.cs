using UnityEngine;


public class DoorAnimator : MonoBehaviour
{
    public Transform left;
    public Transform right;

    public float openTime = 1.0f;
    public bool isOpen;

    float openState;


    public void ToggleOpen()
    {
        isOpen = !isOpen;
    }

    public void Open() => isOpen = true;

    public void Close() => isOpen = false;


    void Update()
    {
        openState = Mathf.MoveTowards(openState, isOpen ? 0.01f : openTime, Time.deltaTime);
        
        Vector3 scale = left.localScale;
        scale.x = openState;
        left.localScale = scale;

        scale = right.localScale;
        scale.x = openState;
        right.localScale = scale;
    }
}
