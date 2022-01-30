using UnityEngine;

namespace DefaultNamespace
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager INSTANCE;

        public AudioClip sfxEmitted;
        public AudioClip sfxCaptured;
        public AudioClip sfxVictory;
        public AudioClip sfxDeath;
        public AudioClip sfxSwitch;
        public AudioClip sfxPhotonLost;

        private AudioSource audioSource;

        private void Awake()
        {
            INSTANCE = this;
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySFX(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    static class SfxHelper
    {
        public static void PlaySFX(this AudioClip clip)
        {
            SoundManager.INSTANCE.PlaySFX(clip);
        }
    }
}