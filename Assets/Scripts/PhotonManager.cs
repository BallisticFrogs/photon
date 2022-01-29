using DefaultNamespace;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    public float energy = 1;
    public Vector2 Velocity;
    private PhotonState State = PhotonState.MOVING_PARTICULE;

    [HideInInspector] public GameObject source;
    private float timeFromSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPhoton();
        }

        transform.Translate(Velocity * Time.deltaTime);

        timeFromSource += Time.deltaTime;
        if (timeFromSource > GameManager.INSTANCE.missDetectionTime)
        {
            var hit = Physics2D.Raycast(transform.position, Velocity);
            if (!hit)
            {
                // 
                Destroy(gameObject);
            }
        }
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