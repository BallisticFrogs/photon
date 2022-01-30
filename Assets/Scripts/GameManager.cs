using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const string TAG_LEVEL_MANAGER = "LevelManager";

    public static GameManager INSTANCE;

    public GameObject photonPrefab;

    public CinemachineVirtualCamera cinemachineCam;
    public CinemachineTargetGroup cinemachineTargetGroup;

    public ParticleSystem photonParticleVFX;

    public float levelSpacing = 50f;
    public float emissionAngleSpeed = -1f;
    public float emissionSpeed = 2f;
    public float missDetectionTime = 3f;
    public float checkpointPhotonPopDistance = 15f;

    public int menuSceneIndex = 1;
    public List<int> levelSceneIndices = new List<int>();

    private void Awake()
    {
        INSTANCE = this;
    }

    private async void Start()
    {
        if (SceneManager.sceneCount == 1)
        {
            // load menu scene
            SceneManager.LoadScene(menuSceneIndex, LoadSceneMode.Additive);
        }
    }

    public async void NextLevel(Atom currentPosition)
    {
        Debug.Log("loading next level");

        // compute next scene to load
        var currentScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        var sceneIndex = currentScene.buildIndex;
        var levelIndex = levelSceneIndices.IndexOf(sceneIndex);
        var nextLevel = levelIndex < 0 ? 0 : levelIndex + 1;
        if (nextLevel >= levelSceneIndices.Count)
        {
            Debug.Log("no next level found");
            return;
        }

        var nextSceneIndex = levelSceneIndices[nextLevel];

        // make sure the old level manager is not active
        LevelManager.INSTANCE.enabled = false;

        // load scene
        await SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);

        Debug.Log("next level loaded, preparing it");

        // translate everything to the right of the current position
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(scene);
        var offset = new Vector3(currentPosition.transform.position.x + levelSpacing, 0, 0);
        var sceneObjects = scene.GetRootGameObjects();
        foreach (var obj in sceneObjects)
        {
            obj.transform.position += offset;
        }

        Debug.Log("sending player to next level start atom");

        // send player to the new level
        currentPosition.GenerateBonusPhotonTowards(LevelManager.INSTANCE.start);

        // unload previous scene
        await Task.Delay(3000);
        await SceneManager.UnloadSceneAsync(currentScene);
        Debug.Log("old level has been unloaded");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        if (LevelManager.INSTANCE == null) return;

        for (var i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            var t = cinemachineTargetGroup.m_Targets[i];
            if (!t.target)
            {
                cinemachineTargetGroup.RemoveMember(t.target);
            }
        }

        if (cinemachineTargetGroup.m_Targets.Length == 0)
        {
            SoundManager.INSTANCE.sfxDeath.PlaySFX();
            LevelManager.INSTANCE.RestartAtLastCheckpoint();
        }

        var targets = cinemachineTargetGroup.m_Targets;
        if (targets.Length > 0)
        {
            targets[0].weight = 10;
            cinemachineCam.Follow = targets[0].target;
        }
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void emitParticles(Vector3 position)
    {
        photonParticleVFX.transform.position = position;
        photonParticleVFX.Emit(1);
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

    public Photon CreateNewPhoton(Photon photon)
    {
        return CreateNewPhoton(photon.source, photon.transform.position, photon.Velocity, photon.energy);
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

    public void CleanupForVictory(Atom end)
    {
        var atoms = GameObject.FindGameObjectsWithTag("Atom");
        foreach (var atom in atoms)
        {
            var a = atom.GetComponent<Atom>();
            a.energy = 0;
        }

        var photons = GameObject.FindGameObjectsWithTag("Photon");
        foreach (var photon in photons)
        {
            var p = photon.GetComponent<Photon>();
            if (p)
            {
                p.energy = 0;
            }

            Destroy(photon);
        }

        RegisterAtom(end);
    }
}