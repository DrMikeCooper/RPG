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

        public override GameObject Begin(Transform t, RPGSettings.Tint tint, bool autoStop = true)
        {
            GameObject go = Instantiate(prefab);
            go.transform.parent = t;
            go.transform.localPosition = prefab.transform.position;
            go.gameObject.name = prefab.name;

            // deals with trail renderers, mesh renderers and particle renderers if they use the Particles shaders (and not the mobile ones)
            Renderer r = go.GetComponent<Renderer>();
            if (r)
                r.material.SetColor("_TintColor", tint.GetColor());

            fader = go.GetComponent<LifeSpanFader>();
            if (fader == null)
                fader = go.AddComponent<LifeSpanFader>();

            fader.enabled = autoStop;
            fader.lifespan = lifespan;
            fader.color = tint.GetColor();
            fader.autoDestroy = autoStop;

            return go;
        }

        public override void End(GameObject go)
        {
            fader.enabled = true;
        }
    }

}