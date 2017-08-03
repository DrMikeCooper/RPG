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
            objects = new GameObject[number];
            for (int i = 0; i < number; i++)
            {
                objects[i] = Instantiate(prefab);
                objects[i].transform.parent = transform;
                objects[i].name = prefab.name + "_obj_" + i;
                objects[i].SetActive(false);
            }
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
            Debug.Log("Object Pool " + gameObject.name + "has run out of objects!");
            return null;
        }
    }
}