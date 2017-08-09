using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "EffectParticles", menuName = "RPG/EffectParticles", order = 3)]
    public class EffectParticles : VisualEffect
    {
        public ParticleSystem particles; // reference to the prefab set up in editor

        public override GameObject Begin(Transform t, RPGSettings.ColorCode color, bool autoStop = true)
        {
            GameObject go = Instantiate(particles.gameObject);
            go.transform.parent = t;
            go.transform.localPosition = Vector3.zero;
            go.gameObject.name = particles.gameObject.name;
            go.GetComponent<ParticleSystem>().startColor = RPGSettings.GetColor(color);
            if (autoStop)
                Destroy(go, lifespan);
            return go;
        }

        public override void End(GameObject go)
        {
            Destroy(go);
        }
    }

}