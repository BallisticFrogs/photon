using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager INSTANCE;

    public GameObject victoryVFX;
    public AudioClip victorySFX;

    public Atom start;
    public Atom end;

    private bool done;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        start.GenerateBonusPhoton(1);
    }

    void Update()
    {
        if (!done && end.energy > 0)
        {
            done = true;

            // victory
            Instantiate(victoryVFX, end.transform.position, Quaternion.identity);
            // TODO play SFX
        }
    }
}