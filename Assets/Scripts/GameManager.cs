using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    public GameObject photonPrefab;

    public float emissionAngleSpeed = -1f;
    public float emissionSpeed = 2f;
    public float missDetectionTime = 3f;
    public float checkpointPhotonPopDistance = 15f;

    private void Awake()
    {
        INSTANCE = this;
    }
}