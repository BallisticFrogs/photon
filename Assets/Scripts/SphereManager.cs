using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SphereManager : MonoBehaviour
{

    public GameObject Center;
    public Vector3 rotationVector;
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Center.transform.position, rotationVector, rotationSpeed * Time.deltaTime );
    }
}
