using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TurretComponents
{
    [RequireComponent(typeof(TurretController))]
    [RequireComponent(typeof(AudioSource))]
    public class GunSoundSource : MonoBehaviour
    {
        AudioSource audioSource;
        TurretController controller;

        public AudioClip AudioClip;
        public float PitchMultiplier = 1;
        public float VolumeMultiplier = 1;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            controller = GetComponent<TurretController>();
        }

        private void Start()
        {
            controller.OnFire += HandleOnFireEvent;
        }

        private void OnDestroy()
        {
            controller.OnFire -= HandleOnFireEvent;
        }

        private void HandleOnFireEvent(object sender, TurretController.OnFireEventArgs e)
        {
            float pitch = Random.Range(0.75f * PitchMultiplier, 1.25f * PitchMultiplier);
            float volume = Random.Range(0.75f * VolumeMultiplier, 1.25f * VolumeMultiplier);

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(AudioClip, volume);
        }
    }
}