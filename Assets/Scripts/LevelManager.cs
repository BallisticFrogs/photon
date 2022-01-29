using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager INSTANCE;

    public GameObject victoryVFX;
    public AudioClip victorySFX;

    public Atom start;
    public Atom end;

    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private bool done;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        if (checkpoints == null) checkpoints = new List<Checkpoint>();
        checkpoints.Insert(0, new Checkpoint(true, start));
        checkpoints.Add(new Checkpoint(false, end));
        RestartAtLastCheckpoint();
    }

    void Update()
    {
        RecordCheckpointProgress();

        if (!done && checkpoints[^1].reached)
        {
            done = true;

            // victory
            Instantiate(victoryVFX, end.transform.position, Quaternion.identity);
            // TODO play SFX
        }
    }

    private void RecordCheckpointProgress()
    {
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.reached) continue;
            var reached = checkpoint.CheckCurrentlyReached();
            if (!reached) break;
        }
    }

    public void RestartAtLastCheckpoint()
    {
        var checkpoint = FindLatestCheckpoint();

        foreach (var atom in checkpoint)
        {
            atom.GenerateBonusPhoton(1);
        }
    }

    private List<Atom> FindLatestCheckpoint()
    {
        for (var i = checkpoints.Count - 1; i >= 0; i--)
        {
            var checkpoint = checkpoints[i];
            if (!checkpoint.reached) continue;
            return checkpoint.atoms;
        }

        throw new Exception("should not have been possible...");
    }
}