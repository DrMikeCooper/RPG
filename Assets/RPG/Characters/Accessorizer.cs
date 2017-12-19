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
            Character ch = GetComponent<Character>();
            foreach (Accessory ac in accessories)
                ac.Apply(ch);
        }
    }
}
