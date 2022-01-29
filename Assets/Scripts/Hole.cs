using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Transform exit1;
    public Transform exit2;

    private readonly List<Transit> photonsInTransit = new List<Transit>();

    void Update()
    {
        for (var i = photonsInTransit.Count - 1; i >= 0; i--)
        {
            var transit = photonsInTransit[i];

            // check for exit distance
            var d = (transit.photon.transform.position - transit.exit.position).magnitude;

            // if at the exit point, let go
            if (d < 0.01)
            {
                photonsInTransit.RemoveAt(i);
                transit.photon.SetInTransit(false);
                transit.photon.Velocity = transit.photon.Velocity.Rotate(Random.Range(-60, 60));
                // TODO redirect or split
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var photon = other.gameObject.GetComponent<Photon>();
        if (!photon) return;

        var photonPos = photon.transform.position;
        var d1 = (photonPos - exit1.position).magnitude;
        var d2 = (photonPos - exit2.position).magnitude;
        var chosenExit = d1 > d2 ? exit1 : exit2;
        photonsInTransit.Add(new Transit() { photon = photon, exit = chosenExit });

        var speed = photon.Velocity.magnitude;
        var dir = (chosenExit.transform.position - photonPos).normalized;
        photon.Velocity = dir * speed;
        photon.SetInTransit(true);
    }
}

class Transit
{
    public Photon photon;
    public Transform exit;
}