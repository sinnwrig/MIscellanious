using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PushAnimator : MonoBehaviour
{
    public Transform pushTransform;
    public float pushSpeed;
    public Vector3 pushDir;


    private Vector3 origPos;

    void Awake() => origPos = pushTransform.position;

    public void Push(float pushDistance)
    {
        pushTransform.position = origPos;

        StopAllCoroutines();        
        StartCoroutine(AnimatePush(pushDistance));
    }

    private IEnumerator AnimatePush(float pushDistance)
    {
        origPos = pushTransform.position;
        Vector3 pushPos = pushTransform.position + (pushTransform.TransformVector(pushDir).normalized * pushDistance);
        Vector3 pos = origPos;

        while (pos != pushPos)
        {
            pos = Vector3.MoveTowards(pos, pushPos, Time.deltaTime * pushSpeed);
            pushTransform.position = pos;

            yield return null;
        }

        while (pos != origPos)
        {
            pos = Vector3.MoveTowards(pos, origPos, Time.deltaTime * pushSpeed);
            pushTransform.position = pos;

            yield return null;
        }
    }
}
