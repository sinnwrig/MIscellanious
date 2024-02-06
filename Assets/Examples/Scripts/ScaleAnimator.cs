using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimator : MonoBehaviour
{
    public float scaleSpeed = 1.0f;
    public RectTransform toScale;


    public void ScaleTo(float scale)
    {   
        if (scale > 0)
            toScale.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(AnimateScale(scale));
    }   


    private IEnumerator AnimateScale(float scaleTarget)
    {
        while (Mathf.Abs(toScale.localScale.x - scaleTarget) > 0.05f)
        {
            float scale = toScale.localScale.x;
            scale = Mathf.MoveTowards(scale, scaleTarget, Time.deltaTime * scaleSpeed);

            toScale.localScale = Vector3.one * scale;

            yield return null;
        }

        if (scaleTarget == 0)
            Invoke(nameof(Disable), 0.0f);
    }


    private void Disable()
    {
        toScale.gameObject.SetActive(false);
    }
}
