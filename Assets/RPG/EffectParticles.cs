using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "EffectParticles", menuName = "RPG/EffectParticles", order = 3)]
    public class EffectParticles : VisualEffect
    {
        public GameObject prefab; // reference to the prefab set up in editor
        LifeSpanFader fader;

        public override GameObject Begin(Transform t, RPGSettings.ColorCode color, bool autoStop = true)
        {
            GameObject go = Instantiate(prefab);
            go.transform.parent = t;
            go.transform.localPosition = Vector3.zero;
            go.gameObject.name = prefab.name;

            // set color of particle systems
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps)
                ps.startColor = RPGSettings.GetColor(color);

            // deals with trailrenderers and meshrenderers
            Renderer r = go.GetComponent<Renderer>();
            if (r)
                r.material.SetColor("_TintColor", RPGSettings.GetColor(color));

            fader = go.GetComponent<LifeSpanFader>();
            if (fader == null)
                fader = go.AddComponent<LifeSpanFader>();

            fader.enabled = autoStop;
            fader.lifespan = lifespan;
            fader.color = RPGSettings.GetColor(color);
            fader.autoDestroy = autoStop;

            return go;
        }

        public override void End(GameObject go)
        {
            fader.enabled = true;
        }
    }

}