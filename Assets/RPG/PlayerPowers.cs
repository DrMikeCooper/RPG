using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour
    {
        Character character;
        GameObject previewObject;
        MeshFilter previewMeshFilter;
        Mesh previewSphere;
        Mesh previewCone;
        PowerArea previewPower;
        public Material previewMaterial;
        bool previewOnTarget;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode key = KeyCode.Alpha1;
            foreach (Power p in character.powers)
            {
                if (Input.GetKeyDown(key) && character.activePower == null && character.animLock == false)
                {
                    p.OnStart(character);
                    previewPower = p as PowerArea;
                    previewOnTarget = false;
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
                
                if (Input.GetKeyUp(key))
                {
                    if (character.activePower != null) // == p.GetPower(character))
                        p.OnEnd(character);
                    previewPower = null;
                    GetPreviewObject().SetActive(false);
                }
                key++;
            }
            if (previewPower)
            {
                Transform t = (previewOnTarget && character.target) ? character.target.transform : character.transform;
                GetPreviewObject().SetActive(true);
                GetPreviewObject().transform.position = t.position + Vector3.up;
                GetPreviewObject().transform.forward = t.forward;
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
            }

            if (character.activePower)
            {
                character.activePower.OnUpdate(character);
            }
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
                previewObject.GetComponent<MeshRenderer>().material = previewMaterial;

                // cone from cylinder
                /*GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Mesh coneMesh = cylinder.GetComponent<MeshFilter>().mesh;

                previewCone = new Mesh();
                Vector3[] vertices = new Vector3[coneMesh.vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 pos = coneMesh.vertices[i];
                    float xScale = pos.y < 0 ? 0.01f : 2.0f;
                    vertices[i] = new Vector3(pos.x * xScale, pos.y, pos.z * xScale);
                }
                previewCone.vertices = coneMesh.vertices;
                previewCone.normals = coneMesh.normals;
                previewCone.uv = coneMesh.uv;
                previewCone.triangles = coneMesh.triangles;

                Destroy(cylinder);*/

                // whip up a quick cone
                previewCone = MeshUtilities.MakeCone(8, 0.05f, 1);
                
            }
            return previewObject;
        }
    }
}