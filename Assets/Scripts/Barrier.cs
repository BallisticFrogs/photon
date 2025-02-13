﻿using UnityEngine;

namespace DefaultNamespace
{
    public class Barrier : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var photon = other.gameObject.GetComponentInParent<Photon>();
            if (!photon || photon.IsInTransit()) return;

            Debug.Log("killing " + photon);

            SoundManager.INSTANCE.sfxPhotonLost.PlaySFX();
            GameManager.INSTANCE.PhotonLost(photon);
        }
    }
}