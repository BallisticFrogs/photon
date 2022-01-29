using DefaultNamespace;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    public float energy = 1;
    public Vector2 Velocity;
    private PhotonState State = PhotonState.MOVING_PARTICULE;

    [HideInInspector] public GameObject source;
    private float timeFromSource;
    private bool dead;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPhoton();
        }

        transform.Translate(Velocity * Time.deltaTime);

        timeFromSource += Time.deltaTime;
        if (!dead && timeFromSource >= GameManager.INSTANCE.missDetectionTime)
        {
            var hit = Physics2D.CircleCast(transform.position, GetRadius(), Velocity,
                float.PositiveInfinity, Masks.ATOMS);
            if (!hit || !hit.collider)
            {
                dead = true;

                // create a new photon near the latest checkpoint to continue playing
                var pos = FindCorrectPopPosition(source);
                var photonObj = Instantiate(GameManager.INSTANCE.photonPrefab, pos, Quaternion.identity);
                var photon = photonObj.GetComponent<PhotonManager>();
                photon.energy = energy;
                photon.Velocity = (source.transform.position - pos).normalized * GameManager.INSTANCE.emissionSpeed;

                Velocity *= 10;
                Destroy(gameObject, 5f);
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

    void SwitchPhoton()
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