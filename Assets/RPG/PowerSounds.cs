using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerSounds", menuName = "RPG/PowerSounds", order = 1)]
    public class PowerSounds : ScriptableObject
    {
        public AudioClip[] startFX;
        public AudioClip[] launchFX;
        public AudioClip[] hitFX;

        public void PlayStart(AudioSource src)
        {
            if (startFX.Length > 0)
                src.PlayOneShot(startFX[Random.Range(0, startFX.Length)]);
        }

        public void PlayLaunch(AudioSource src)
        {
            if (launchFX.Length > 0)
                src.PlayOneShot(launchFX[Random.Range(0, launchFX.Length)]);
        }

        public void PlayHit(AudioSource src)
        {
            if (hitFX.Length > 0)
                src.PlayOneShot(hitFX[Random.Range(0, hitFX.Length)]);
        }
    }
}
