using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "BeamParticles", menuName = "RPG/Visual FX/BeamParticles", order = 2)]
    public class BeamParticles : ScriptableObject
    {
        public enum ScalingType
        {
            ScaleLength,
            ScaleLifeTime,
        };
    
        public GameObject prefab;
        public ScalingType scalingType;
        
        ParticleSystem particles;

        float baseEmissionRate;

        // Use this for initialization
        public void Init(Transform parent)
        { 
            GameObject go = ObjectFactory.GetObject(prefab, parent);
            particles = go.GetComponent<ParticleSystem>();

            ParticleSystem.EmissionModule emission = particles.emission;
            baseEmissionRate = emission.rateOverTime.constant;
        }

        // Update is called once per frame
        public void UpdateParticles(BeamRenderer br)
        {
            Transform target = br.target.transform;

            float timer = br.timer;

            // update particles - position the at the parent transform
            particles.transform.localPosition = Vector3.zero;
            //point them at the target
            particles.transform.forward = target.transform.position - particles.transform.position;
            float dist = Vector3.Distance(target.transform.position, particles.transform.position);

            switch (scalingType)
            {
                case ScalingType.ScaleLength:
                    //beamParticles.transform.localScale = new Vector3(1,1,Vector3.Distance(target.transform.position, source.position));
                    ParticleSystem.ShapeModule shape = particles.GetComponent<ParticleSystem>().shape;
                    shape.length = dist;

                    // scale emwission rate to increase over time
                    ParticleSystem.EmissionModule emission = particles.emission;
                    ParticleSystem.MinMaxCurve eCurve = emission.rateOverTime;
                    eCurve.constant = baseEmissionRate * dist;
                    emission.rateOverTime = eCurve;
                    break;
                case ScalingType.ScaleLifeTime:
                    ParticleSystem.MainModule main = particles.main;
                    ParticleSystem.MinMaxCurve sCurve = main.startLifetime;
                    sCurve.constant = dist/main.startSpeed.constant;
                    main.startLifetime = sCurve;
                    break;
            }

            if (timer < 1.01f)
                Stop();
        }

        public void Stop()
        {
            particles.Stop();
        }

        public void Finish()
        {
            // TODO - make some pools!
            particles.gameObject.SetActive(false);
            ObjectFactory.Recycle(particles.gameObject); // had a 01.f delay...
        }
    }
}
