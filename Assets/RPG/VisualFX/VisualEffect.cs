﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class VisualEffect : ScriptableObject
    {
        public float lifespan = 5; // how long before ending it?

        public enum ScalingType
        {
            ScaleNone,
            ScaleRadius,
            ScaleLifeTime,
            ScaleSpeed,
        };
        public ScalingType scalingType;

        public abstract GameObject Begin(Transform t, RPGSettings.Tint tint, bool autoStop = true, bool autoDestroy = true);
        public abstract void End(GameObject go);
        public virtual void ScaleToRadius(GameObject go, float radius) { }

    }
}
