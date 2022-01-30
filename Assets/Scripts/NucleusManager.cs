using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NucleusManager : MonoBehaviour
{
    public GameObject NucleusSphere;
    private Vector3 randomAxis1;
    private Vector3 randomAxis2;
    private float speed1;
    private float speed2;

    // Start is called before the first frame update
    void Start()
    {
        var nucleusCoordinates = new List<Vector3>()
        {
            new Vector3(0.211324919700475f, -0.366025373071777f, -0.472536763890327f),
            new Vector3(-0.422649680133210f, 0.000000024445871f, -0.472536809709025f),
            new Vector3(0.211324912464434f, 0.366025434497091f, -0.472536719546539f),
            new Vector3(-0.316987292680581f, -0.549038108810054f, -0.000000067253062f),
            new Vector3(0.633974596215562f, 0.000000006266591f, 0.000000067990669f),
            new Vector3(-0.316987303534895f, 0.549038102543317f, -0.000000000737605f),
            new Vector3(0.211324818345971f, -0.366025430319168f, 0.472536764873803f),
            new Vector3(-0.422649781487713f, -0.000000032801520f, 0.472536719054802f),
            new Vector3(0.211324811109930f, 0.366025377249699f, 0.472536809217290f)
        };
        foreach (var coordinate in nucleusCoordinates)
        {
            var s1 = Instantiate(NucleusSphere, this.transform);
            s1.transform.localPosition = coordinate;
        }
        
        randomAxis1 = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        randomAxis2 = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        speed1 = Random.Range(10, 200);
        speed2 = Random.Range(10, 200);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(randomAxis1, speed1*Time.deltaTime );
        this.transform.Rotate(randomAxis2, speed2*Time.deltaTime );
    }
}
