using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class VisualEffect : ScriptableObject
    {
        public float lifespan = 5; // how long before ending it?

        public abstract GameObject Begin(Transform t, RPGSettings.ColorCode color, bool autoStop = true);
        public abstract void End(GameObject go);

    }
}
