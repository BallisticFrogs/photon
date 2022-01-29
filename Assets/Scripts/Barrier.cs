using UnityEngine;

namespace DefaultNamespace
{
    public class Barrier : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var photon = other.gameObject.GetComponent<Photon>();
            if (!photon || photon.IsInTransit()) return;

            GameManager.INSTANCE.PhotonLost(photon);
        }
    }
}