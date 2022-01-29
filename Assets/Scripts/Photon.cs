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
                var pos = FindCorrectPopPosition(source.gameObject);
                var velocity = (source.transform.position - pos).normalized * GameManager.INSTANCE.emissionSpeed * 2;
                GameManager.INSTANCE.CreateNewPhoton(null, pos, velocity, energy, false);
                GameManager.INSTANCE.RegisterAtom(source);

                dead = true;
                Velocity *= 10;
                GameManager.INSTANCE.PhotonLost(this, 5000);
            }
        }
    }

    private Vector3 FindCorrectPopPosition(GameObject source)
    {
        var sourcePos = source.transform.position;

        int attempts = 0;
        while (attempts < 10)
        {
            attempts++;
            var v = GenerateRandomPosition();
            var pos = sourcePos + v;
            var hitCheck = Physics2D.CircleCast(pos, GetRadius(), -v, float.PositiveInfinity, Masks.ATOMS);
            if (hitCheck.collider && hitCheck.rigidbody.gameObject == source)
            {
                return pos;
            }
            else
            {
                Debug.Log("hit: " + hitCheck.rigidbody.gameObject);
            }
        }

        return sourcePos + GenerateRandomPosition();
    }

    private static Vector3 GenerateRandomPosition()
    {
        return Vector2.up.Rotate(Random.Range(0, 360)) * GameManager.INSTANCE.checkpointPhotonPopDistance;
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