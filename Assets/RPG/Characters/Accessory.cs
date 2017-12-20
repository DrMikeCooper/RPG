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
        public static string prefix = "Accessory_";

        public void Apply(Character ch, bool replaceParts = true)
        {
            Transform t = ch.GetBodyPart(bodyPart);
            string partName = prefix + prefab.name;

            Transform child = t.Find(partName);
            if (child != null)
            {
                if (!replaceParts)
                    return;
                else
                    DestroyImmediate(child.gameObject);
            }

            GameObject go = Instantiate(prefab, t);
            go.name = partName;
            go.transform.localPosition = offset;
            go.transform.eulerAngles = ch.transform.eulerAngles;
        }
    }
}
