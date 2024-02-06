using UnityEngine;


public class BobAnimator : MonoBehaviour
{
    public Transform bobMesh;
    public float bobSpeed = 1f;
    public float bobStrength = 0.15f;  


    private void Update()
    {
        float pow = Mathf.PingPong(Time.time * bobSpeed, 2) - 1;

        Vector3 p = bobMesh.localPosition;
        p.y += pow * bobStrength * Time.deltaTime;
        bobMesh.localPosition = p;
    }
}
