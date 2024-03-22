using UnityEngine;


[AddComponentMenu(""), DisallowMultipleComponent]
public class FloatingTransform : MonoBehaviour
{
    public Vector3 customPos;

    [Tooltip("Should child rigidbodies be disabled past a certain threshold?")]
    public bool disableDistantPhysicsObjects;

    [Tooltip("The distance at which any attached or parented rigidbodies are disabled. Helps with distant simulations that accumulate imprecision in a collision/position simulation.")]
    public double physicsDisableDistance = 100000.0; 
}
