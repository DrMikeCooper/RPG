using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class TargetPreview : MonoBehaviour
    {
        public Material previewMaterial;
        public Color targetColor = Color.white;
        public Color previewColorCan = Color.yellow;
        public Color previewColorCant = Color.red;

        Character character;
        GameObject previewObject;
        MeshFilter previewMeshFilter;
        MeshRenderer previewMeshRenderer;
        Mesh previewSphere;
        Mesh previewCone;
        PowerArea previewPower;
        
        bool previewOnTarget;
        List<Character> previewTargets = new List<Character>();
        List<Character> targets = new List<Character>();

        void Start()
        {
            character = GetComponent<Character>();
        }

        void Update()
        {
            targets.Clear();
            if (previewPower)
            {
                Transform t = (previewOnTarget && character.target) ? character.target.transform : character.transform;
                GetPreviewObject().SetActive(true);
                GetPreviewObject().transform.position = t.position + Vector3.up;
                GetPreviewObject().transform.forward = character.target ? character.target.transform.position - character.transform.position : character.transform.forward;
                previewMeshRenderer.material.SetColor("_TintColor", previewPower.CanUse(character) ? previewColorCan : previewColorCant);

                if (previewPower.angle >= 360)
                {
                    previewMeshFilter.mesh = previewSphere;
                    GetPreviewObject().transform.localScale = 2 * previewPower.radius * Vector3.one;
                }
                else
                {
                    previewMeshFilter.mesh = previewCone;
                    float x0 = Mathf.Tan(previewPower.angle * 0.5f * Mathf.Deg2Rad);
                    GetPreviewObject().transform.localScale = new Vector3(x0, x0, 1) * previewPower.radius;
                }
                targets = previewPower.GetTargets(character, GetPreviewObject().transform.position, GetPreviewObject().transform.forward);
            }

            // update multi-reticles
            foreach (Character ch in previewTargets)
            {
                if (targets.Contains(ch) == false)
                    ch.GetReticle().SetActive(false);
            }
            previewTargets.Clear();
            foreach (Character ch in targets)
            {
                ch.GetReticle().transform.localScale = 0.8f * Vector3.one;
                ch.ShowReticle(targetColor);
                previewTargets.Add(ch);
            }
        }

        public void StartPreview(Power p)
        {
            previewPower = p as PowerArea;
            previewOnTarget = false;
            // is its not an Area power, check for explosive effects and preview them
            if (previewPower == null)
            {
                previewOnTarget = false;
                foreach (Status s in p.effects)
                {
                    Explosion ex = s as Explosion;
                    if (ex)
                    {
                        previewPower = ex.explosion;
                        previewOnTarget = true;
                    }
                }
            }
        }

        public void EndPreview()
        {
            previewPower = null;
            GetPreviewObject().SetActive(false);
        }

        GameObject GetPreviewObject()
        {
            if (previewObject == null)
            {
                previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                previewObject.GetComponent<SphereCollider>().enabled = false;
                previewMeshFilter = previewObject.GetComponent<MeshFilter>();
                previewSphere = previewMeshFilter.mesh;
                previewObject.SetActive(false);
                previewMeshRenderer = previewObject.GetComponent<MeshRenderer>();
                previewMeshRenderer.material = previewMaterial;

                // whip up a quick cone
                previewCone = MeshUtilities.MakeCone(8, 0.05f, 1);

            }
            return previewObject;
        }
    }
}
