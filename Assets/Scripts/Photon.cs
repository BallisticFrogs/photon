using DefaultNamespace;
using UnityEngine;

public class Photon : MonoBehaviour
{
    public float energy = 1;
    public Vector2 Velocity;
    private PhotonState State = PhotonState.MOVING_PARTICULE;

    [HideInInspector] public Atom source;
    private float timeFromSource;
    private bool dead;

    private void Start()
    {
        // GameManager.INSTANCE.RegisterPhoton(this);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SwitchPhotonState();
        }

        transform.Translate(Velocity * Time.deltaTime);

        timeFromSource += Time.deltaTime;
        if (!dead && timeFromSource >= GameManager.INSTANCE.missDetectionTime)
        {
            var hit = Physics2D.CircleCast(transform.position, GetRadius(), Velocity,
                float.PositiveInfinity, Masks.ATOMS);
            if (!hit || !hit.collider)
            {
                // create a new photon near the latest checkpoint to continue playing
                source.GenerateBonusPhoton(energy);

                dead = true;
                Velocity *= 10;
                GameManager.INSTANCE.PhotonLost(this, 5000);
            }
        }
    }

    private float GetRadius()
    {
        return transform.localScale.x * 0.5f;
    }

    void SwitchPhotonState()
    {
        if (PhotonState.MOVING_PARTICULE == State)
        {
            State = PhotonState.MOVING_WAVE;
        }
        else if (PhotonState.MOVING_WAVE == State)
        {
            State = PhotonState.MOVING_PARTICULE;
        }
    }
}