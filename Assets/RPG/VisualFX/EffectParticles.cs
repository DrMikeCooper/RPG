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

        public override GameObject Begin(Transform t, RPGSettings.Tint tint, bool autoStop = true, bool autoDestroy = true)
        {
            GameObject go = Instantiate(prefab, t);
            go.transform.localPosition = prefab.transform.position;
            go.gameObject.name = prefab.name;

            // deals with trail renderers, mesh renderers and particle renderers if they use the Particles shaders (and not the mobile ones)
            Renderer r = go.GetComponent<Renderer>();
            if (r)
                r.material.SetColor("_TintColor", tint.GetColor());

            fader = go.GetComponent<LifeSpanFader>();
            if (fader == null)
                fader = go.AddComponent<LifeSpanFader>();

            // make any particle systems loop 
            if (!autoStop)
            {
                ParticleSystem ps = go.GetComponent<ParticleSystem>();
                if (ps)
                {
                    ParticleSystem.MainModule main = ps.main;
                    main.loop = true;
                }
            }

            fader.enabled = autoStop;
            fader.lifespan = lifespan;
            fader.color = tint.GetColor();
            fader.autoDestroy = autoDestroy;

            return go;
        }

        public override void End(GameObject go)
        {
            LifeSpanFader fd = go.GetComponent<LifeSpanFader>();
            if (fd)
                fd.enabled = true;
        }
    }

}