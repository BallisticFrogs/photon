using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    public float speed;
    private PhotonState State = PhotonState.LAUNCHING;
    public GameObject Direction ;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (State)
            {
                case PhotonState.LAUNCHING :
                    LaunchPhoton();
                    break;
                case PhotonState.MOVING_PARTICULE | PhotonState.MOVING_WAVE:
                    SwitchPhoton();
                    break;
                // No defa  ult
            }
        }
        if (PhotonState.MOVING_PARTICULE == State)
            this.transform.Translate(Direction.transform.position * speed * Time.deltaTime);
    }

    void LaunchPhoton()
    {
        State = PhotonState.MOVING_PARTICULE;
        Direction.SetActive(false);
    }

    void SwitchPhoton()
    {
        if (PhotonState.MOVING_PARTICULE == State)
            State = PhotonState.MOVING_WAVE;
        if (PhotonState.MOVING_WAVE == State)
            State = PhotonState.MOVING_PARTICULE;
    }
}
