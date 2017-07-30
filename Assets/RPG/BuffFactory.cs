using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

namespace RPG
{
    public class BuffFactory : MonoBehaviour
    {
        static BuffFactory instance;

        // describes the particles, icon and ingame effect used by a particular buff
        [Serializable]
        public struct BuffSettings
        {
            public ParticleSystem particles;
            public Status status;
            public Sprite icon;
            public Color color;
        }

        // array visible from the inspector
        public BuffSettings[] buffSettings;

        // internal dictionary for fast lookup
        Dictionary<string, BuffSettings> settings = new Dictionary<string, BuffSettings>();

        void Start()
        {
            instance = this;

            //Add("Bubbles", new BuffSettings(Buff.GetEnergiseBuff(5, 10), "sparkles", "ButtonAcceleratorOverSprite", Color.blue));
            //Add("Regen", new BuffSettings(Buff.GetRegenBuff(5, 10), "sparkles", "ButtonArrowUpSprite", Color.red));
            //Add("Armour", new BuffSettings(Buff.GetResBuff(Attack.DamageType.Crushing, 100, 10), "ring", "ButtonBrakeOverSprite", Color.yellow));
        }

        void Add(string name, BuffSettings setting)
        {
            setting.status.name = name;
            setting.status.color = setting.color;
            settings[name] = setting;
        }

        public static BuffSettings GetSettings(string name)
        {
            return instance.settings[name];
        }

        public static ParticleSystem GetParticles(string name)
        {
            return instance.settings[name].particles;
        }

        public static Sprite GetIcon(string name)
        {
            return instance.settings[name].icon;
        }

        public static Color GetColor(string name)
        {
            return instance.settings[name].color;
        }

        public static Status GetStatusEffect(string name)
        {
            return instance.settings[name].status;
        }
    }
}
