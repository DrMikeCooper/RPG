using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class MeshUtilities : MonoBehaviour
    {
        public enum MeshPrimitive
        {
            Cone,
        };
        public MeshPrimitive primitive;
        public int numRings;
        public float startRadius;
        public float length;
        public float offset;

        void Start()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf)
            {
                if (primitive == MeshPrimitive.Cone)
                    mf.mesh = MakeCone(numRings, startRadius, length, offset);
             }
        }

        public static Mesh MakeCone(int numRings, float start, float length, float offset = 0)
        {
            Mesh cone = new Mesh();
            Vector3[] vertices = new Vector3[32 * numRings];
            Vector3[] normals = new Vector3[32 * numRings];
            Vector2[] uvs = new Vector2[32 * numRings];

            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < numRings; i++)
                {
                    float angle = j * Mathf.PI / 16.0f;
                    float radius = start + i * (1.0f - start) / (numRings - 1);
                    vertices[j * numRings + i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, radius+offset);
                    normals[j * numRings + i] = vertices[j * 2 + 1] - 2 * Vector3.forward;
                    int j0 = j >= 16 ? 32 - j : j;
                    uvs[j * numRings + i] = new Vector2(j0 / 16.0f, i / (float)(numRings - 1));
                }
            }

            cone.vertices = vertices;
            cone.normals = normals;
            cone.uv = uvs;

            int[] triangles = new int[32 * (numRings - 1) * 6];
            int k = 0;
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < numRings - 1; j++)
                {
                    int ip1 = (i + 1) & 31;

                    triangles[k] = j + i * numRings; k++;
                    triangles[k] = j + ip1 * numRings; k++;
                    triangles[k] = j + i * numRings + 1; k++;

                    triangles[k] = j + ip1 * numRings; k++;
                    triangles[k] = j + i * numRings + 1; k++;
                    triangles[k] = j + ip1 * numRings + 1; k++;
                }
            }
            cone.triangles = triangles;
            return cone;
        }

        
    }
}
