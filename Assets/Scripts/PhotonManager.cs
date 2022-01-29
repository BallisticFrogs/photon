using DefaultNamespace;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    public float speed;
    public float energy = 1;
    private PhotonState State = PhotonState.MOVING_PARTICULE;
    public Vector2 Direction;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPhoton();
        }

        transform.Translate(Direction * speed * Time.deltaTime);
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