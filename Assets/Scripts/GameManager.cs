using System.Threading.Tasks;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    public GameObject photonPrefab;

    public CinemachineVirtualCamera cinemachineCam;
    public CinemachineTargetGroup cinemachineTargetGroup;

    public float emissionAngleSpeed = -1f;
    public float emissionSpeed = 2f;
    public float missDetectionTime = 3f;
    public float checkpointPhotonPopDistance = 15f;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        var targets = cinemachineTargetGroup.m_Targets;
        if (targets.Length > 0)
        {
            targets[0].weight = 10;
            cinemachineCam.Follow = targets[0].target;
        }
    }

    public async void PhotonLost(Photon photon, float delay = 0)
    {
        cinemachineTargetGroup.RemoveMember(photon.transform);
        if (cinemachineCam.Follow == photon.transform)
        {
            cinemachineCam.Follow = null;
        }

        if (delay > 0)
        {
            await Task.Delay(5000);
        }

        if (photon && photon.gameObject)
        {
            Destroy(photon.gameObject);
        }
    }

    public Photon CreateNewPhoton(Atom atom)
    {
        var pos = atom.charge.transform.position;
        var dir = (pos - atom.transform.position).normalized;
        var velocity = dir * INSTANCE.emissionSpeed;
        return CreateNewPhoton(atom, pos, velocity, atom.energy);
    }

    public Photon CreateNewPhoton(Atom source, Vector3 pos, Vector3 velocity, float energy,
        bool register = true)
    {
        var photonObj = Instantiate(INSTANCE.photonPrefab, pos, Quaternion.identity);
        var photon = photonObj.GetComponent<Photon>();
        photon.Velocity = velocity;
        photon.source = source;
        photon.energy = energy;

        if (register)
        {
            RegisterPhoton(photon);
        }

        return photon;
    }

    public async void RegisterPhoton(Photon photon, int registerDelay = 0)
    {
        if (registerDelay > 0)
        {
            Debug.Log("waiting");
            await Task.Delay(registerDelay);
            Debug.Log("done waiting");
        }

        if (photon)
        {
            var i = cinemachineTargetGroup.FindMember(photon.gameObject.transform);
            if (i < 0)
            {
                cinemachineTargetGroup.AddMember(photon.gameObject.transform, 1, 0.5f);
            }
        }
    }

    public void RegisterAtom(Atom atom)
    {
        var i = cinemachineTargetGroup.FindMember(atom.transform);
        if (i < 0)
        {
            cinemachineTargetGroup.AddMember(atom.transform, 1, 0.5f);
        }
    }

    public void UnregisterAtom(Atom atom)
    {
        cinemachineTargetGroup.RemoveMember(atom.transform);
    }
}