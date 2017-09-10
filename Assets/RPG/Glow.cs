using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Glow : MonoBehaviour
    {
        public Color color;
        SkinnedMeshRenderer[] meshes;

        void Start()
        {
            meshes = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            TurnOn();
        }

        // Use this for initialization
        void OnEnable()
        {
            TurnOn();
        }

        void TurnOn()
        { 
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                mesh.material.EnableKeyword("_EMISSION");
                //mesh.material.GetColor("_EmissionColor")
                mesh.material.SetColor("_EmissionColor", color);
                mesh.material.SetFloat("_EmissionScaleUI", color.a);
                Debug.Log("emission on " + mesh.name);
            }
        }

        // Update is called once per frame
        public void Update(float alpha)
        {
            Color col = color;
            col.a *= alpha;
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                mesh.material.SetColor("_EmissionColor", col);
                mesh.material.SetFloat("_EmissionScaleUI", col.a);
                Debug.Log("emission on " + mesh.name);
            }
        }

        public void Restore()
        {
            // return to normal
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                mesh.material.DisableKeyword("_EMISSION");
                //mesh.material.GetColor("_EmissionColor")
                //mesh.material.SetColor("_EmissionColor", color);
                //Debug.Log("emission on " + mesh.name);
            }
        }
    }
}