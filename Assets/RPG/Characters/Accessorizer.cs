using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Accessorizer : MonoBehaviour
    {
        public Accessory[] accessories;

        // Use this for initialization
        void Start()
        {
            Apply(false);
        }

        [ContextMenu("Apply Parts")]
        void ApplyParts()
        {
            Apply(true);
        }

        [ContextMenu("Remove All")]
        void RemoveParts()
        {
            Transform[] childs = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in childs)
                if (child.gameObject.name.StartsWith(Accessory.prefix))
                    DestroyImmediate(child.gameObject);

        }

        void Apply(bool replaceParts)
        {
            Character ch = GetComponent<Character>();
            foreach (Accessory ac in accessories)
                if (ac)
                    ac.Apply(ch, replaceParts);
        }
    }
}
