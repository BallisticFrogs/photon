using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Photon : MonoBehaviour
{
    public GameObject particle;
    public GameObject wave;

    public float energy = 1;
    public Vector2 Velocity;
    [HideInInspector] public PhotonState State = PhotonState.MOVING_PARTICULE;
    [HideInInspector] public Atom source;

    private readonly List<Collider2D> collidersToIgnore = new List<Collider2D>();
    private bool inTransit;
    private float timeFromSource;
    private bool dead;

    private void Start()
    {
        SyncVisual();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SwitchPhotonState();
        }

        // move
        transform.Translate(Velocity * Time.deltaTime);
        
        // align to movement
        float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
        wave.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        particle.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // check if it will never hit anything
        timeFromSource += Time.deltaTime;
        if (!dead && timeFromSource >= GameManager.INSTANCE.missDetectionTime)
        {
            var hit = Physics2D.CircleCast(transform.position, 1, Velocity,
                float.PositiveInfinity, Masks.ATOMS | Masks.OBSTACLES);
            if (!hit || !hit.collider)
            {
                dead = true;
                Velocity *= 10;
                GameManager.INSTANCE.PhotonLost(this, 5000);
            }
        }
    }

    public void SetInTransit(bool transit)
    {
        this.inTransit = transit;
        timeFromSource = 0;
    }

    public bool IsInTransit()
    {
        return inTransit;
    }

    private float GetRadius()
    {
        return transform.localScale.x * 0.5f;
    }

    public void SwitchPhotonState()
    {
        if (PhotonState.MOVING_PARTICULE == State)
        {
            State = PhotonState.MOVING_WAVE;
        }
        else if (PhotonState.MOVING_WAVE == State)
        {
            State = PhotonState.MOVING_PARTICULE;
        }

        SyncVisual();
    }

    private void SyncVisual()
    {
        if (PhotonState.MOVING_PARTICULE == State)
        {
            particle.SetActive(true);
            wave.SetActive(false);
        }
        else
        {
            particle.SetActive(false);
            wave.SetActive(true);
        }
    }

    public void AddColliderToIgnore(Collider2D col)
    {
        collidersToIgnore.Add(col);
    }

    public bool ShouldIgnoreCollision(Collider2D col)
    {
        foreach (var other in collidersToIgnore)
        {
            if (other == col) return true;
        }

        return false;
    }
}