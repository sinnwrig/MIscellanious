using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchAnimator : MonoBehaviour
{
    public Transform switchTransform;
    public float switchSpeed;
    public Vector3 switchAxis;

    public void Rotate(float newAngle)
    {
        StopAllCoroutines();        
        StartCoroutine(AnimateRotate(newAngle));
    }

    private IEnumerator AnimateRotate(float newAngle)
    {
        Quaternion rot = switchTransform.rotation;
        Quaternion switchRot = Quaternion.AngleAxis(newAngle, switchAxis);
        

        while (rot != switchRot)
        {
            rot = Quaternion.RotateTowards(rot, switchRot, Time.deltaTime * switchSpeed);
            switchTransform.rotation = rot;

            yield return null;
        }
    }
}


public static class SwitchExt
{
    public static void Rotate2(this SwitchAnimator switchA, float angle) => switchA.Rotate(angle);
}
