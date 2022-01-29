using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class Checkpoint
    {
        [HideInInspector] public bool reached;
        public List<Atom> atoms;

        public Checkpoint(bool reached, params Atom[] a)
        {
            this.reached = reached;
            atoms = new List<Atom>(a);
        }

        public bool CheckCurrentlyReached()
        {
            foreach (var atom in atoms)
            {
                if (atom.energy == 0) return false;
            }

            Debug.Log("checkpoint reached");

            reached = true;
            return reached;
        }
    }
}