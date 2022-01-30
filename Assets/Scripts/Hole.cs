using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private Collider2D holeCollider;

    public Transform exit1;
    public Transform exit2;

    private List<Transit> photonsInTransit = new List<Transit>();

    private void Awake()
    {
        holeCollider = GetComponentInChildren<Collider2D>();
    }

    void Update()
    {
        for (var i = photonsInTransit.Count - 1; i >= 0; i--)
        {
            var transit = photonsInTransit[i];
            if (!transit.photon) photonsInTransit.RemoveAt(i);
        }
    }

    private void ProcessPhotonExit(Transit transit)
    {
        transit.photon.SetInTransit(false);

        Debug.Log("photon exiting hole: " + transit.photon.State);

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
                newPhoton.AddColliderToIgnore(holeCollider);
                newPhoton.Velocity = ComputeOutVelocity(transit, i, speed);
            }
        }
    }

    private static Vector3 ComputeOutVelocity(Transit transit, int index, float speed)
    {
        var outTarget = transit.exit.transform.GetChild(index).transform.position;
        return (outTarget - transit.exit.position).normalized * speed;
    }

    private async void OnTriggerEnter2D(Collider2D other)
    {
        var photon = other.gameObject.GetComponentInParent<Photon>();
        if (!photon || photon.ShouldIgnoreCollision(holeCollider)) return;
        if (FindExistingTransit(photon) != null) return;

        var photonPos = photon.transform.position;
        var d1 = (photonPos - exit1.position).magnitude;
        var d2 = (photonPos - exit2.position).magnitude;
        var chosenExit = d1 > d2 ? exit1 : exit2;
        photonsInTransit.Add(new Transit() { photon = photon, exit = chosenExit });
        photon.SetInTransit(true);
        Debug.Log(photon + " is now in transit");

        // wait until the photon is near the axis of the hole
        // without this, when aligning it, it may end the collision...
        float d = PhotonDist(photon);
        while (d > 0.1f)
        {
            var dd = PhotonDist(photon);
            if (dd > d) break;
            d = dd;
            await Awaiters.FixedUpdate;
        }

        RedirectPhotonThroughHole(photon, chosenExit);
    }

    private static void RedirectPhotonThroughHole(Photon photon, Transform chosenExit)
    {
        Vector3 photonPos = photon.transform.position;
        var speed = photon.Velocity.magnitude;
        var dir = (chosenExit.transform.position - photonPos).normalized;
        photon.Velocity = dir * speed;
    }

    private float PhotonDist(Photon photon)
    {
        return HandleUtility.DistancePointLine(photon.transform.position, exit1.position, exit2.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var transit = FindExistingTransit(other);
        if (transit != null)
        {
            ProcessPhotonExit(transit);
            photonsInTransit.Remove(transit);
        }
    }

    private float DistToSegment(Vector3 s1, Vector3 s2, Vector3 p)
    {
        var len = (s1 - s2).sqrMagnitude;
        if (len <= 0.0001) return Dist(p, s1);
        var t = Mathf.Max(0, Mathf.Min(1, Vector3.Dot(p - s1, s2 - s1) / len));
        var projection = s1 + t * (s2 - s1);
        return Dist(p, projection);
    }

    private float Dist(Vector3 p1, Vector3 p2)
    {
        return (p1 - p2).magnitude;
    }

    private Transit FindExistingTransit(Collider2D other)
    {
        var photon = other.attachedRigidbody.gameObject.GetComponent<Photon>();
        if (!photon) return null;
        return FindExistingTransit(photon);
    }

    private Transit FindExistingTransit(Photon photon)
    {
        for (var i = photonsInTransit.Count - 1; i >= 0; i--)
        {
            var transit = photonsInTransit[i];
            if (transit.photon == photon)
            {
                return transit;
            }
        }

        return null;
    }
}

class Transit
{
    public Photon photon;
    public Transform exit;
}