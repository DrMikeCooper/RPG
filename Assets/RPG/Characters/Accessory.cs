using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Accessory", menuName = "RPG/Accessory")]
    public class Accessory : ScriptableObject
    {
        public GameObject prefab;
        public Character.BodyPart bodyPart;
        public Vector3 offset;

        public void Apply(Character ch)
        {
            Transform t = ch.GetBodyPart(bodyPart);
            GameObject go = Instantiate(prefab, t);
            go.transform.localPosition = offset;
            go.transform.eulerAngles = ch.transform.eulerAngles;
        }
    }
}
