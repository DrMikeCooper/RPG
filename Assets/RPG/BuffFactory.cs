using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class BuffFactory : MonoBehaviour
    {

        public ParticleSystem[] particles;
        public Image[] images;
        static BuffFactory instance;

        // describes the particles, icon and ingame effect used by a particular buff
        public class BuffSettings
        {
            public BuffSettings(Status s, string pname, string iname, Color col)
            {
                // find the named particle system (can be null)
                if (pname != "")
                {
                    foreach (ParticleSystem p in BuffFactory.instance.particles)
                        if (p.gameObject.name == pname)
                            particles = p;
                }

                // in game effects of this setting
                status = s;

                // find the named icon
                foreach (Image i in BuffFactory.instance.images)
                    if (i.gameObject.name == iname)
                        icon = i;

                color = col;
            }

            public ParticleSystem particles;
            public Status status;
            public Image icon;
            public Color color;
        }

        Dictionary<string, BuffSettings> settings = new Dictionary<string, BuffSettings>();

        void Start()
        {
            instance = this;

            Add("Bubbles", new BuffSettings(Buff.GetEnergiseBuff(5, 10), "sparkles", "Icon", Color.blue));
            Add("Regen", new BuffSettings(Buff.GetRegenBuff(5, 10), "sparkles", "Icon", Color.red));
            Add("Armour", new BuffSettings(Buff.GetResBuff(Attack.DamageType.Crushing, 5, 10), "sparkles", "Icon", Color.white));
        }

        void Add(string name, BuffSettings setting)
        {
            setting.status.name = name;
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

        public static Image GetIcon(string name)
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
