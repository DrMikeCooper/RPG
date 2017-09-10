using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Glow : MonoBehaviour
    {
        public Color color;
        SkinnedMeshRenderer[] meshes;
        Color[] baseColors;
        public float pulseSpeed = 0;
        public float pulseBase = 0.5f;
        [HideInInspector]
        public float fadeTimer = 1;
        public float pulseTimer;

        public enum ColorNames
        {
            _EmissionColor,
            _Color,
            _TintColor,
        }

        public ColorNames colorName = ColorNames._EmissionColor;

        void Start()
        {
            meshes = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            baseColors = new Color[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
                baseColors[i] = meshes[i].material.GetColor(colorName.ToString());

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
                if (colorName == ColorNames._EmissionColor)
                {
                    mesh.material.EnableKeyword("_EMISSION");
                    mesh.material.SetFloat("_EmissionScaleUI", color.a);
                }
                mesh.material.SetColor(colorName.ToString(), color);
                
                Debug.Log("emission on " + mesh.name);
            }
            fadeTimer = 1;
            pulseTimer = 1;
        }

        // Update is called once per frame
        public void UpdateGlow(float alpha)
        {
            fadeTimer = alpha;
        }

        void Update()
        {
            float alpha = fadeTimer;

            if (pulseSpeed > 0)
            {
                pulseTimer += Time.deltaTime*pulseSpeed;
                alpha *= pulseBase + (1.0f- pulseBase) * Mathf.PingPong(pulseTimer, 1.0f);
            }

            Color col = color;
            col.a *= alpha;

            int i = 0;
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                Color col0 = col;
                if (colorName == ColorNames._Color)
                {
                    col0.r = baseColors[i].r * (1 - alpha) + col.r * alpha;
                    col0.g = baseColors[i].g * (1 - alpha) + col.g * alpha;
                    col0.b = baseColors[i].b * (1 - alpha) + col.b * alpha;
                }
                if (colorName == ColorNames._EmissionColor)
                {
                    col0 *= Mathf.LinearToGammaSpace(col.a);
                }

                mesh.material.SetColor(colorName.ToString(), col0);
                Debug.Log("emission on " + mesh.name);
                i++;
            }
        }

        public void Restore()
        {
            // return to normal
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                if (colorName == ColorNames._EmissionColor)
                    mesh.material.DisableKeyword("_EMISSION");
                for (int i = 0; i < meshes.Length; i++)
                    meshes[i].material.SetColor(colorName.ToString(), baseColors[i]);
            }
        }
    }
}