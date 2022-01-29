using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 1f;

    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}