using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 0.1f;

    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed);
    }
}