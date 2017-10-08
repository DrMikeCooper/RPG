using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;
        public int number;

        GameObject[] objects;

        // Use this for initialization
        void Start()
        {
            GrowBy(number);
        }

        public GameObject GetObject()
        {
            for (int i = 0; i < number; i++)
            {
                if (objects[i].activeSelf == false)
                {
                    objects[i].SetActive(true);
                    return objects[i];
                }
            }

            // allocate more then!
            int index = objects.Length;
            GrowBy(number);
            return objects[index];
        }

        void GrowBy(int delta)
        {
            int num = objects == null ? 0 : objects.Length;
            GameObject[] newObjects = new GameObject[num + delta];

            // copy old ones across
            for (int i = 0; i < num; i++)
                newObjects[i] = objects[i];

            // allocate new ones
            for (int i = 0; i < delta; i++)
            {
                newObjects[i + num] = Instantiate(prefab);
                newObjects[i + num].transform.parent = transform;
                newObjects[i + num].name = prefab.name + "_obj_" + (i+ num);
                newObjects[i + num].SetActive(false);
            }

            objects = newObjects;
        }
    }
}