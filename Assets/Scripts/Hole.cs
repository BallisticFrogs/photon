using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private new Collider2D collider;

    public Transform exit1;
    public Transform exit2;

    private readonly List<Transit> photonsInTransit = new List<Transit>();

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        for (var i = photonsInTransit.Count - 1; i >= 0; i--)
        {
            var transit = photonsInTransit[i];

            if (!transit.photon)
            {
                ProcessPhotonExit(transit);
            }

            // check for exit distance
            var d = (transit.photon.transform.position - transit.exit.position).magnitude;

            // if at the exit point, let go
            if (d < 0.01)
            {
                photonsInTransit.RemoveAt(i);
                ProcessPhotonExit(transit);
            }
        }
    }

    private void ProcessPhotonExit(Transit transit)
    {
        transit.photon.SetInTransit(false);

        var speed = transit.photon.Velocity.magnitude;
        if (transit.photon.State == PhotonState.MOVING_PARTICULE)
        {
            var outCount = transit.exit.childCount;
            var index = Random.Range(0, outCount);
            transit.photon.Velocity = ComputeOutVelocity(transit, index, speed);
        }
        else
        {
            // incoming photon takes first out target
            transit.photon.Velocity = ComputeOutVelocity(transit, 0, speed);

            // clone photon for each out target
            for (int i = 1; i < transit.exit.childCount; i++)
            {
                var newPhoton = GameManager.INSTANCE.CreateNewPhoton(transit.photon);
                newPhoton.SwitchPhotonState();
                newPhoton.AddColliderToIgnore(collider);
                newPhoton.Velocity = ComputeOutVelocity(transit, i, speed);
            }
        }
    }

    private static Vector3 ComputeOutVelocity(Transit transit, int index, float speed)
    {
        var outTarget = transit.exit.transform.GetChild(index).transform.position;
        return (outTarget - transit.exit.position).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var photon = other.gameObject.GetComponentInParent<Photon>();
        if (!photon || photon.ShouldIgnoreCollision(collider)) return;

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