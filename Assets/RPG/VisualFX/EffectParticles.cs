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
            GameObject go = ObjectFactory.GetObject(prefab, t);
            go.transform.localPosition = prefab.transform.position;
            go.gameObject.name = prefab.name;

            // deals with trail renderers, mesh renderers and particle renderers if they use the Particles shaders (and not the mobile ones)
            Renderer r = go.GetComponent<Renderer>();
            if (r)
                r.material.SetColor("_TintColor", tint.GetColor());

            fader = go.GetComponent<LifeSpanFader>();
            if (fader == null)
                fader = go.AddComponent<LifeSpanFader>();

            // particle systems should loop if we're going to tell them when to finish later
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps)
            {
                ParticleSystem.MainModule main = ps.main;
                main.loop = !autoStop;
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

        public override void ScaleToRadius(GameObject go, float radius)
        {
            if (scalingType != VisualEffect.ScalingType.ScaleNone)
            {
                ParticleSystem ps = go.GetComponent<ParticleSystem>();
                if (ps)
                {
                    ParticleSystem.MainModule main = ps.main;
                    ParticleSystem.MinMaxCurve curve;
                    switch (scalingType)
                    {
                        case VisualEffect.ScalingType.ScaleSpeed:
                            curve = main.startSpeed;
                            curve.constant = radius / main.startLifetime.constant;
                            main.startSpeed = curve;
                            break;
                        case VisualEffect.ScalingType.ScaleLifeTime:
                            curve = main.startLifetime;
                            curve.constant = radius / main.startSpeed.constant;
                            main.startLifetime = curve;
                            break;
                        case VisualEffect.ScalingType.ScaleRadius:
                            //beamParticles.transform.localScale = new Vector3(1,1,Vector3.Distance(target.transform.position, source.position));
                            ParticleSystem.ShapeModule shape = ps.GetComponent<ParticleSystem>().shape;
                            shape.radius = radius;
                            break;
                    }
                }
            }
            ExpandoSphere exp = go.GetComponent<ExpandoSphere>();
            if (exp)
                exp.endRadius = 2 * radius;
        }
    }

}