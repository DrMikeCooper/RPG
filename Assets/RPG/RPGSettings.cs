using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class RPGSettings : MonoBehaviour
    {
        [Tooltip("Standard HUD to display over the head of every character in the scene")]
        public GameObject overheadHUD;

        [HideInInspector]
        public static RPGSettings instance; // singleton accessor

        public ObjectPool numbersPool;

        public enum DamageType
        {
            Crushing = 1,
            Piercing = 2,
            Fire = 4,
            Cold = 8,
            Energy = 16,
            Negative = 32,
            Psionic = 64,
            Magic = 128,
            Toxic = 256,
            Physical = Crushing | Piercing | Toxic,
            Elemental = Fire | Cold | Energy,
            Paranormal = Negative | Psionic | Magic,
            All = Physical | Elemental | Paranormal
        };
        public const int BasicDamageTypesCount = 9; 

        public enum StatName
        {
            Res,
            Dam,
            Def, // this is the last composite one, check Buff.Apply to see how its used
            EnergyRegen,
            HealthRegen,
            Speed,
            Jump,
            Recharge,
            Health,
            Energy,
        }


        public enum ColorCode
        {
            Health,
            Mana,
            Crushing, 
            Piercing,
            Fire,
            Cold,
            Energy,
            Negative,
            Psionic,
            Magic,
            Physical,
            Elemental,
            Paranormal,
            Custom
        };

        public GameObject projectile;

        public Color[] colors = new Color[(int)ColorCode.Custom];

        public static Color GetColor(ColorCode cc)
        {
            return instance.colors[(int)cc];
        }
    
        // Use this for initialization
        void Awake()
        {
            instance = this;
        }

        public void SetupCharacter(Character character)
        {
            // copy the HUD template and make it this
            GameObject hud = Instantiate(overheadHUD);
            hud.name = "HUD_" + character.name;
            hud.transform.SetParent(transform);
            hud.GetComponent<CharacterHUD>().character = character;
        }

        // Utility functions for damage types
        public static bool IsComposite(DamageType dt)
        {
            return (dt == DamageType.All) || (dt == DamageType.Physical) || (dt == DamageType.Paranormal) || (dt == DamageType.Elemental);
        }

        // if an attack gets set a composite type, return an appropriate damage type for it.
        static DamageType GetDamageType(DamageType dt)
        {
            if (dt == DamageType.All || dt == DamageType.Physical)
                return DamageType.Crushing;
            if (dt == DamageType.Elemental)
                return DamageType.Fire;
            if (dt == DamageType.Paranormal)
                return DamageType.Magic;
            return dt;
        }

        public NumberFloater GetNumberFloater()
        {
            GameObject go = numbersPool.GetObject();
            if (go)
                return go.GetComponent<NumberFloater>();
            return null;
        }


        public static string GetResistanceStat(DamageType dt) { return dt.ToString() + StatName.Res.ToString(); }
        public static string GetDefenceStat(DamageType dt) { return dt.ToString() + StatName.Def.ToString(); }
        public static string GetDamageStat(DamageType dt) { return dt.ToString() + StatName.Dam.ToString(); }
    }
}